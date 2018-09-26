namespace kangjiabase
{
    using System;
    //2.6.1.1	上位机告知下位机进入选男女界面的协议
    public class Cmd_X_Sex : Command
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
            return "Host Command: Cmd_X_Sex";
        }
    }
}

