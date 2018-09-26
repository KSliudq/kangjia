namespace kangjiabase
{
    using System;
    //1.3 绿光模式协议
    public class Cmd_X_Green : Command
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
            return "Device Command: Cmd_X_Start";
        }
    }
}

