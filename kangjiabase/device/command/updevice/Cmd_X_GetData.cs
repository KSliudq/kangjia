namespace kangjiabase
{
    using System;
    //3.5	下位机向上位机请求固件数据的协议
    public class Cmd_X_GetData : Command
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
            return "Host Command: Cmd_X_GetData";
        }
    }
}

