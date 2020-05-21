using System;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FetchDataFromDB
{
    public enum DBType
    {
        SqlServer = 0,
        Oracle = 1
    }

    public class DataAccess
    {
        private IDbConnection _dbConnection;
        private IDbCommand _dbCommand;
        private IDbDataAdapter _dbDataAdapter;

        private void AddConnection(DBType dbType, string connectionString)
        {
            switch (dbType)
            {
                case DBType.SqlServer:
                    _dbConnection = new SqlConnection(connectionString);
                    _dbCommand = new SqlCommand();
                    _dbDataAdapter = new SqlDataAdapter(_dbCommand as SqlCommand);
                    break;
                case DBType.Oracle:
                    _dbConnection = new OracleConnection(connectionString);
                    _dbCommand = new OracleCommand();
                    _dbDataAdapter = new OracleDataAdapter(_dbCommand as OracleCommand);
                    break;
            }

            _dbCommand.Connection = _dbConnection;

        }


        public DataSet FetchDataFromTable(DBType dbType, string connectionString, string query)
        {
            try
            {
                AddConnection(dbType, connectionString);

                _dbCommand.CommandText = query;

                using (var ds = new DataSet())
                {
                    _dbDataAdapter.Fill(ds);
                    return ds;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

    }
}
