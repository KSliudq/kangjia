namespace kangjiabase
{
    using System;
    //下位机返回升级开始
    public class Cmd_X_Upgrade : Command
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
            return "Device Command: Cmd_X_Upgrade";
        }
    }
}

