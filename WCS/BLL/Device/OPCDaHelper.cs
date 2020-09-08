using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Bll;
using Device;

namespace OPC
{
    /// <summary>
    /// OPC Helper类，只创建一个同步组和一个异步组，调用设备函数等待修改
    /// </summary>
    public class OPCDaHelper
    {
        #region Fields

        char _idAimSplitStr = '@';

        /// <summary>
        /// OpcDaConnection对象
        /// </summary>
        OpcDaConnection _opcDaConn;

        SyncGroupOpcInfo _syncGroupOpcInfo;

        AsyncGroupOpcInfo _asyncGroupOpcInfo;

        string[] _idAim;
        int[] _syncItemHandle;

        Dictionary<int, string> AsyncItemHandleToDeviceDic = new Dictionary<int, string>();

        #endregion


        public OPCDaHelper()
        {
            _opcDaConn = OpcDaConnection.GetInstance();
            _opcDaConn._dgOnDataChange += OnDataChange;
        }

        private string BindOPCDb()
        {
            string errText = string.Empty;

            DataSet ds = DbInf.GetOPCDbInfo(ref errText);
            if (errText.Length > 0)
                return errText;

            int itemNum = 0;
            //创建同步组
            DataRow[] drs = ds.Tables[0].Select("type = 'SYNC'");
            if ((itemNum = drs.Length) > 0)
            {
                _idAim = new string[itemNum];
                _syncItemHandle = new int[itemNum];

                //添加组
                errText = _opcDaConn.AddSyncGroup(out _syncGroupOpcInfo);
                if (errText.Length > 0)
                    return errText;

                //组装对象
                OpcRcw.Da.OPCITEMDEF[] items = new OpcRcw.Da.OPCITEMDEF[itemNum];
                for (int i = 0; i < itemNum; i++)
                {
                    items[i].szAccessPath = "";
                    items[i].bActive = 1;
                    items[i].dwBlobSize = 1;
                    items[i].pBlob = IntPtr.Zero;
                    items[i].hClient = i;
                    items[i].vtRequestedDataType = (short)VarEnum.VT_BSTR;
                    items[i].szItemID = drs[i]["db_str"].ToString();

                    _idAim[i] = drs[i]["id"].ToString() + _idAimSplitStr + drs[i]["db_aim"].ToString();
                }
                //添加对象
                errText = _opcDaConn.AddSyncItems(_syncGroupOpcInfo.SyncGroupObj, items, _syncItemHandle);
                if (errText.Length > 0)
                    return errText;
            }

            //创建异步组
            drs = ds.Tables[0].Select("type = 'ASYNC'");
            if ((itemNum = drs.Length) > 0)
            {
                _idAim = new string[itemNum];

                //添加组
                errText = _opcDaConn.AddAsyncGroup(out _asyncGroupOpcInfo);
                if (errText.Length > 0)
                    return errText;

                //组装对象
                OpcRcw.Da.OPCITEMDEF[] items = new OpcRcw.Da.OPCITEMDEF[itemNum];
                for (int i = 0; i < itemNum; i++)
                {
                    items[i].szAccessPath = "";
                    items[i].bActive = 1;
                    items[i].dwBlobSize = 1;
                    items[i].pBlob = IntPtr.Zero;
                    items[i].hClient = i;
                    items[i].vtRequestedDataType = (short)VarEnum.VT_BSTR;
                    items[i].szItemID = drs[i]["db_str"].ToString();

                    AsyncItemHandleToDeviceDic.Add(i, drs[i]["id"].ToString() + _idAimSplitStr + drs[i]["db_aim"].ToString());
                }
                //添加对象
                int[] itemHandle = new int[itemNum];
                _opcDaConn.AddAsyncItems(_asyncGroupOpcInfo.AsyncGroupObj, items, itemHandle);
                if (errText.Length > 0)
                    return errText;
            }
            return errText;
        }

        /// <summary>
        /// opc id+aim find sync itemHandle
        /// </summary>
        /// <param name="id"></param>
        /// <param name="aim"></param>
        /// <returns><0没有 >=0有</returns>
        private int FindSyncItemHandleByIdAim(string id,string aim,ref string errText)
        {
            errText = string.Empty;
            int retVal = 0;
            int index = 0;

            if (_idAim == null || _syncItemHandle == null)
                errText = "无同步db信息";
            else if (_idAim.Length != _syncItemHandle.Length)
                errText = "同步db信息不对应";
            else
            {
                index = _idAim.ToList().IndexOf(id + _idAimSplitStr + aim);
                if (index < 0)
                    errText = string.Format("无设备{0}任务类型{1}对应db信息", id, aim);
                else
                {
                    retVal = _syncItemHandle[index];
                }
            }
            return retVal;
        }

        /// <summary>
        /// 读取设备对应任务的db数据
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="aim"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        public string Read(string deviceId, string aim,ref string errText)
        {
            errText = string.Empty;
            string[] values = new string[1];
            int[] itemHandles = new int[1];

            itemHandles[0] = FindSyncItemHandleByIdAim(deviceId, aim, ref errText);
            if (errText.Length<=0)
                errText = _opcDaConn.SyncRead(_syncGroupOpcInfo.SyncGroupObj, values, itemHandles);

            return values[0];
        }

        public void Write(string deviceId, string aim,string value,ref string errText)
        {
            errText = string.Empty;
            string[] values = new string[1] { value};
            int[] itemHandles = new int[1];

            itemHandles[0] = FindSyncItemHandleByIdAim(deviceId, aim, ref errText);
            if (errText.Length <= 0)
                errText = _opcDaConn.SyncWrite(_syncGroupOpcInfo.SyncGroupObj, values, itemHandles);
        }

        public void OnDataChange(int[] asyncItemHandles, object[] values)
        {
            for (int i = 0; i < asyncItemHandles.Length; i++)
            {
                int itmeHandle = asyncItemHandles[i];
                if (AsyncItemHandleToDeviceDic.ContainsKey(itmeHandle))
                {
                    string[] deviceIdAim = AsyncItemHandleToDeviceDic[itmeHandle].Split(_idAimSplitStr);
                    string deviceId = deviceIdAim[0];
                    string aim = deviceIdAim[1];
                    //调用设备函数
                }
                    
            }
        }
    }
}
