namespace kangjiabase
{
    using System;
    //2.6.2.3	上报测量百分比的软件协议
    public class Cmd_X_Persent : Command
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
            return "Host Command: Cmd_X_Tuch";
        }
    }
}

