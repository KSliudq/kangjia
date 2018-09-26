namespace kangjiabase
{
    using System;
    //2.10	上位机告知下位机服务器时间的协议
    public class Cmd_X_Settime : Command
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
            return "Host Command: Cmd_X_Settime";
        }
    }
}

