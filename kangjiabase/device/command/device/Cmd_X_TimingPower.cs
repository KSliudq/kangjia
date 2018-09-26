namespace kangjiabase
{
    using System;
    //下位机返回定时开关机
    public class Cmd_X_TimingPower : Command
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
            return "Device Command: Cmd_X_TimingPower";
        }
    }
}

