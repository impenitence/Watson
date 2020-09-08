using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WCF
{
    [ServiceContract]
    public interface IService
    {
        #region for wms

        /// <summary>
        /// 2.1.2 站点（料箱）拣货完成
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string PickOver(string value);

        /// <summary>
        /// 2.1.3 站点迁移或合并（更新店铺位置）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string StationUpdate(string value);

        /// <summary>
        /// 2.1.4 波次完成
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string WaveOver(string value);

        /// <summary>
        /// 2.1.5 站点释放
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string StationRelease(string value);

        /// <summary>
        /// 2.1.8 PTL手工扫描料箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string ManualPick(string value);

        /// <summary>
        /// 2.1.9 二拣
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string WcsOutsidePick(string value);

        /// <summary>
        /// 2.1.10 关闭位置门店
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string CloseTagCustomer(string value);

        /// <summary>
        /// 2.1.11 新增位置门店
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string NewTagCustomer(string value);

        #endregion

        #region for pda

        /// <summary>
        /// log in
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string LogIn(string value);

        /// <summary>
        /// judge wave is legal or not
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string WaveLegal(string value);

        /// <summary>
        /// judge container is legal or not
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string ContainerLegal(string value);

        /// <summary>
        /// input sku ,return note information ,include smart feed pattern and normal feed pattern
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string InputSku(string value);

        /// <summary>
        /// input quantity ,return note information
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string InputQty(string value);

        /// <summary>
        /// current container feed over ,return note information
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string FeedOver(string value);

        /// <summary>
        /// query container details
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string QueryContainer(string value);

        /// <summary>
        /// manual push PTL container details
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string ManualPushPtl(string value);

        /// <summary>
        /// manual release container
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string ReleaseContainer(string value);

        #endregion
    }
}
