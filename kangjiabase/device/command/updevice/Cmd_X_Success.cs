namespace kangjiabase
{
    using System;
    //3.6	下位机告知上位机固件下载成功的协议
    public class Cmd_X_Success : Command
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
            return "Host Command: Cmd_X_Success";
        }
    }
}

