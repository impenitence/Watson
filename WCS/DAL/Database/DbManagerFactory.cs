using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public enum DataProvider
    {
        Oracle,
        SqlServer,
        OleDb,
        Odbc,
        MySql
    }
    public class DbManagerFactory
    {

        #region Methods

        /// <summary>
        /// 新建连接
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbConnection GetConnection(DataProvider dataProvider)
        {
            IDbConnection iDbConnection;
            switch (dataProvider)
            {
                case DataProvider.SqlServer:
                    iDbConnection = new SqlConnection();
                    break;
                case DataProvider.OleDb:
                    iDbConnection = new OleDbConnection();
                    break;
                case DataProvider.Odbc:
                    iDbConnection = new OdbcConnection();
                    break;
                case DataProvider.Oracle:
                    iDbConnection = new OracleConnection();
                    break;
                //case DataProvider.MySql:
                //    iDbConnection = new MySqlConnection();
                    break;
                default:
                    return null;
            }
            return iDbConnection;
        }

        /// <summary>
        /// 新建连接
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbConnection GetConnection(DataProvider dataProvider,string connectString)
        {
            IDbConnection iDbConnection;
            switch (dataProvider)
            {
                case DataProvider.SqlServer:
                    iDbConnection = new SqlConnection(connectString);
                    break;
                case DataProvider.OleDb:
                    iDbConnection = new OleDbConnection(connectString);
                    break;
                case DataProvider.Odbc:
                    iDbConnection = new OdbcConnection(connectString);
                    break;
                case DataProvider.Oracle:
                    iDbConnection = new OracleConnection(connectString);
                    break;
                    //case DataProvider.MySql:
                    //    iDbConnection = new MySqlConnection(ConnectString);
                    break;
                default:
                    return null;
            }
            return iDbConnection;
        }

        /// <summary>
        /// 新建命令
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbCommand GetCommand(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlCommand();
                case DataProvider.OleDb:
                    return new OleDbCommand();
                case DataProvider.Odbc:
                    return new OdbcCommand();
                case DataProvider.Oracle:
                    return new OracleCommand();
                //case DataProvider.MySql:
                //    return new MySqlCommand();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 新建适配器
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns></returns>
        public static IDbDataAdapter GetDataAdapter(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlDataAdapter();
                case DataProvider.OleDb:
                    return new OleDbDataAdapter();
                case DataProvider.Odbc:
                    return new OdbcDataAdapter();
                case DataProvider.Oracle:
                    return new OracleDataAdapter();
                //case DataProvider.MySql:
                //    return new MySqlDataAdapter();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 新建参数数组
        /// </summary>
        /// <param name="providerType"></param>
        /// <param name="paramsCount"></param>
        /// <returns></returns>
        public static IDbDataParameter[] GetParameters(DataProvider providerType, int paramsCount)
        {
            IDbDataParameter[] idbParams = new IDbDataParameter[paramsCount];
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new SqlParameter();
                    }
                    break;
                case DataProvider.OleDb:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new OleDbParameter();
                    }
                    break;
                case DataProvider.Odbc:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new OdbcParameter();
                    }
                    break;
                case DataProvider.Oracle:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new OracleParameter();
                    }
                    break;
                //case DataProvider.MySql:
                //    for (int i = 0; i < paramsCount; i++)
                //    {
                //        idbParams[i] = new MySqlParameter();
                //    }
                //    break;
                default:
                    idbParams = null;
                    break;
            }
            return idbParams;
        }

        public static DbCommandBuilder GetCommandBuilder(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlCommandBuilder();
                case DataProvider.OleDb:
                    return new OleDbCommandBuilder();
                case DataProvider.Odbc:
                    return new OdbcCommandBuilder();
                case DataProvider.Oracle:
                    return new OracleCommandBuilder();
                //case DataProvider.MySql:
                //    return new MySqlCommand();
                default:
                    return null;
            }
        }
        #endregion
    }
}
