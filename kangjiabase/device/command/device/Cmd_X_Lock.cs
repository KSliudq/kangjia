namespace kangjiabase
{
    using System;
    //下位机返回锁定状态
    public class Cmd_X_Lock : Command
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
            return "Device Command: Cmd_X_Lock";
        }
    }
}

