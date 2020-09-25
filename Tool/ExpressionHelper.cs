using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 表达式帮助类，通过表达式构建委托方法。
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// 通过实体封送查询条件，转化成Linq查询表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public static Func<T, bool> BuildWhere<T>(T where)
        {
            if (where == null)
            {
                return null;
            }

            ConstantExpression theTrue = Expression.Constant(true);
            BinaryExpression whereExpression = Expression.AndAlso(theTrue, theTrue);
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                if (p.GetValue(where) != null && p.PropertyType == typeof(string))
                {
                    ConstantExpression value = Expression.Constant(p.GetValue(where).ToString().ToLower());

                    MemberExpression property = Expression.Property(parameter, p);
                    MethodCallExpression toLower = Expression.Call(property, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                    MethodCallExpression contains = Expression.Call(toLower, typeof(string).GetMethod("Contains"), value);

                    whereExpression = Expression.AndAlso(whereExpression, contains);
                }
            });

            return Expression.Lambda<Func<T, bool>>(whereExpression, parameter).Compile();
        }

        /// <summary>
        /// 通过实体封送查询条件，转化成Linq查询表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static Func<T, string> BuildOrderBy<T>(T orderBy)
        {
            if (orderBy == null)
            {
                return null;
            }
            Func<T, string> orderByFunc = null;
            typeof(T).GetProperties().ToList().ForEach(p =>
            {
                if (p.GetValue(orderBy) != null)
                {
                    ParameterExpression parameter = Expression.Parameter(typeof(T));
                    MemberExpression property = Expression.Property(parameter, p);

                    orderByFunc = Expression.Lambda<Func<T, string>>(property, parameter).Compile();
                }
            });

            return orderByFunc;
        }
    }
}
