using Bll;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WCS
{
    public class FtpFileListener
    {
        #region Fields
        /// <summary>
        /// the path of the directory to listen
        /// </summary>
        string _path = string.Empty;
        /// <summary>
        /// listen thread
        /// </summary>
        Thread _thread;
        /// <summary>
        /// action of dealing file
        /// </summary>
        Action<string> _action;
        /// <summary>
        /// thread wait or set
        /// </summary>
        public static ManualResetEvent _exeTaskMre = new ManualResetEvent(false);
        #endregion

        public FtpFileListener(string path)
        {
            _action += DealInfFile;
            _path = path;
            _thread = new Thread(new ThreadStart(AutoWork));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void AutoWork()
        {
            while (true)
            {
                //_exeTaskMre.WaitOne();
                try
                {
                    if (Directory.Exists(_path))
                    {
                        string tmpPath=string.Empty;

                        string[] files = Directory.GetFiles(_path, "*.csv", SearchOption.TopDirectoryOnly);
                        foreach (string path in files)
                        {
                            if (Regex.IsMatch(path, @"^.*\\(?:STINFO|BOINFO|SKUINSERT|SKUDELETE|UPCINSERT|UPCDELETE)\d{17}\.csv$"))
                            {
                                //rename
                                tmpPath = path.Replace(".csv", "loading.csv");
                                File.Move(path, tmpPath);
                                //deal
                                _action.BeginInvoke(tmpPath, null, null);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //log待修改
                }
                finally
                {
                    Thread.Sleep(2579);
                }
            }
        }

        private void DealInfFile(string fullPath)
        {
            string errText = string.Empty;
            string tmpFullPath = fullPath.Replace(".csv", "tmp.csv");
            string sourceFileName = Path.GetFileName(fullPath).Replace("loading", "");
            string tableName = Path.GetFileName(fullPath).Replace(".csv", "").Substring(0,31);
            string import_s_time = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            try
            {
                //转化临时excel
                FileHelper.ConvertExcel(fullPath, tmpFullPath, ref errText);
                if (errText.Length <= 0)
                {
                    string extractSqlStr = string.Empty; ;
                    string sqlStr=string.Empty;
                    //2.1.1.1 站点标签信息
                    if (fullPath.Contains("STINFO"))
                    {
                        extractSqlStr = @"select F1 as send_time,F2 as batch_id,F3 as tag_id,F4 as tag_attribute,F5 as station_id,F6 as customer_id,'" + sourceFileName + "' as interface_way,'" + import_s_time + "' as import_s_time from [" + tableName + "$]";
                        sqlStr = "select t.station_id,t.tag_id,t.tag_attribute,t.batch_id,t.customer_id,t.send_time,t.interface_way,t.import_s_time from inf_tag t where 1=0";
                    }
                    else if (fullPath.Contains("BOINFO"))   //2.1.1.2 批次拣选数据
                    {
                        extractSqlStr = @"select F1 as send_time,F2 as batch_id,F3 as sku,F4 as order_qty,F5 as line_id,F6 as customer_id,F7 as wave_id,F8 as flag,'" + sourceFileName + "' as interface_way,'" + import_s_time + "' as import_s_time from [" + tableName + "$]";
                        sqlStr = "select t.batch_id,t.wave_id,t.customer_id,t.sku,t.order_qty,t.flag,t.send_time,t.interface_way,t.import_s_time,t.line_id from inf_order t where 1=0";
                    }
                    else if (fullPath.Contains("SKU"))   //2.1.6 SKU Master
                    {
                        string flag = (fullPath.Contains("DELETE")) ? "OFF" : "ON";
                        extractSqlStr = @"select F1 as sku,F2 as sku_name,F3 as sku_description,F4 as sku_attribute,F5 as weight,F6 as length,F7 as width,F8 as height,F9 as pack_unit_size,'" + flag + "' as flag,'" + sourceFileName + "' as interface_way,'" + import_s_time + "' as import_s_time from [" + tableName + "$]";
                        sqlStr = "select t.sku,t.sku_name,t.sku_description,t.sku_attribute,t.weight,t.length,t.width,t.height,t.pack_unit_size,t.flag,t.interface_way,t.import_s_time from inf_sku t where 1=0";

                    }
                    else if (fullPath.Contains("UPC"))   //2.1.7 UPC Master
                    {
                        string flag = (fullPath.Contains("DELETE")) ? "OFF" : "ON";
                        extractSqlStr = @"select F1 as sku,F2 as upc,'" + flag + "' as flag,'" + sourceFileName + "' as interface_way,'" + import_s_time + "' as import_s_time from [" + tableName + "$]";
                        sqlStr = "select t.upc,t.sku,t.flag,t.interface_way,t.import_s_time from inf_upc t where 1=0";
                    }
                    else
                        errText = "interface file name does not include agreed string";

                    if (sqlStr.Length > 0 && extractSqlStr.Length > 0)
                        DbInf.BatchImport(tmpFullPath, false, extractSqlStr, sqlStr, ref errText);

                    #region batch import other implement
                    //DataTable dt = DbInf.GetTemplate("STINFO", ref errText).Tables[0];
                    //if (errText.Length > 0)
                    //{
                    //    return;
                    //}

                    //FileHelper.InputFromExcel(tmpFullPath, false, extractSqlStr, ref dt, ref errText);
                    //if (errText.Length > 0)
                    //{
                    //    return;
                    //}

                    //DbInf.BatchImportTag(dt, sqlStr, ref errText);
                    //if (errText.Length > 0)
                    //{
                    //    return;
                    //}
                    #endregion
                }
            }
            catch (Exception ex)
            {
                errText = ex.Message;
            }
            finally
            {
                string resultFileName = string.Empty;
                if (errText.Length > 0)
                    resultFileName = fullPath.Replace("loading", "fail");
                else
                    resultFileName = fullPath.Replace("loading", "success");
                File.Move(fullPath, resultFileName);
                if (File.Exists(tmpFullPath))
                    File.Delete(tmpFullPath);
            }
        }


    }
}
