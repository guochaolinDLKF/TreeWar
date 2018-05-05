using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using XianXiaJianServer.Model;
using XianXiaJianServer.Server;

namespace XianXiaJianServer.Controller
{
    class GameController:BaseController
    {
        public GameController()
        {
            requestCode=RequestCode.Game;
        }

        private ScoreData sendMsg;
        public string StartGame(string data, ClientPeer client, MainServer server)
        {
            if (client.IsHouseOwner())
            {
                Room room = client.GetRoom;
                sendMsg=new ScoreData(ReturnCode.Success,0,null,0,0);
                room.BroadcastMessage(client, ActionCode.StartGame, ParsePackage.JSONDataSerialize(sendMsg));
                room.ShowTimer();
                return ParsePackage.JSONDataSerialize(sendMsg); 
            }
            else
            {
                sendMsg = new ScoreData(ReturnCode.Fail,0,null,0,0);
                return ParsePackage.JSONDataSerialize(sendMsg);
            }
        }
    }
}
