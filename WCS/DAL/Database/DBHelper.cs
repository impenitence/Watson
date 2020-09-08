using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace DataBase
{
    public class DBHelper : IDBHelper
    {
        #region Fields

        private DataProvider _dataProvider;

        private string _connectString;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public DBHelper(string providerType, string connectionString)
        {
            try
            {
                _dataProvider = (DataProvider)Enum.Parse(typeof(DataProvider), providerType);
                _connectString = connectionString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private

        private void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDbDataParameter[] commandParameters, out bool mustCloseConnection)
        {
            // Create a command and prepare it for execution
            if (command == null) throw new ArgumentNullException("command null");

            if (connection == null) throw new ArgumentNullException("connection null");
            
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText null or empty");

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("请提供一个已经打开的transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        /// <summary>
        /// 将OracleParameters数组参数附加到OracleCommand中，对于值为null的输入和输入输出类型的参数，将指定为DbNull值
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of OracleParameters to be added to command</param>
        private static void AttachParameters(IDbCommand command, IDbDataParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (IDbDataParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        #endregion

        #region Interface Implementation

        public IDbConnection GetConnection()
        {
            IDbConnection conn = DbManagerFactory.GetConnection(_dataProvider);
            conn.ConnectionString = _connectString;
            return conn;
        }

        /// <summary>
        /// 连接执行参数语句
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            // Create a command and prepare it for execution
            IDbCommand cmd = DbManagerFactory.GetCommand(_dataProvider);
            IDbConnection conn = GetConnection();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, conn, null, commandType, commandText, commandParameters,out mustCloseConnection);

            // Finally, execute the command
            int retVal = cmd.ExecuteNonQuery();

            // Detach the OracleParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if(mustCloseConnection)
                conn.Close();
            return retVal;
        }

        /// <summary>
        /// 连接执行无参语句
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(commandType, commandText, null);
        }

        /// <summary>
        /// 连接执行参数语句返回DataSet
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText, IDbConnection conn = null,params IDbDataParameter[] commandParameters)
        {
            // Create a command and prepare it for execution
            IDbCommand cmd = DbManagerFactory.GetCommand(_dataProvider);
            if(conn==null)
                conn = GetConnection();
            bool mustCloseConnection = false;

            PrepareCommand(cmd, conn, null, commandType, commandText, commandParameters,out mustCloseConnection);

            // Create the DataAdapter & DataSet
            IDbDataAdapter da = DbManagerFactory.GetDataAdapter(_dataProvider);
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();

            // Fill the DataSet using default values for DataTable names, etc
            //da.SelectCommand.CommandTimeout = 0;
            da.Fill(ds);

            // Detach the OracleParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            if (mustCloseConnection)
                conn.Close();

            // Return the dataset
            return ds;
            
        }

        /// <summary>
        /// 连接执行无参语句返回DataSet
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteDataset(commandType, commandText, null);
        }

        /// <summary>
        /// 事务执行参数语句
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(IDbTransaction transaction, CommandType commandType, string commandText, params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            IDbCommand command = DbManagerFactory.GetCommand(_dataProvider);
            bool mustCloseConnection = false;

            PrepareCommand(command, transaction.Connection, transaction, commandType, commandText, commandParameters,out mustCloseConnection);
            int retVal = command.ExecuteNonQuery();

            command.Parameters.Clear();

            return retVal;
        }

        /// <summary>
        /// 事务执行无参语句
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(IDbTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteNonQuery(transaction, commandType, commandText, null);
        }

        /// <summary>
        /// 事务执行有参语句返回DataSet
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(IDbTransaction transaction, CommandType commandType, string commandText,params IDbDataParameter[] commandParameters)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            // Create a command and prepare it for execution
            IDbCommand cmd = DbManagerFactory.GetCommand(_dataProvider);
            bool mustCloseConnection = false;

            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters,out mustCloseConnection);

            // Create the DataAdapter & DataSet
            IDbDataAdapter da = DbManagerFactory.GetDataAdapter(_dataProvider);
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();

            // Fill the DataSet using default values for DataTable names, etc
            //da.SelectCommand.CommandTimeout = 0;
            da.Fill(ds);

            // Detach the OracleParameters from the command object, so they can be used again
            cmd.Parameters.Clear();

            // Return the dataset
            return ds;
        }

        /// <summary>
        /// 事务执行无参语句返回DataSet
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="commandType"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(IDbTransaction transaction, CommandType commandType, string commandText)
        {
            // Pass through the call providing null for the set of OracleParameters
            return ExecuteDataset(transaction, commandType, commandText, null);
        }

        #endregion

        #region Advanced Function

        /// <summary>
        /// batch import table data,short connect or trans
        /// </summary>
        /// <param name="sqlStr"></param>
        /// <param name="dt"></param>
        /// <param name="errText"></param>
        /// <param name="trans"></param>
        /// <returns>-1 异常 0 未影响数据 >0 影响数据行数</returns>
        public int BatchImportTable(string sqlStr, DataTable dt, IDbTransaction trans = null)
        {
            int retVal = -1;
            bool mustCloseConnection = false;

            IDbConnection conn;
            if (trans == null)
            {
                conn = GetConnection();
                mustCloseConnection = true;
            }
            else if (trans.Connection == null)
            {
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            else
            {
                conn = trans.Connection;
            }

            if (conn.State != ConnectionState.Open)
                conn.Open();

            IDbCommand cmd = DbManagerFactory.GetCommand(_dataProvider);
            cmd.CommandText = sqlStr;
            cmd.Connection = conn;

            IDbDataAdapter da = DbManagerFactory.GetDataAdapter(_dataProvider);
            da.SelectCommand = cmd;

            DbCommandBuilder cmdBuilder = DbManagerFactory.GetCommandBuilder(_dataProvider);
            cmdBuilder.DataAdapter = (DbDataAdapter)da;

            retVal = ((DbDataAdapter)da).Update(dt);

            if (mustCloseConnection)
                conn.Close();
            return retVal;
        }

        #endregion

        #endregion
    }
}
