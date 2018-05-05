using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianXiaJianServer.Server;

namespace XianXiaJianServer.Controller
{
    abstract class BaseController
    {
      protected  RequestCode requestCode = RequestCode.None;//设置默认请求枚举
        public virtual void DefaultHandle() { }//默认请求函数
        /// <summary>
        /// 默认请求参数为客户端发过来的数据和负责和客户端交互的Socket、Server类
        /// </summary>
        /// <param name="data"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public virtual string DefaultHandle(string data, ClientPeer client, MainServer server)
        {
            return null;//返回值表示要给客户端返回的数据
        }//设置默认请求函数
    }

}
