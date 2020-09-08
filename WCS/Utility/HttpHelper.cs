using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class HttpHelper
    {
        #region Fields

        /// <summary>
        /// 超时时间，单位ms
        /// </summary>
        static int _timeout = 2000;

        #endregion

        #region Properties

        /// <summary>
        /// 超时时间，单位ms
        /// </summary>
        public static int Timeout { get => _timeout; set => _timeout = value; }

        #endregion

        public static void ApiRequest(string url,string method,string contentType)
        {
            //创建连接
            HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            //添加header
            //SetHeaderValue(httpRequest.Headers, "Content-Type", "application/x-www-form-urlencoded");
            //SetHeaderValue(httpRequest.Headers, "Authorization", "Basic YjI4Y2Y3MGE2ODE4NDBmMjg2MTgwOTI4Yzc1NTUzNWM6NzllOTc1ZDdjNTJkNGRiYzk2Njk0NDE3NDM2NTlhY2Y=");
            //发送请求的方式
            httpRequest.Method = "POST";
            //添加body
            string param = "grant_type=authorization_code&code=8873RA&redirect_uri=http://58.221.198.86:8116/api/suhong/HandBind/RecKeyCode";
            byte[] bs = Encoding.ASCII.GetBytes(param);
            httpRequest.ContentLength = bs.Length;
            using (Stream reqStream = httpRequest.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }

            httpRequest.CookieContainer = new CookieContainer();
            //创建一个响应对象
            using (HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse())
            {
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream resultStream = httpResponse.GetResponseStream();
                    StreamReader sr = new StreamReader(resultStream, Encoding.UTF8);
                    string rec = sr.ReadToEnd();
                    sr.Close();
                    resultStream.Close();
                }
                else
                {

                }
                httpResponse.Close();
            }
        }
    }
}
