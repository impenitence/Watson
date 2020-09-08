using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public class DeviceBase
    {
        //static int _handle = 0;
        //static object _handleObjLock = new object();
        static Dictionary<string, DeviceBase> _deviceDic = new Dictionary<string, DeviceBase>();

        string _id;

        string _name;

        string _areaId;

        public string Id { get => _id;}
        public string Name { get => _name;}
        public string AreaId { get => _areaId; }
        public static Dictionary<string, DeviceBase> DeviceDic { get => _deviceDic; set => _deviceDic = value; }

        //public DeviceBase()
        //{
        //    int tmpHandle;
        //    lock (_handleObjLock)
        //    {
        //        tmpHandle = _handle + 1;
        //    }
        //    DeviceHandleDic.Add(tmpHandle, this);
        //}

        public DeviceBase()
        {
            DeviceDic.Add(Id, this);
        }
    }
}
