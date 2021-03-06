﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using XianXiaJianServer.DAO;
using XianXiaJianServer.Model;
using XianXiaJianServer.Server;

namespace XianXiaJianServer.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        private ScoreDAO scoreDAO = new ScoreDAO();
        public UserController()
        {
            requestCode = RequestCode.UserData;
        }
        ScoreData score;
        public string Login(string data, ClientPeer client, MainServer server)
        {
            Console.WriteLine("服务器收到的数据：" + data);
            UserData ReciveUserData= ParsePackage.JSONDataDeSerialize<UserData>(data);
           
            //string[] strs = data.Split(',');
            UserData user = userDAO.VerifyUser(client.MySqlConn, ReciveUserData.UserName, ReciveUserData.Password);
            if (user == null)
            {
                score = new ScoreData(ReturnCode.Fail, 0, null, 0, 0);
                return ParsePackage.JSONDataSerialize(score);
            }
            else
            {
                Console.WriteLine("user.UserName：" + user.UserName);
                client.SetCurAccountData(user);
                score = scoreDAO.GetScoreByUserName(client.MySqlConn, user.UserName); 
                client.SetCurPlayerData(score);
                server.mUserDataList.Add(user.UserName,user);//添加账户
                Console.WriteLine("一个账户登录了");
                return ParsePackage.JSONDataSerialize(score); 
            }
        } 
        public string QuitLogin(string data, ClientPeer client, MainServer server)
        {
            Console.WriteLine("服务器收到的数据：" + data);
            UserData ReciveUserData = ParsePackage.JSONDataDeSerialize<UserData>(data);

            if (server.mUserDataList.ContainsKey(ReciveUserData.UserName))
            {
                lock (server.mUserDataList)
                {
                    server.mUserDataList.Remove(ReciveUserData.UserName);
                }
                Console.WriteLine("一个账户退出登录了");
                score = new ScoreData(ReturnCode.Success, 0, ReciveUserData.UserName, 0, 0);
            }
            else
            {
                score = new ScoreData(ReturnCode.Fail, 0, ReciveUserData.UserName, 0, 0);
            }
           
            return ParsePackage.JSONDataSerialize(score);
        }
        public string Regiest(string data, ClientPeer client, MainServer server)
        {
            Console.WriteLine("服务器收到的数据：" + data);
            UserData ReciveUserData = ParsePackage.JSONDataDeSerialize<UserData>(data);
            //string[] strs = data.Split(',');
         
            bool isSuccess = userDAO.GetUserByUserName(client.MySqlConn, ReciveUserData.UserName); 
            if (isSuccess)
            {
                score = new ScoreData(ReturnCode.Fail, 0, null, 0, 0);
                return ParsePackage.JSONDataSerialize(score);
            }
            else
            {
                //Console.WriteLine("接受到的数据为：" + strs[0] + ";" + strs[1] + ";" + strs[2]);
                bool resoult = userDAO.AddUser(client.MySqlConn, ReciveUserData.UserId, ReciveUserData.UserName, ReciveUserData.Password);
                if (resoult)
                {
                   bool isRes= scoreDAO.AddScoreData(client.MySqlConn, ReciveUserData.UserName, 0, 0);
                    if (isRes)
                    {
                        score = new ScoreData(ReturnCode.Success, 0, ReciveUserData.UserName, 0, 0);
                        client.SetCurPlayerData(score);
                        client.SetCurAccountData(ReciveUserData);
                        server.mUserDataList.Add(ReciveUserData.UserName, ReciveUserData);//添加账户
                        Console.WriteLine("注册成功给客户端回调");
                        return ParsePackage.JSONDataSerialize(score);
                    }
                    else
                    {
                        score = new ScoreData(ReturnCode.Fail, 0, null, 0, 0);
                        return ParsePackage.JSONDataSerialize(score);
                    }
                 
                }
                else
                {
                    score=new ScoreData(ReturnCode.Fail, 0,null,0,0);
                    
                    return ParsePackage.JSONDataSerialize(score);
                }

            }
        }
    }
}
