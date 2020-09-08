using ConfigUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Bll
{
    public static class ValidityCheck
    {
        /// <summary>
        /// 加密key
        /// </summary>
        private static string _key = "bkzn2020";

        /// <summary>
        /// 有效期即将结束提醒
        /// </summary>
        private static int _warnTime = 15;

        /// <summary>
        /// 验证软件使用状态 0无限期 1试用期大于15天 2试用期小于15天 3过期，必须注册
        /// </summary>
        /// <param name="validityKey">使用期字符串秘钥</param>
        /// <returns>0无限期 1试用期，早于提示注册时间 2试用期，晚于提示注册时间 3过期，必须注册</returns>
        public static int CheckValidity(string validityKey)
        {
            try
            {
                if (validityKey == null)
                    return 3;
                string[] validityInfo = DesBase.DesDecrypt(validityKey, _key).Split('#');
                if (validityInfo[0] == "0")
                    return 0;
                DateTime startTime = Convert.ToDateTime(validityInfo[0]);
                DateTime endTime = startTime.AddDays(int.Parse(validityInfo[1]));
                if (DateTime.Compare(DateTime.Now.Date, endTime) > 0)
                    return 3;
                if (DateTime.Compare(DateTime.Now.Date, endTime.AddDays((-1) * _warnTime)) > 0)
                    return 2;
                return 1;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("CheckValidity:" + ex.Message);
            }
        }

        public static string CreateKey(string licence)
        {
            return DesBase.DesEncrypt(licence, _key);
        }

        #region 获取注册码,并验证
        public static bool ValiteLimt(ref string note)
        {
            try
            {
                //2018-05-29#30
                note = string.Empty;
                string pass_word = AppConfigHelper.GetConfig("pass_word");
                string tempLimit = DesBase.DesDecrypt(pass_word, _key);
                int sIndex = tempLimit.IndexOf('#');
                string sTime = tempLimit.Substring(0, sIndex);
                string limit = tempLimit.Substring(sIndex + 1);
                DateTime stime = Convert.ToDateTime(sTime);
                DateTime endtime = stime.AddDays(int.Parse(limit));
                if (DateTime.Compare(DateTime.Now.Date, endtime) > 0)
                    return false;
                if (DateTime.Compare(DateTime.Now.Date, endtime.AddDays((-1) * _warnTime)) > 0)
                {
                    TimeSpan tSpan = endtime.Subtract(DateTime.Now.Date).Duration();
                    string time = tSpan.Days.ToString();
                    note = "系统剩余使用时间" + time + "天";
                }
                return true;
            }
            catch (Exception ex)
            {
                note += "验证码错误,请输入正确的注册码！\r\n" + ex.Message;
                return false;
            }
        }
        #endregion
    }
}
