using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigUtility
{
    public static class AppConfigHelper
    {
        private static Configuration config;

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key"></param>
        public static string GetConfig(string key)
        {
            try
            {
                OpenConfig();
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("GetConfig:" + ex.Message);
            }
        }

        /// <summary>
        /// 编辑配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetConfig(string key, string value)
        {
            try
            {
                OpenConfig();
                KeyValueConfigurationElement elementKey = config.AppSettings.Settings[key];
                if (elementKey == null)
                {
                    config.AppSettings.Settings.Add(key, "");
                }
                config.AppSettings.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("SetConfig:" + ex.Message);
            }
        }

        /// <summary>
        /// 打开应用程序配置文件
        /// </summary>
        private static void OpenConfig()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (!config.HasFile)
                throw new ArgumentNullException("OpenConfig:程序配置文件缺失");
        }
    }
}
