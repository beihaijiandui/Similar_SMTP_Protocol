using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    [Serializable]
    public class MessageEntity
    {
        private CommandType _command;
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private object _data;
        /// <summary>
        /// 消息数据
        /// </summary>
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public MessageEntity(CommandType command, object data)
        {
            this.Command = command;
            this.Data = data;
        }

        public enum CommandType
        {
            OK,
            #region Server's Command
            /// <summary>
            /// 服务器就绪
            /// </summary>
            ServerReady,
            /// <summary>
            /// 允许客户断开
            /// </summary>
            AgreeDisconnect,
            #endregion

            #region Client's Command
            /// <summary>
            /// 呼叫服务器
            /// </summary>
            HelloServer,
            /// <summary>
            /// 数据消息
            /// </summary>
            Data,
            /// <summary>
            /// 请求退出
            /// </summary>
            Quit
            #endregion
        }
    }
}
