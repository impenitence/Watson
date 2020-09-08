using OpcRcw.Comn;
using OpcRcw.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace OPC
{
    /// <summary>
    /// 单例模式，OPC连接类
    /// </summary>
    public class OpcDaConnection : IOPCDataCallback
    {
        #region Fields
        /// <summary>
        /// 是否创建opc服务对象
        /// </summary>
        private bool _state = false;
        /// <summary>
        /// opc服务
        /// </summary>
        private IOPCServer _opcServer;
        /// <summary>
        /// opc数据组handle
        /// </summary>
        private int _groupHandle = 1;
        /// <summary>
        /// 添加组对象时防止线程冲突
        /// </summary>
        public object _addGroupLockObj = new object();
        /// <summary>
        /// 数据变化回调委托
        /// </summary>
        /// <param name="asyncItemHandles"></param>
        /// <param name="values"></param>
        public DgOnDataChange _dgOnDataChange;
        public delegate void DgOnDataChange(int[] asyncItemHandles, object[] values);

        #region opc通用变量
        /// <summary>
        /// 时差用于调整时区，在此不对时区进行调整
        /// </summary>
        private int _timeBias = 0;
        /// <summary>
        /// 死区百分比，在此不设死区
        /// </summary>
        private float _deadband = 0;
        /// <summary>
        /// the fastest rate at which data changes may be sent to OnDataChange for items in this group
        /// </summary>
        private Int32 _dwRequestedUpdateRate = 500;
        /// <summary>
        /// OPCServer返回文本的语言，0x407为英语实测返回的德语，0x804为中文，实测返回的是英语
        /// </summary>
        private const int LOCALE_ID = 0x804;
        #endregion

        #region singleton

        private volatile static OpcDaConnection _instance;

        public static OpcDaConnection Instance { get => _instance; set => _instance = value; }

        private static Object _singletonLock = new object();

        #endregion

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// 构造函数
        /// </summary>
        private OpcDaConnection()
        {

        }

        public static OpcDaConnection GetInstance()
        {

            if (Instance == null)
            {
                lock (_singletonLock)
                {
                    if(Instance==null)
                        Instance = new OpcDaConnection(); 
                }
            }
            return Instance;
        }

        /// <summary>
        /// 建立OPC连接
        /// </summary>
        /// <returns></returns>
        public string Connect()
        {
            lock (this)
            {
                string retStr = string.Empty;
                if (!_state)
                {
                    try
                    {
                        Type svrComponenttyp = Type.GetTypeFromProgID("OPC.SimaticNet", "localhost");
                        _opcServer = (IOPCServer)Activator.CreateInstance(svrComponenttyp);
                        _state = true;
                    }
                    catch (Exception ex)
                    {
                        //调试时修改
                        //isConnected = true;
                        _state = false;
                        retStr = new StringBuilder().Clear().Append("建立和OPCServer的连接失败:").Append(ex.Message).ToString();
                    }
                }
                return retStr;
            }
        }

        /// <summary>
        /// 断开OPC连接
        /// </summary>
        /// <returns></returns>
        public string DisConnect()
        {
            string retStr = string.Empty;
            try
            {
                _state = false;
                if (_opcServer != null)
                {
                    Marshal.ReleaseComObject(_opcServer);
                    _opcServer = null;
                }
            }
            catch (Exception ex)
            {
                retStr = new StringBuilder().Clear().Append("断开和OPCServer的连接:").Append(ex.Message).ToString();
            }
            return retStr;
        }

        #region Synchronization

        /// <summary>
        /// 添加同步组
        /// </summary>
        /// <returns></returns>
        public string AddSyncGroup(out SyncGroupOpcInfo groupOpcInfo)
        {
            string errText = string.Empty;
            groupOpcInfo = new SyncGroupOpcInfo();
            int tmpGroupHandle;
            lock (_addGroupLockObj)
            {
                tmpGroupHandle = _groupHandle;
                _groupHandle++;
            }
            //返回的实际更新速率值单位ms
            Int32 pRevUpdaterate;
            //客户端的组句柄
            Int32 hClientGroup = tmpGroupHandle;
            //在生成组对象的时候组的异步通信是否被激活，0不被激活
            int bActive = 0;
            string groupName = tmpGroupHandle.ToString();
            GCHandle hTimeBias, hDeadband;
            hTimeBias = GCHandle.Alloc(_timeBias, GCHandleType.Pinned);
            hDeadband = GCHandle.Alloc(_deadband, GCHandleType.Pinned);
            Guid iidRequiredInterface = typeof(IOPCItemMgt).GUID;
            int syncGroupHandle;
            Object syncGroupObj;
            IOPCSyncIO2 syncIOPCIO2Obj;
            try
            {   //返回值类型为void
                _opcServer.AddGroup(groupName, bActive,
                         _dwRequestedUpdateRate, hClientGroup,
                         hTimeBias.AddrOfPinnedObject(), hDeadband.AddrOfPinnedObject(),
                         LOCALE_ID, out syncGroupHandle,
                         out pRevUpdaterate, ref iidRequiredInterface, out syncGroupObj);
                syncIOPCIO2Obj = (IOPCSyncIO2)syncGroupObj;
                groupOpcInfo = new SyncGroupOpcInfo(hClientGroup,syncGroupHandle, syncGroupObj, syncIOPCIO2Obj);
            }
            catch (Exception ex)
            {
                errText = new StringBuilder().Clear().Append("创建同步组对象时出错：").Append(ex.ToString()).ToString();
            }
            finally
            {
                if (hDeadband.IsAllocated) hDeadband.Free();
                if (hTimeBias.IsAllocated) hTimeBias.Free();
            }
            return errText;
        }

        /// </summary>
        /// <param name="syncGroupObj">同步组对象，添加组时产生</param>
        /// <param name="items">添加同步读写数据项，Items为读写对象数组</param>
        /// <returns>添加Items是否执行成功</returns>
        public string AddSyncItems(Object syncGroupObj, OPCITEMDEF[] items, int[] itemHandle)
        {
            string errText = string.Empty;
            IntPtr pResults = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;
            try
            {
                ((IOPCItemMgt)syncGroupObj).AddItems(items.Length, items, out pResults, out pErrors);
                int[] errors = new int[items.Length];
                Marshal.Copy(pErrors, errors, 0, items.Length);
                IntPtr pos = pResults;
                OPCITEMRESULT result;
                for (int i = 0; i < items.Length; i++)
                {
                    if (errors[i] == 0)
                    {
                        result = (OPCITEMRESULT)Marshal.PtrToStructure(pos, typeof(OPCITEMRESULT));
                        itemHandle[i] = result.hServer;
                        pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMRESULT)));
                    }
                    else
                    {
                        String pstrError;
                        _opcServer.GetErrorString(errors[i], LOCALE_ID, out pstrError);
                        errText = new StringBuilder().Clear().Append("添加同步对象")
                                .Append(items[i].szItemID)
                                .Append("时出错：")
                                .Append(pstrError)
                                .ToString();
                        break;
                    }
                }
            }
            catch (System.Exception ex) // catch for add item  
            {
                errText = new StringBuilder().Clear().Append("添加同步Item对象时出错：")
                        .Append(ex.ToString())
                        .ToString();
            }
            finally
            {
                // Free the memory  
                if (pResults != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pResults);
                    pResults = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }
            return errText;
        }

        /// <summary>
        /// 同步写
        /// </summary>
        public string SyncWrite(Object syncGroupObj,object[] values, int[] itemHandle) //由编程人员保证，所写数据和添加Item的数据说明相对应
        {
            IntPtr pErrors = IntPtr.Zero;
            try
            {
                if (values.Length != itemHandle.Length)
                {
                    return new StringBuilder().Clear().Append("同步写数据出错：").Append("写入数据的个数与添加Item的数据说明长度不一致").ToString();
                }
                ((IOPCSyncIO2)syncGroupObj).Write(values.Length, itemHandle, values, out pErrors);//四个参数
                int[] errors = new int[values.Length];
                Marshal.Copy(pErrors, errors, 0, values.Length);
                for (int i = 0; i < values.Length; i++)
                {
                    if (errors[i] != 0)  //写数据不成功
                    {
                        String pstrError;   //需不需要释放？
                        _opcServer.GetErrorString(errors[i], LOCALE_ID, out pstrError);
                        return new StringBuilder().Clear().Append("同步写入第").Append(i.ToString()).Append("个数据时出错：").Append(pstrError).ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return new StringBuilder().Clear().Append("同步写数据出错：").Append(ex.ToString()).ToString();
            }
            finally
            {
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 同步读，读的结果存放在values中，读成功返回true，失败返回false
        /// </summary>
        public string SyncRead(Object syncGroupObj,object[] values, int[] itemHandle)
        {
            string errText = string.Empty;
            IntPtr pItemValues = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;
            try
            {
                if (values.Length != itemHandle.Length)
                {
                    return new StringBuilder().Clear().Append("同步读数据出错：").Append("需要读出数据的个数与添加Item的数据说明长度不一致").ToString();
                }
                ((IOPCSyncIO2)syncGroupObj).Read(OPCDATASOURCE.OPC_DS_DEVICE, itemHandle.Length, itemHandle, out pItemValues, out pErrors);
                int[] errors = new int[itemHandle.Length];
                Marshal.Copy(pErrors, errors, 0, itemHandle.Length);
                OPCITEMSTATE pItemState = new OPCITEMSTATE();
                for (int i = 0; i < itemHandle.Length; i++)
                {
                    if (errors[i] == 0)
                    {
                        pItemState = (OPCITEMSTATE)Marshal.PtrToStructure(pItemValues, typeof(OPCITEMSTATE));
                        values[i] = pItemState.vDataValue.ToString();   //pItemState中还包含质量和时间等信息，目前只使用了读取的数据值
                        pItemValues = new IntPtr(pItemValues.ToInt32() + Marshal.SizeOf(typeof(OPCITEMSTATE)));
                    }
                    else
                    {
                        String pstrError;   //需不需要释放？
                        _opcServer.GetErrorString(errors[i], LOCALE_ID, out pstrError);
                        return new StringBuilder().Clear().Append("读取第").Append(i.ToString()).Append("个数据时出错：").Append(pstrError).ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return new StringBuilder().Clear().Append("同步读数据出错：").Append(ex.ToString()).ToString();
            }
            finally
            {
                if (pItemValues != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pItemValues);
                    pItemValues = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }
        }

        #endregion

        #region Asynchronization

        /// <summary>
        /// 添加异步组,失败返回false，成功返回true
        /// </summary>
        public string AddAsyncGroup(out AsyncGroupOpcInfo groupOpcInfo)
        {
            string errText = string.Empty;
            groupOpcInfo = new AsyncGroupOpcInfo();
            int tmpGroupHandle;
            lock (_addGroupLockObj)
            {
                tmpGroupHandle = _groupHandle;
                _groupHandle++;
            }
            //返回的实际更新速率值单位ms
            Int32 pRevUpdaterate;
            //客户端的组句柄
            Int32 hClientGroup = tmpGroupHandle;
            //在生成组对象的时候组的异步通信是否被激活，0不被激活
            int bActive = 0;
            string groupName = tmpGroupHandle.ToString();
            GCHandle hTimeBias, hDeadband;
            hTimeBias = GCHandle.Alloc(_timeBias, GCHandleType.Pinned);
            hDeadband = GCHandle.Alloc(_deadband, GCHandleType.Pinned);
            Guid iidRequiredInterface = typeof(IOPCItemMgt).GUID;
            int asyncGroupHandle;
            object asyncGroupObj;
            try
            {   //返回值类型为void
                _opcServer.AddGroup(groupName, bActive,
                         _dwRequestedUpdateRate, hClientGroup,
                         hTimeBias.AddrOfPinnedObject(), hDeadband.AddrOfPinnedObject(),
                         LOCALE_ID, out asyncGroupHandle,
                         out pRevUpdaterate, ref iidRequiredInterface, out asyncGroupObj);
                groupOpcInfo = new AsyncGroupOpcInfo(hClientGroup, asyncGroupHandle, asyncGroupObj, (IOPCAsyncIO2)asyncGroupObj, (IOPCGroupStateMgt2)asyncGroupObj);
                IConnectionPointContainer pIConnectionPointContainer = (IConnectionPointContainer)asyncGroupObj;
                IConnectionPoint pIConnectionPoint;
                Int32 dwCookie;
                Guid iid = typeof(IOPCDataCallback).GUID;
                pIConnectionPointContainer.FindConnectionPoint(ref iid, out pIConnectionPoint);
                pIConnectionPoint.Advise(this, out dwCookie);
            }
            catch (System.Exception ex)
            {
                errText = new StringBuilder().Clear().Append("创建异步组对象时出错：").Append(ex.ToString()).ToString();
            }
            finally
            {
                if (hDeadband.IsAllocated) hDeadband.Free();
                if (hTimeBias.IsAllocated) hTimeBias.Free();
            }
            return errText;
        }

        /// </summary>
        /// <param name="items">添加异步读写数据项，Items为读写对象数组</param>
        /// <returns>添加Items是否执行成功</returns>
        public string AddAsyncItems(Object asyncGroupObj, OPCITEMDEF[] items, int[] itemHandle)
        {
            string errText = string.Empty;
            IntPtr pResults = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;
            try
            {
                ((IOPCItemMgt)asyncGroupObj).AddItems(items.Length, items, out pResults, out pErrors);
                int[] errors = new int[items.Length];
                Marshal.Copy(pErrors, errors, 0, items.Length);
                IntPtr pos = pResults;
                OPCITEMRESULT result;
                for (int i = 0; i < items.Length; i++)
                {
                    if (errors[i] == 0)
                    {
                        result = (OPCITEMRESULT)Marshal.PtrToStructure(pos, typeof(OPCITEMRESULT));
                        itemHandle[i] = result.hServer;
                        pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMRESULT)));
                    }
                    else
                    {
                        String pstrError;
                        _opcServer.GetErrorString(errors[i], LOCALE_ID, out pstrError);
                        errText = new StringBuilder().Clear().Append("添加异步对象").Append(items[i].szItemID).Append("时出错：").Append(pstrError).ToString();
                        break;
                    }
                }
            }
            catch (System.Exception ex) // catch for add item  
            {
                errText = new StringBuilder().Clear().Append("添加异步Item对象时出错：").Append(ex.ToString()).ToString();
            }
            finally
            {
                if (pResults != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pResults);
                    pResults = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }
            return errText;
        }

        //异步写

        /// <summary>
        /// 异步读函授
        /// </summary>
        private string AsyncRead(Object asyncGroupObj,int[] itemHandle)
        {
            string errText = string.Empty;
            int nCancelid;
            IntPtr pErrors = IntPtr.Zero;
            if ((IOPCAsyncIO2)asyncGroupObj != null)
            {
                try
                {
                    ((IOPCAsyncIO2)asyncGroupObj).Read(itemHandle.Length, itemHandle, itemHandle.Length, out nCancelid, out pErrors);
                    int[] errors = new int[itemHandle.Length];
                    Marshal.Copy(pErrors, errors, 0, itemHandle.Length);
                }
                catch (Exception ex)
                {
                    errText = new StringBuilder().Clear().Append("异步读出错：").Append(ex.ToString()).ToString();
                }
            }
            return errText;
        }

        /// <summary>
        /// 设置异步更新状态，使之触发或关闭OnDataChange事件函数
        /// </summary>
        /// <param name="group"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetState(object asyncGroupObj,bool value)
        {
            IntPtr pRequestedUpdateRate = IntPtr.Zero;
            int nRevUpdateRate;
            IntPtr hClientGroup = IntPtr.Zero;
            IntPtr pTimeBias = IntPtr.Zero;
            IntPtr pDeadband = IntPtr.Zero;
            IntPtr pLCID = IntPtr.Zero;
            int nActive = 0;

            // activates or deactivates group according to value status  
            GCHandle hActive = GCHandle.Alloc(nActive, GCHandleType.Pinned);
            if (value != true)
                hActive.Target = 0;
            else
                hActive.Target = 1;
            try
            {
                ((IOPCGroupStateMgt)asyncGroupObj).SetState(pRequestedUpdateRate, out nRevUpdateRate, hActive.AddrOfPinnedObject(), pTimeBias, pDeadband, pLCID, hClientGroup);
            }
            finally
            {
                hActive.Free();
            }
        }

        /// <summary>
        /// 订阅数据改变回调函数
        /// </summary>
        public virtual void OnDataChange(Int32 dwTransid, Int32 hGroup, Int32 hrMasterquality, Int32 hrMastererror, Int32 dwCount,
             int[] phClientItems, object[] pvValues, short[] pwQualities, OpcRcw.Da.FILETIME[] pftTimeStamps, int[] pErrors)
        {
            if (_dgOnDataChange != null)
                _dgOnDataChange.BeginInvoke(phClientItems, pvValues, null, null);
        }
        /// <summary>
        /// 异步读完成回调函数
        /// </summary>
        public virtual void OnReadComplete(Int32 dwTransid, Int32 hGroup, Int32 hrMasterquality, Int32 hrMastererror,
            Int32 dwCount, int[] phClientItems, object[] pvValues, short[] pwQualities, OpcRcw.Da.FILETIME[] pftTimeStamps, int[] pErrors)
        { }
        /// <summary>
        /// 异步读写取消完成回调函数
        /// </summary>
        public virtual void OnCancelComplete(int dwTransid, int hGroup)
        { }
        /// <summary>
        /// 异步写完成回调函数
        /// </summary>
        public virtual void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles, int[] pErrors)
        { }

        #endregion

        #endregion
    }

    public struct SyncGroupOpcInfo
    {
        private int _clientHandle;
        /// <summary>
        /// 同步组句柄，创建组对象时产生，释放内存时用
        /// </summary>
        private int _syncGroupHandle;
        /// <summary>
        /// 同步组对象，创建组对象时产生，添加Item时用
        /// </summary>
        private Object _syncGroupObj;
        /// <summary>
        /// 同步读写接口对象，创建组对象时产生，执行同步读写时用
        /// </summary>
        private IOPCSyncIO2 _syncIOPCIO2Obj;
        public SyncGroupOpcInfo(int clientHandle, int syncGroupHandle, object syncGroupObj, IOPCSyncIO2 syncIOPCIO2Obj)
        {
            this._clientHandle = clientHandle;
            this._syncGroupHandle = syncGroupHandle;
            this._syncGroupObj = syncGroupObj;
            this._syncIOPCIO2Obj = syncIOPCIO2Obj;
        }

        public object SyncGroupObj { get => _syncGroupObj; }
    }

    public struct AsyncGroupOpcInfo
    {
        private int _clientHandle;
        /// <summary>
        /// 同步组句柄，创建组对象时产生，释放内存时用
        /// </summary>
        private int _asyncGroupHandle;
        /// <summary>
        /// 异步组对象，创建组对象时产生，添加Item时用
        /// </summary>
        private Object _asyncGroupObj;
        /// <summary>
        /// 异步读写接口对象，创建组对象时产生，执行异步读写时用
        /// </summary>
        private IOPCAsyncIO2 _asyncIOPCIO2Obj;
        /// <summary>
        /// 通信组状态接口对象，用于异步通信状态设置
        /// </summary>
        private IOPCGroupStateMgt2 _iGroupStateMgt2Obj;
        //private Int32 _dwCookie;
        //private IConnectionPointContainer pIConnectionPointContainer;
        //private IConnectionPoint pIConnectionPoint;

        public AsyncGroupOpcInfo(int clientHandle, int asyncGroupHandle, object asyncGroupObj, IOPCAsyncIO2 asyncIOPCIO2Obj, IOPCGroupStateMgt2 iGroupStateMgt2Obj)
        {
            this._clientHandle = clientHandle;
            this._asyncGroupHandle = asyncGroupHandle;
            this._asyncGroupObj = asyncGroupObj;
            this._asyncIOPCIO2Obj = asyncIOPCIO2Obj;
            this._iGroupStateMgt2Obj = iGroupStateMgt2Obj;
        }

        public object AsyncGroupObj { get => _asyncGroupObj; }
    }
}
