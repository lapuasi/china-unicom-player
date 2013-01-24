using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShowRoomSys.Client.Play
{
    public class Client //: ControlService.IMainControlCallback
    {
        public void SendMessage(string message)
        {
            File.WriteAllText("log.txt",string.Format("[ClientTime{0:HHmmss}]Service Broadcast:{1}", DateTime.Now, message));
        }
    }
}
