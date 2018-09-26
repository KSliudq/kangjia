namespace kangjiabase
{
    using System;
    //2.6.2.2	上报是否接触电极的软件协议
    public class Cmd_X_SN : Command
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
            return "Host Command: Cmd_X_SN";
        }
    }
}

