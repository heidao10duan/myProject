//using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.SqlClient;
using System.Data.Entity;
using LROSE_Model.MrData;
using LROSE_Model.PMData;
using System.Configuration;
//using LROSE_Model;
//using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration;
using System.Data.SQLite;
//using System.Data.SQLite.EF6.Migrations;

namespace LROSE_DAL
{

    public partial class LROSRDbContext : DbContext
    {
        public static string GetEFConnctionString(string dbName)
        {

            string enString = ConfigurationManager.ConnectionStrings["MyStrConn"].ConnectionString.ToString();
            string[] t = enString.Split(';');
            StringBuilder myStringBuilder = new StringBuilder();
            if (enString.Contains("Pooling"))
            {
                string path = GetDirectoryPath();//文件夹路径              
                foreach (string item in t)
                {
                    if (!item.Contains("Source"))
                    {
                        myStringBuilder.Append(item + ";");
                    }
                    else
                    {
                        myStringBuilder.Append("Data Source=" + path + "\\" + dbName + ";");
                    }
                }
            }
            else
            {
                foreach (string item in t)
                {
                    if (!item.Contains("Initial"))
                    {
                        myStringBuilder.Append(item + ";");
                    }
                    else
                    {
                        myStringBuilder.Append("Initial Catalog= " + dbName + ";");
                    }
                }
            }
            return myStringBuilder.ToString();
        }

        //sqliite文件夹路径
        private static string GetDirectoryPath()
        {
            string xmlPath = ".\\LROSEDB";//路径xml文件夹  bin下面的
            if (!Directory.Exists(xmlPath))
            {
                Directory.CreateDirectory(xmlPath);
            }
            return xmlPath;
        }



        public LROSRDbContext(string dbName)
            : base(GetEFConnctionString(dbName))
        {
        }

        public LROSRDbContext()
                        //: base("Data Source=.\\LROSEDB\\test.db")
                        //: base("name=MyStrConn")
                        //: base(@"Data Source=E:\test.db;Pooling=true")
                        //:base("DefaultConnection")
                        : base(new SQLiteConnection()
                        {
                            ConnectionString =
                    new SQLiteConnectionStringBuilder()
                    { DataSource = "D:\\sqliter.db", ForeignKeys = true }
                    .ConnectionString
                        }, true)

        {
            //第一次运行时，初始化数据库
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LROSRDbContext, Configuration>());
        }

        public DbSet<MrTableAllColumn> MrTableAllColumn { get; set; }
        public DbSet<PMAllMoid> PMAllMoid { get; set; }
        public DbSet<PMTableListColumn> PMTableListColumn { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(LROSRDbContext).Assembly);
        }
    }



    public class Configuration : DbMigrationsConfiguration<LROSRDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new System.Data.SQLite.EF6.Migrations.SQLiteMigrationSqlGenerator());
        }
        protected override void Seed(LROSRDbContext context)
        {
        }
    }



    public class Mapping
    {
        public class PMAllMoidMap : EntityTypeConfiguration<PMAllMoid>
        {
            public PMAllMoidMap()
            {
            }
        }
        public class PMTableListColumnMap : EntityTypeConfiguration<PMTableListColumn>
        {
            public PMTableListColumnMap()
            {
            }
        }

        public class MrTableAllColumnMap : EntityTypeConfiguration<MrTableAllColumn>
        {
            public MrTableAllColumnMap()
            {
            }
        }
    }
}
