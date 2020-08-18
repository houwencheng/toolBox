using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 静态只读实例
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogHelper));

        static LogHelper()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        public static void LogError(object message, Exception exp)
        {
            log.Error(LogFormat(message), exp);
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <typeparam name="T">调用日志的类型</typeparam>
        /// <param name="message"></param>
        public static void LogError(object message)
        {
            log.Error(LogFormat(message));
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        public static void LogWarn(object message, Exception exp)
        {
            log.Warn(LogFormat(message), exp);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarn(object message)
        {
            log.Warn(LogFormat(message));
        }


        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        public static void LogInfo(object message, Exception exp)
        {
            log.Info(LogFormat(message), exp);
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(object message)
        {
            log.Info(LogFormat(message));
        }

        /// <summary>
        /// 日志格式
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string LogFormat(object message)
        {
            var method = GetStackMethodFullName(3);
            var msg = string.Format("{0} - {1}", method, message);
            return msg;
        }

        /// <summary>
        /// 获取调用堆栈方法名称
        /// </summary>
        /// <param name="depth">层数</param>
        /// <returns></returns>
        private static string GetStackMethodFullName(int depth)
        {
            try
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(depth).GetMethod().Name;
                string className = st.GetFrame(depth).GetMethod().DeclaringType.ToString();
                return className + "." + methodName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 改变默认的日志位置
        /// </summary>
        /// <param name="folder"></param>
        public static void UpdateFolder(string folder)
        {
            var repository = log4net.LogManager.GetRepository();
            var appenders = repository.GetAppenders();
            if (appenders == null) return;
            foreach (var appender in appenders)
            {
                if (appender.GetType().Equals(typeof(log4net.Appender.RollingFileAppender)))
                {
                    var ra = appender as log4net.Appender.RollingFileAppender;
                    ra.File = folder;
                    ra.ActivateOptions();
                }
            }
        }
    }
}
