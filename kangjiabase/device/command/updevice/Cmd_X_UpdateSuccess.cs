namespace kangjiabase
{
    using System;
    //3.7	下位机通知上位机升级成功的协议
    public class Cmd_X_UpdateSuccess : Command
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
            return "Host Command: Cmd_X_UpdateSuccess";
        }
    }
}

