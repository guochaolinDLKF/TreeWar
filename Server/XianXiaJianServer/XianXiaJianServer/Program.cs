using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XianXiaJianServer.Model;
using XianXiaJianServer.Server;

namespace XianXiaJianServer
{
    class Program
    {
      
        static void Main(string[] args)
        {
            MainServer m_socket = new MainServer(IPAddress.Parse("127.0.0.1"),5807,100);
            //m_socket.Init();
            m_socket.Start();

            //m_socket.ReceiveClientData += GetReceiveData;
            //MainServer server = new MainServer("127.0.0.1",8967);
            //server.Start();
            // string str = "{'ReturnCode':0,'UserId':'801d5b816c905c9e','UserName':'334','Password':'334'}";

            //UserData user = ParsePackage.JSONDataDeSerialize<UserData>(str);
            //Console.WriteLine(user.UserName);
            
            Console.ReadKey();
        }
   
    }
}
