using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCF
{
    public class Service : IService
    {
        #region for wms

        /// <summary>
        /// 2.1.2 站点（料箱）拣货完成
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string PickOver(string value)
        {
            
        }

        public string CloseTagCustomer(string value)
        {
            //string errText = string.Empty;
            //JObject jobj = new JObject();
            ////获取统计数据
            //DataSet dcencus = DbIntf.GetTabCencus(ref errText);
            //if (errText.Length > 0)
            //    return ErrMess("cencus" + errText);
            //string bthsum, ordsum, fshordsum, goodsum, fshgoodsum;
            //JArray jarray = new JArray();
            //foreach (DataRow row in dcencus.Tables[0].Rows)
            //{
            //    jobj = new JObject();
            //    bthsum = row["bthsum"].ToString();
            //    jobj.Add("bthsum", bthsum);
            //    ordsum = row["ordsum"].ToString();
            //    jobj.Add("ordsum", ordsum);
            //    fshordsum = row["fshordsum"].ToString();
            //    jobj.Add("fshordsum", fshordsum);
            //    goodsum = row["goodsum"].ToString();
            //    jobj.Add("goodsum", goodsum);
            //    fshgoodsum = row["fshgoodsum"].ToString();
            //    jobj.Add("fshgoodsum", fshgoodsum);
            //    jarray.Add(jobj);
            //}
            //jobj = new JObject();
            //jobj.Add("status", "1");
            //jobj.Add("cencus", jarray);
            //jobj.Add("err", "");
            //return jobj.ToString();
            return string.Empty;
        }

        public string ManualPick(string value)
        {
            throw new NotImplementedException();
        }

        public string NewTagCustomer(string value)
        {
            throw new NotImplementedException();
        }

        

        public string StationRelease(string value)
        {
            throw new NotImplementedException();
        }

        public string StationUpdate(string value)
        {
            throw new NotImplementedException();
        }

        public string WaveOver(string value)
        {
            throw new NotImplementedException();
        }

        public string WcsOutsidePick(string value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region for pda

        public string LogIn(string value)
        {
            throw new NotImplementedException();
        }

        public string WaveLegal(string value)
        {
            throw new NotImplementedException();
        }

        public string ContainerLegal(string value)
        {
            throw new NotImplementedException();
        }

        public string InputSku(string value)
        {
            throw new NotImplementedException();
        }

        public string InputQty(string value)
        {
            throw new NotImplementedException();
        }

        public string FeedOver(string value)
        {
            throw new NotImplementedException();
        }

        public string QueryContainer(string value)
        {
            throw new NotImplementedException();
        }
        public string ManualPushPtl(string value)
        {
            throw new NotImplementedException();
        }
        public string ReleaseContainer(string value)
        {
            throw new NotImplementedException();
        }
        #endregion

        public string PackResponseMessage(string id, string status,string info)
        {
            JObject jobj = new JObject();
            jobj.Add(" transfer_id", id);
            jobj.Add("status", status);
            jobj.Add("err", info);
            return jobj.ToString();
        }
    }
}
