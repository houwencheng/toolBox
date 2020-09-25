using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 分页查询条件封送，结果返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageQueryObject<T>
    {
        public PageQueryObject()
        {
            ASC = true;
        }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 记录总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 查询条件
        /// </summary>
        public T Where { get; set; }


        private Func<T, bool> whereFunc = null;

        /// <summary>
        /// 查询条件的委托形式
        /// </summary>
        [JsonIgnore]
        public Func<T, bool> WhereFunc
        {
            get { return whereFunc == null ? BuildWhereFunc() : whereFunc; }
            set { whereFunc = value; }
        }

        /// <summary>
        /// 升降序（true: 升序， 否者降序）
        /// </summary>
        public bool ASC { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public T OrderBy { get; set; }

        private Func<T, string> orderByFunc = null;

        /// <summary>
        /// 排序字段的委托形式
        /// </summary>
        [JsonIgnore]
        public Func<T, string> OrderByFunc
        {
            get { return orderByFunc == null ? BuildOrderByFunc() : orderByFunc; }
            set { orderByFunc = value; }
        }

        /// <summary>
        /// 分页数据
        /// </summary>
        public IEnumerable<T> PageResultData { get; set; }


        /// <summary>
        /// 根据where文本生成Fun
        /// </summary>
        /// <returns></returns>
        private Func<T, bool> BuildWhereFunc()
        {
            whereFunc = ExpressionHelper.BuildWhere<T>(Where);
            return whereFunc;
        }

        /// <summary>
        /// 根据OrderBy文本生成Fun
        /// </summary>
        /// <returns></returns>
        private Func<T, string> BuildOrderByFunc()
        {
            orderByFunc = ExpressionHelper.BuildOrderBy<T>(OrderBy);
            return orderByFunc;
        }
    }
}
