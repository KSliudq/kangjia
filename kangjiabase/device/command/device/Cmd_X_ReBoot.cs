namespace kangjiabase
{
    using System;
    //下位机返回重启
    public class Cmd_X_ReBoot : Command
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
            return "Device Command: Cmd_X_ReBoot";
        }
    }
}

