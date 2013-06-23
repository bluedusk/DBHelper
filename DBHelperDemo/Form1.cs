using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DBH.Helper;
using System.Xml;
using DBH.Config;
using DBH.DBException;
using DBH;


namespace DBHelperDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Bind2();
            //DBHelperTest.Init("DBHelper1.config");
        }

        private void Bind2()//静态调用，读取app.config
        {

            string sql = "select * from ABST_User ";
            DataTable dt = DBHelper.ExecuteQuery(sql);
            dataGridView1.DataSource = dt;

        }
        private void Bind()//静态调用，读取app.config
        {

            string sql = "select * from ABST_User";
            DataTable dt = DBHelper.ExecuteQuery(sql);
            dataGridView1.DataSource = dt;

        }

        private void Bind1()//实例化调用
        {
            string connStr = "Data Source='10.232.27.214';Port='4200';UID='abstdbo';PWD='abstdbop';Database='ABST' ;Connect Timeout=10;Connection Lifetime=300;Pooling=true;Min pool size=1 ;Max pool size=500;";

            DBHelper.SetDBConfig("Sybase", connStr, "ISO-1");
            //DBHelper dh = DBHelperFactory.CreateDBHelper();
            string sql = "select * from ABST_User";
            DataTable dt = DBHelper.ExecuteQuery(sql);

            dataGridView1.DataSource = dt;

        }


        public void Save()
        {

            DBHelperParmCollection param = new DBHelperParmCollection();

            param.Add(new DBHelperParm("@UserName", "UserName"));
            param.Add(new DBHelperParm("@Password", "Password"));


            StringBuilder sbSql = new StringBuilder("");
            try
            {
                sbSql.Append("INSERT INTO  AMP_TEST  (UserName,Password) ");
                sbSql.Append("VALUES (?,?)");

                DBHelper.ExecuteNoQuery(sbSql.ToString(), param);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "DBHelper.config");

            }
            catch
            {
                throw new DBFileNotFoundException();
            }

            System.Collections.Generic.Dictionary<string, DataSourceConfig> dsTable = new Dictionary<string, DataSourceConfig>();
            XmlNodeList xnl = xmlDoc.DocumentElement.SelectNodes("DataSource");
            try
            {
                foreach (XmlNode xl in xnl)
                {
                    DataSourceConfig dsConfig = new DataSourceConfig();
                    string dsName = xl.Attributes["name"].Value;
                    string dialect = xl.Attributes["dialect"].Value;
                    dsConfig.dataSourceName = dsName;
                    dsConfig.dialectClass = dialect;
                    XmlNodeList parmNodeList = xl.SelectNodes("parm");
                    int i = 0;
                    object[] parmList = new object[parmNodeList.Count];
                    foreach (XmlNode parmNode in parmNodeList)
                    {
                        //参数个数要与构造函数对应 否则要进行转换
                        string id = parmNode.Attributes["id"].Value;
                        string value = parmNode.Attributes["value"].Value;
                        parmList[i] = value;
                        i++;
                    }
                    dsConfig.Parameters = parmList;


                    dsTable.Add(dsName, dsConfig);


                }
            }
            catch (Exception ex)
            {
                
                throw new DBConfigException(ex);
            }


        }


        private void button2_Click(object sender, EventArgs e)
        {
            Bind2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            DataTable dt = DBHelper.ExecuteQuery("select * from ABST_Role");
            dataGridView1.DataSource = dt;
        }


        /// <summary>
        /// 直接调用，访问default数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {

            //DataTable dt = DBHelper.ExecuteQuery("select * from ABST_User");
            //dataGridView1.DataSource = dt;


            IDBHelper dbhelper = DBHelperManager.GetHelper();
            dataGridView1.DataSource = dbhelper.ExecuteQuery("select * from ABST_User");


        }

        //使用配置节点初始化
        private void button5_Click(object sender, EventArgs e)
        {

            XmlDocument xmldoc = new XmlDocument();
            

            //DBHelperTest.Init("DBHelper1.config");
            DataTable dt = DBHelper.ExecuteQuery("select * from ABST_Terminal");
            dataGridView1.DataSource = dt;
        }

        //app.config初始化
        private void button6_Click(object sender, EventArgs e)
        {
            IDBHelper dbhelper = DBHelperManager.GetHelper("mysql");
            try
            {
                dataGridView1.DataSource = dbhelper.ExecuteQuery("select * from wordlist_1");

//                string sql = @"CREATE TABLE wordlist_1 (             
//              id INT(11) NOT NULL AUTO_INCREMENT,  
//              word VARCHAR(50) DEFAULT NULL,       
//              word_cn VARCHAR(50) DEFAULT NULL,    
//              courseid TINYINT(4) DEFAULT NULL,    
//              wordtype TINYTEXT,                   
//              PRIMARY KEY (id))";

//                dbhelper.ExecuteNoQuery(sql);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                dbhelper.Close();
            }

        }

        //指定配置文件初始化
        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = DBHelper.ExecuteQuery("select * from ABST_User");
        }


    }

}
