using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WCS
{
    public class FileHelper
    {
        #region properties

        /// <summary>
        /// Encoding
        /// </summary>
        static Encoding _encoding = Encoding.Default;

        #endregion

        #region Base

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <returns></returns>
        public static bool FileExist(string dirPath, string fileName)
        {
            string filePath = dirPath + "\\" + fileName;
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            return File.Exists(filePath);
        }

        #endregion

        #region xlsx or csv file

        /// <summary>
        /// import excel to datatable ,source data 
        /// </summary>
        /// <param name="ExcelFilePath"></param>
        /// <param name="TableName"></param>
        /// <param name="hdr"></param>
        /// <param name="errText"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        public static void InputFromExcel(string ExcelFilePath, bool hdr, string extractStr, ref DataTable dt, ref string errText)
        {
            errText = string.Empty;
            if (!File.Exists(ExcelFilePath))
            {
                errText = "Excel文件不存在！";
            }

            //如果数据表名不存在，则数据表名为Excel文件的第一个数据表
            //ArrayList TableList = new ArrayList();
            //TableList = GetExcelTables(ExcelFilePath);

            //if (TableList.IndexOf(tableName) < 0)
            //{
            //    tableName = TableList[0].ToString().Trim();
            //}


            string connStr = "Provider=Microsoft.Jet.Oledb.4.0; Data Source=" + ExcelFilePath + "; Extended Properties=\"Excel 8.0; HDR=" + (hdr ? "YES" : "NO") + "; IMEX=1;\"";
            OleDbConnection dbcon = new OleDbConnection(connStr);
            OleDbCommand cmd = new OleDbCommand(extractStr, dbcon);
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            //hold datarow state,important
            adapter.AcceptChangesDuringFill = false;

            try
            {
                if (dbcon.State == ConnectionState.Closed)
                {
                    dbcon.Open();
                    adapter.Fill(dt);
                }               
            }
            catch (Exception ex)
            {
                errText = ex.Message;
            }
            finally
            {
                if (dbcon.State == ConnectionState.Open)
                {
                    dbcon.Close();
                }
            }
        }

        /// <summary>
        /// get Excel or csv table list
        /// </summary>
        private static ArrayList GetExcelTables(string ExcelFileName)
        {
            DataTable dt = new DataTable();
            ArrayList TablesList = new ArrayList();
            if (File.Exists(ExcelFileName))
            {
                string strConn = "Provider=Microsoft.Jet.Oledb.4.0; Data Source=" + ExcelFileName + "; Extended Properties=\"Excel 8.0; HDR=YES; IMEX=1;\"";

                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    try
                    {
                        conn.Open();
                        dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    //获取数据表个数
                    int tablecount = dt.Rows.Count;
                    for (int i = 0; i < tablecount; i++)
                    {
                        string tablename = dt.Rows[i][2].ToString().Trim().TrimEnd('$');
                        if (TablesList.IndexOf(tablename) < 0)
                        {
                            TablesList.Add(tablename);
                        }
                    }
                }
            }
            return TablesList;
        }

        public static void ConvertExcel(string openPath, string savePath,ref string errText)
        {
            errText = string.Empty;
            try
            {
                //将xml文件转换为标准的Excel格式 
                Object Nothing = Missing.Value;//由于yongCOM组件很多值需要用Missing.Value代替   
                Excel.Application ExclApp = new Excel.ApplicationClass();// 初始化
                Excel.Workbook ExclDoc = ExclApp.Workbooks.Open(openPath, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing);//打开Excl工作薄   
                try
                {
                    Object format = Excel.XlFileFormat.xlWorkbookNormal;//获取Excl 2007文件格式   
                    ExclApp.DisplayAlerts = false;
                    ExclDoc.SaveAs(savePath, format, Nothing, Nothing, Nothing, Nothing, Excel.XlSaveAsAccessMode.xlExclusive, Nothing, Nothing, Nothing, Nothing, Nothing);//保存为Excl 2007格式   
                }
                catch (Exception ex)
                {
                    errText = ex.Message;
                }
                finally
                {
                    ExclDoc.Close(Nothing, Nothing, Nothing);
                    ExclApp.Quit();
                }
            }
            catch (Exception ex)
            {
                errText = ex.Message;
            }
        }

        #endregion

    }
}
