using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LROSE_DAL;
using System.Configuration;
using System.Data.SQLite;

namespace LROSE_Main.DbManagement
{
    public delegate void TransfDelegate();
    public partial class dbCreate : Form
    {
        private Label lblDbName;
        private TextBox txtDbName;
        private Button btnNew;
        private Button btnCancal;

        public dbCreate()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(dbCreate));
            this.lblDbName = new System.Windows.Forms.Label();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnCancal = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDbName
            // 
            this.lblDbName.AutoSize = true;
            this.lblDbName.Location = new System.Drawing.Point(78, 126);
            this.lblDbName.Name = "lblDbName";
            this.lblDbName.Size = new System.Drawing.Size(89, 12);
            this.lblDbName.TabIndex = 3;
            this.lblDbName.Text = "新建数据库名称";
            // 
            // txtDbName
            // 
            this.txtDbName.Location = new System.Drawing.Point(213, 126);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(196, 21);
            this.txtDbName.TabIndex = 6;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(213, 200);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 33);
            this.btnNew.TabIndex = 11;
            this.btnNew.Text = "新增";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnCancal
            // 
            this.btnCancal.Location = new System.Drawing.Point(334, 200);
            this.btnCancal.Name = "btnCancal";
            this.btnCancal.Size = new System.Drawing.Size(75, 33);
            this.btnCancal.TabIndex = 12;
            this.btnCancal.Text = "取消";
            this.btnCancal.UseVisualStyleBackColor = true;
            this.btnCancal.Click += new System.EventHandler(this.btnCancal_Click);
            // 
            // dbCreate
            // 
            this.ClientSize = new System.Drawing.Size(595, 380);
            this.Controls.Add(this.btnCancal);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.txtDbName);
            this.Controls.Add(this.lblDbName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "dbCreate";
            this.Text = "创建数据库";
            this.Load += new System.EventHandler(this.dbCreate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public static string newDbName = "";
        private void btnCancal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public event TransfDelegate TransfEvent;
        private void btnNew_Click(object sender, EventArgs e)
        {
            string enString = ConfigurationManager.ConnectionStrings["MyStrConn"].ConnectionString.ToString();
            if (enString.Contains("Pooling"))
            {
                CreateSqliteDb();  
            }
            else
            {
                CreateSqlServerDb();
            }
 
        }

        //创建sqlite数据库
        private void CreateSqliteDb()
        {
            string dbName = txtDbName.Text;
            string dbPath =LROSRDbContext.GetEFConnctionString(txtDbName.Text);
            using (SQLiteConnection cn = new SQLiteConnection(dbPath))
            {
                cn.Open();
                MessageBox.Show(string.Format("数据库{0}创建成功", dbName));
            }
            TransfEvent();
            this.Close();
        }

        //创建sql server数据库
        private void CreateSqlServerDb() 
        {
            string dbName = txtDbName.Text;
            try
            {
                //string connStr = String.Format("Data source={0};Integrated Security=True", serverName);
                string connStr = GetdbbConnctionString();
                string sql = String.Format("create database {0}", dbName);
                ExecuteSql(connStr, "Master", sql);//调用ExecuteNonQuery()来创建数据库  
                newDbName = dbName;
                DialogResult d = MessageBox.Show(string.Format("数据库{0}创建成功", dbName));
            }
            catch (Exception ex)
            {
                //弹出提示窗口
                DialogResult a = MessageBox.Show(ex.Message);
            }
            finally
            {
                System.Diagnostics.Process sqlProcess = new System.Diagnostics.Process();//创建一个进程  

                sqlProcess.StartInfo.FileName = "osql.exe";//OSQL基于ODBC驱动连接服务器的一个实用工具（可查阅SQL帮助手册）  
                sqlProcess.StartInfo.Arguments = " -U sa -P sa -d SqlTest -i C:\\Program Files\\Microsoft SQL Server\\MSSQL10.SQLEXPRESS\\MSSQL\\Data";//获取启动程序时的参数  
                sqlProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//调用进程的窗口状态，隐藏为后台 
                sqlProcess.Start();
                sqlProcess.WaitForExit();
                sqlProcess.Close();
            }
            TransfEvent();
            this.Close();
        }
    
        /// <summary>  
        /// 创建数据库，调用ExecuteNonQuery()执行  
        /// </summary>  
        /// <param name="conn"></param>  
        /// <param name="DatabaseName"></param>  
        /// <param name="Sql"></param>
        private void ExecuteSql(string conn, string DatabaseName, string Sql)
        {
            SqlConnection mySqlConnection = new SqlConnection();
            SqlCommand Command = new SqlCommand();
            try
            {
                mySqlConnection = new SqlConnection(conn);
                Command = new SqlCommand(Sql, mySqlConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Command.Connection.Open();
            Command.Connection.ChangeDatabase(DatabaseName);  
            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Command.Connection.Close();
            }
        }

        private void dbCreate_Load(object sender, EventArgs e)
        {
            txtDbName.Text = "";
        }

        //得到不包括数据库名的连接字符串
        private  string GetdbbConnctionString()
        {
            string enString = ConfigurationManager.ConnectionStrings["MyStrConn"].ConnectionString.ToString();
            string[] t = enString.Split(';');
            StringBuilder myStringBuilder = new StringBuilder();
            foreach (string item in t)
            {
                if (!item.Contains("Initial"))
                {
                    myStringBuilder.Append(item + ";");
                }
            }
            return myStringBuilder.ToString();
        }
    }
}
