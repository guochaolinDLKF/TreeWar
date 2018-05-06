using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XianXiaJianServer.Server;

namespace XianXiaJianServer.Controller
{
    class ControllerManager
    {        //将Controller存进字典中，根据请求类型获取控制器
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private MainServer server;
        public ControllerManager(MainServer server)
        {
            this.server = server;
            Init();
        }
        void Init()
        {
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
            controllerDict.Add(RequestCode.UserData, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController());

        }
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="requestCode">请求类型</param>
        /// <param name="actionCode">执行请求的函数类型</param>
        /// <param name="data">数据</param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, ClientPeer client)
        {
            Console.WriteLine("actionCode="+actionCode);
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if (!isGet)
            {
                Console.WriteLine("无法得到[" + requestCode + "]所对应的请求"); return;
            }
            string methodName = Enum.GetName(typeof(ActionCode), actionCode);//将枚举类型转化为字符串
            //获取方法信息
            MethodInfo mi = controller.GetType().GetMethod(methodName);//GetType获取对应的实例，GetMothod根据方法名获取对应的方法
            if (mi == null)
            {
                Console.WriteLine("[警告]在Controller" + controller.GetType() + "中没有找到对应的处理方法[" + methodName + "]");
                return;
            }
            object[] parameters = new object[] { data, client, server };
            object o = mi.Invoke(controller, parameters);//调用Controller中的函数，并且传入数据

            if (o == null&& string.IsNullOrEmpty(data))
            {
                return;
            }
            //返回给客户端
            server.SendResponse(client, actionCode, o as string); 
        }
    }
}
