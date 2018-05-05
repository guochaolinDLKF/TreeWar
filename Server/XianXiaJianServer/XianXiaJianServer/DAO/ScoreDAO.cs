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
    class ScoreDAO
    {
        public ScoreData GetScoreByUserName(MySqlConnection conn,string username)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd =
                    new MySqlCommand("select * from scoredata where username = @username ", conn);
                cmd.Parameters.AddWithValue("username", username);
                reader = cmd.ExecuteReader();
                if (reader.Read())//HasRows表示不管是有一行或者多行数据，只要有数据就返回True
                {
                    int id = reader.GetInt32("id");
                    int totalCount = reader.GetInt32("totalcount");
                    int wincount = reader.GetInt32("wincount");
                    ScoreData score =new ScoreData(ReturnCode.Success, id, username, totalCount, wincount);
                    return score;
                }
                else
                {
                    ScoreData score = new ScoreData(ReturnCode.Fail, -1, username, 0, 0);
                    return score;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetScoreByUserId函数执行的时候出现异常：" + e);
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
        public bool AddScoreData(MySqlConnection conn, string username, int totalCount, int winCount) 
        { 
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into scoredata set username=@username , totalcount = @totalcount , wincount = @wincount", conn);
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("totalcount", totalCount);
                cmd.Parameters.AddWithValue("wincount", winCount);
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
