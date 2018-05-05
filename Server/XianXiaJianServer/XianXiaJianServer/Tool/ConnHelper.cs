using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace XianXiaJianServer.Tool
{
    class ConnHelper
    { public const string CONNECTION_STRING = "datasource = 127.0.0.1;port = 3306;database = treewar;user = root;pwd = root;";
        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection Connect()
        {
            MySqlConnection conn=new MySqlConnection(CONNECTION_STRING);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                Console.WriteLine("连接数据库异常："+e);
                return null;
            }
        }

        public static void CloseConnection(MySqlConnection conn)
        {
            if(conn!=null)
                conn.Close();
            else
                Console.WriteLine("数据库不能为空");
        }
    }
}
