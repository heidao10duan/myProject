using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LROSE_BLL;
using LROSE_BLL.MR;
using LROSE_Model;
using LROSE_Model.MrData;
using LROSE_DAL;
using System.Data.Entity;
using System.Xml;
using System.Configuration;
using LROSE_BLL.PMData;
using LROSE_BLL.Basis;
using System.IO;
using System.Data;
using LROSE_Model.PMData;
using System.Data.SQLite;


namespace EFCodeFirstDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //try
            //{
            //    LROSRDbContext lroseContext = new LROSRDbContext(1);
            //    PMAllMoid duhanxu = new PMAllMoid();
            //    duhanxu.KPid = 1;
            //    duhanxu.MeContext = "duhanxu";
            //    lroseContext.PMAllMoid.Add(duhanxu);
            //    lroseContext.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    Console.ReadLine();
            //}

            //using (SQLiteConnection cn = new SQLiteConnection("data source=D:\\sqliter.db;Pooling=true;FailIfMissing=false"))
            //{
            //    cn.Open();
            //}

            using (LROSRDbContext lroseContext = new LROSRDbContext())
            {
                
                PMAllMoid duhanxu = new PMAllMoid();
                List<PMAllMoid> du = new List<PMAllMoid>();
              
                duhanxu.KPid = 23;
                duhanxu.MeContext = "duhanxu2";
                du.Add(duhanxu);
                //lroseContext.Set<PMAllMoid>().Add(duhanxu);
                lroseContext.PMAllMoid.AddRange(du);
                lroseContext.SaveChanges(); 
            }
        }
    }
}
