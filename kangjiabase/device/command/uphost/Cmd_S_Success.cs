namespace kangjiabase
{
    using System;
    //3.6	下位机告知上位机固件下载成功的协议
    public class Cmd_S_Success : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x24;
            this.CommandData[4] = 0x25;

            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Success";
        }
    }
}

