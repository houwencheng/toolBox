using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool
{
    /// <summary>
    /// 配置文件帮助类
    /// </summary>
    public static class ConfigHelper
    {
        public static string Get(string key)
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var value = cfg.AppSettings.Settings[key].Value;
            return value;
        }

        [Obsolete]
        public static dynamic GetType<T>(string key)
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var value = cfg.AppSettings.Settings[key].Value;
            if (typeof(T).Equals(typeof(bool)))
            {
                return bool.Parse(value);
            }
            else if (typeof(T).Equals(typeof(string)))
            {
                return value;
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                return int.Parse(value);
            }

            return default(T);
        }

        public static void Set(string key, string value)
        {
            //System.Configuration.ConfigurationSettings.AppSettings.Set(key, value);
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings[key].Value = value;
            cfg.Save();
        }
    }
}
