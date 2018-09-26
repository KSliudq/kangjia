namespace kangjiabase
{
    using System;
    //3.4	下位机查向上位机请求新版固件信息的协议
    public class Cmd_X_GetNew : Command
    {
        public override byte[] GetData()
        {
                  
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
            base.CommandData = pData;
        }

        public override string ToString()
        {
            return "Host Command: Cmd_X_GetNew";
        }
    }
}

