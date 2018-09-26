namespace kangjiabase
{
    using System;
    //2.10	上位机请求下位机自检的协议
    public class Cmd_X_Check : Command
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
            return "Host Command: Cmd_X_Check";
        }
    }
}

