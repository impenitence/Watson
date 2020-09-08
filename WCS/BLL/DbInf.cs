using DataBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll
{
    public static class DbInf
    {
        private static DBHelper _wcsDbHelper = new DBHelper(ConfigurationManager.ConnectionStrings["WcsDatabase"].ProviderName, ConfigurationManager.ConnectionStrings["WcsDatabase"].ConnectionString.ToString());

        #region interface for wms

        #region ftp interface , batch import type , 2.1.1.1站点标签信息  2.1.1.2批次拣选数据  2.1.6 SKU Master  2.1.7 UPC Master

        /// <summary>
        /// select table template,batch import way 1 step 1
        /// </summary>
        /// <param name="errText"></param>
        /// <returns></returns>
        public static DataSet GetTemplate(string table,ref string errText)
        {
            DataSet ds = null;
            errText = string.Empty;
            string sqlStr = string.Empty;
            try
            {
                switch (table)
                {
                    case "STINFO":
                        sqlStr = "select t.station_id,t.tag_id,t.tag_attribute,t.batch_id,t.customer_id,t.send_time,t.interface_way from inf_tag t where 1=0";
                        break;
                    case "BOINFO":
                        sqlStr = "select t.send_time,t.batch_id,t.sku,t.order_qty,t.line_id,t.customer_id,t.wave_id,t.flag,t.import_time,t.interface_way from INF_ORDER t";
                        break;
                }
                ds = _wcsDbHelper.ExecuteDataset(CommandType.Text, sqlStr);
            }
            catch (Exception ex)
            {
                errText = ex.Message.ToString();
            }
            return ds;
        }

        /// <summary>
        /// batch import,way 1 step 3
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="errText"></param>
        /// <returns></returns>
        public static int BatchImportTag(DataTable dt,string sqlStr, ref string errText)
        {
            errText = string.Empty;
            int retVal = -1;

            using (IDbConnection conn = _wcsDbHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction trans = conn.BeginTransaction();
                try
                {
                    retVal = _wcsDbHelper.BatchImportTable(sqlStr, dt, trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    errText = ex.Message;
                    if (trans != null)
                        trans.Rollback();
                }
            }
            return retVal;
        }

        /// <summary>
        /// batch import,way 2
        /// </summary>
        public static int BatchImport(string path, bool hdr, string extractSqlStr, string sqlStr, ref string errText)
        {
            errText = string.Empty;
            int retVal = -1;

            using (IDbConnection conn = _wcsDbHelper.GetConnection())
            {
                conn.Open();
                IDbTransaction trans = conn.BeginTransaction();
                try
                {
                    DataSet ds = _wcsDbHelper.ExecuteDataset(CommandType.Text, sqlStr, conn);
                    DataTable dt = ds.Tables[0];

                    string strConn = "Provider=Microsoft.Jet.Oledb.4.0; Data Source=" + path + "; Extended Properties=\"Excel 8.0; HDR=" + (hdr ? "YES" : "NO") + "; IMEX=1;\"";
                    using (OleDbConnection oleDbcon = new OleDbConnection(strConn))
                    {
                        OleDbCommand oleCmd = new OleDbCommand(extractSqlStr, oleDbcon);
                        OleDbDataAdapter oleAdapter = new OleDbDataAdapter(oleCmd);

                        if (oleDbcon.State == ConnectionState.Closed)
                        {
                            oleDbcon.Open();
                        }

                        oleAdapter.AcceptChangesDuringFill = false;
                        oleAdapter.Fill(dt);
                    }

                    retVal = _wcsDbHelper.BatchImportTable(sqlStr, dt, trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    errText = ex.Message;
                    if (trans != null)
                        trans.Rollback();
                }
            }
            return retVal;
        }

        #endregion

        /// <summary>
        /// 2.1.2 站点（料箱）拣货完成
        /// </summary>
        public static void IPickOver()
        {

        }

        /// <summary>
        /// 2.1.3 站点迁移或合并（更新店铺位置）
        /// </summary>
        public static void IStationUpdate()
        {
            
        }

        /// <summary>
        /// 2.1.4 波次完成
        /// </summary>
        public static void IWaveOver()
        {

        }

        /// <summary>
        /// 2.1.5 站点释放
        /// </summary>
        public static void IStationRelease()
        {

        }

        /// <summary>
        /// 2.1.6 SKU Master
        /// </summary>
        public static void ISku()
        {

        }

        /// <summary>
        /// 2.1.7 UPC Master
        /// </summary>
        public static void IUpc()
        {

        }

        /// <summary>
        /// 2.1.8 PTL手工扫描料箱
        /// </summary>
        public static void IManualPick()
        {

        }

        /// <summary>
        /// 2.1.9 二拣
        /// </summary>
        public static void IWcsOutsidePick()
        {

        }

        /// <summary>
        /// 2.1.10 关闭位置门店
        /// </summary>
        public static void ICloseTagCustomer()
        {

        }

        /// <summary>
        /// 2.1.11 新增位置门店
        /// </summary>
        public static void INewTagCustomer()
        {

        }

        #endregion

        #region 系统登录

        /// <summary>
        /// 登陆用户校验
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pwd"></param>
        /// <param name="errText"></param>
        public static DataSet LogOnSystem(string userId, string pwd, ref string errText)
        {
            DataSet ds = null;
            errText = string.Empty;
            string sqlStr = string.Empty;
            try
            {
                sqlStr = "select t.name,p.privilege_id,s.func_id,s.description from TD_USER t " +
                    "left join td_user_privilege_dic p on p.object_type='USER' and p.object_id=t.id "+
                    "left join td_user_privilege s on s.id=p.privilege_id "+
                    "where t.id='"+ userId + "' and t.pwd='" + pwd + "' and t.status='00'";
                ds = _wcsDbHelper.ExecuteDataset(CommandType.Text, sqlStr);
            }
            catch (Exception ex)
            {
                errText = ex.Message.ToString();
            }
            return ds;
        }
        #endregion

        #region 获取opc db信息

        /// <summary>
        /// 获取opc db信息
        /// </summary>
        /// <param name="errText"></param>
        /// <returns></returns>
        public static DataSet GetOPCDbInfo(ref string errText)
        {
            DataSet ds = null;
            errText = string.Empty;
            string sqlStr = string.Empty;
            try
            {
                sqlStr = "select t.id,t.db_aim,t.db_str,t.type from TD_WCS_DEVICE_OPCINFO t";
                ds = _wcsDbHelper.ExecuteDataset(CommandType.Text, sqlStr);
            }
            catch (Exception ex)
            {
                errText = ex.Message.ToString();
            }
            return ds;
        }

        #endregion
    }
}
