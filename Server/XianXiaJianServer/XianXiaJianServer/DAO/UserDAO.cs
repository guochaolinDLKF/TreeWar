using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using MySql.Data.MySqlClient;
using XianXiaJianServer.Model;

namespace XianXiaJianServer.DAO
{
    class UserDAO
    {
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserData VerifyUser(MySqlConnection conn, string username, string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand("select * from userdata where username = @username and password = @password", conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string userid = reader.GetString("userid");
                    UserData user = new UserData(ReturnCode.None, userid, username, password);
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在VerifyUser函数执行的时候出现异常：" + e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

            }
            return null;
        }

        public bool GetUserByUserName(MySqlConnection conn, string username) 
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand("select * from userdata where username = @username ", conn);
                cmd.Parameters.AddWithValue("username", username); 
                reader = cmd.ExecuteReader();
                if (reader.HasRows)//HasRows表示不管是有一行或者多行数据，只要有数据就返回True
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetUserByUserId函数执行的时候出现异常：" + e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

            }
            return false;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public bool AddUser(MySqlConnection conn, string userid, string username, string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into userdata set userid=@userid , username = @username , password = @password", conn);
                cmd.Parameters.AddWithValue("userid", userid); 
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);
                int dataCount = cmd.ExecuteNonQuery();
                Console.WriteLine("插入" + dataCount + "条数据");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("在AddUser函数执行的时候出现异常：" + e);
                return false;
            }
        }
    }
}
