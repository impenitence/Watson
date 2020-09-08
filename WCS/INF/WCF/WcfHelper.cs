using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCF
{
     public class WcfHelper
    {
        #region Fields

        /// <summary>
        /// 宿主
        /// </summary>
        ServiceHost _serviceHost;

        #endregion

        #region Methods

        public WcfHelper()
        {
        }

        public bool OpenService(string className,ref string errText)
        {
            bool flag = false;
            errText = string.Empty;
            try
            {
                _serviceHost = new ServiceHost(Type.GetType(className));
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                errText = ex.Message;
            }
            finally
            {
                if ((_serviceHost != null) && _serviceHost.State == CommunicationState.Opened)
                    flag = true;
            }
            return flag;
        }

        #endregion


    }
}
