using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.Common;
using System.Configuration;
using Logger;
using System.Reflection;

namespace DAL
{
    public class DABase
    {
        DatabaseProviderFactory factory = new DatabaseProviderFactory();
        static Database db;// = factory.Create("ConnectionString");

        //static Database db = DatabaseFactory.CreateDatabase();
        public static string connectionString;
        public static SqlConnection connection;
        public DABase()
        {
            db = factory.Create("SMS_WEB");
            connectionString = ConfigurationManager.ConnectionStrings["SMS_WEB"].ConnectionString;
            connection = new SqlConnection(connectionString);
        }
        public void OpenDBConnection()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        public void CloseDBConnection()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        protected int ExecuteNonQuery(string queryString)
        {
            try
            {
                return db.ExecuteNonQuery(db.GetSqlStringCommand(queryString));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected object ExecuteScalar(string queryString)
        {
            try
            {
                return db.ExecuteScalar(db.GetSqlStringCommand(queryString));
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        protected DataSet ExecuteDataSet(string queryString)
        {
            DataSet ds = new DataSet();
            try
            {
                LogWriter.WriteLog("Executing Sql Query : " + queryString);
                ds = db.ExecuteDataSet(db.GetSqlStringCommand(queryString));
            }
            catch (Exception ex)
            {
                LogWriter.WriteProcErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                LogWriter.WriteExceptionLog(ex);
            }
            return ds;
        }
        public DataSet ExecuteStoredProcedure(string procedureName, List<StoredProcedureParams> parametersList)
        {
            DbCommand cmd = db.GetStoredProcCommand(procedureName);
            cmd.CommandType = CommandType.StoredProcedure;
            foreach (var item in parametersList)
            {

                if (item.ParamType.Equals(DbType.DateTime) && (item.ParamValue == null || item.ParamValue.Length <= 0))
                {
                    item.ParamValue = DBNull.Value.ToString();
                }

                db.AddInParameter(cmd, item.ParamName, (DbType)item.ParamType, item.ParamValue);
            }

            DataSet ds = db.ExecuteDataSet(cmd);
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                if (ds.Tables[0].Columns.Contains("ERROR_MESSAGE"))
                {
                    throw new Exception("Error Occured While executing the stored procedure at break point:" + ds.Tables[0].Rows[0]["ERROR_MESSAGE"].ToString());
                }
            }
            return ds;
        }

    }
}
