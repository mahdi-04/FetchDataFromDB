using System;
using System.Configuration;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FetchDataFromDB
{
    public partial class Form1 : Form
    {
        private DataAccess dataAccess = new DataAccess();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dbtype = radioButton1.Checked ? DBType.SqlServer : DBType.Oracle;
            var connectionString = dbtype == DBType.SqlServer ? ConfigurationManager.AppSettings["SQLConnectionString"] : ConfigurationManager.AppSettings["OracleConnectionString"];
            var query = txtQuery.Text;
            var table = dataAccess.FetchDataFromTable(dbtype, connectionString, query);
            if (table != null)
                dataGridView2.DataSource = table.Tables[0];
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sqlConstr = ConfigurationManager.AppSettings["SQLConnectionString"];
            var oraConstr = ConfigurationManager.AppSettings["OracleConnectionString"];
            var sqlColumn = txtSqlFieldName.Text;
            var oraColumn = txtOraFieldName.Text;
            var sqlTableName = txtSqlTableName.Text;
            var oraTableName = txtOraTableName.Text;
            var sqlPkey = txtSqlPrimaryKey.Text;
            var oraPkey = txtOraPrimaryKey.Text;
            var sqlQuery = txtQuery.Text;

            Run(sqlConstr, sqlQuery, sqlColumn, sqlPkey, oraConstr, oraTableName, oraColumn, oraPkey);
        }


        public void Run(string sqlConnstr, string sqlQuery, string sqlColumn, string sqlPKey, string oracleConnstr, string oracleTable, string oracleColumn, string oraclePKey)
        {
            try
            {
                using (var oraConnection = new OracleConnection(oracleConnstr))
                {
                    var sqlConnection = new SqlConnection(sqlConnstr);
                    var oraCommand = new OracleCommand("", oraConnection);
                    var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    var rowUpdated = 1;
                    sqlConnection.Open();
                    var dr = sqlCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        try
                        {
                            var sqlFieldData = dr[sqlColumn];
                            var pkey = dr[sqlPKey];
                            var oraquery = string.Format("Update {0} Set {1} = :param1 Where {2} = :param2", oracleTable, oracleColumn, oraclePKey);
                            oraCommand.Parameters.Add("param1", OracleType.Blob).Value = sqlFieldData;
                            oraCommand.Parameters.Add("param2", OracleType.Number).Value = pkey;
                            oraCommand.CommandText = oraquery;
                            oraConnection.Open();
                            oraCommand.ExecuteNonQuery();
                            oraConnection.Close();
                            Text = string.Format("{0} row(s) is updated!", rowUpdated);
                            rowUpdated++;
                            Application.DoEvents();
                        }
                        catch (Exception e)
                        {
                            oraConnection.Close();
                            MessageBox.Show(e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // ignored
                MessageBox.Show(e.Message);
            }
        }

    }
}
