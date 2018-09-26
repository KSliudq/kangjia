namespace kangjiabase
{
    using System;
    //上位机开始测量
    //2.6.2.1	开启测量的软件协议
    public class Cmd_S_Start : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x10;
            this.CommandData[4] = 0x11;

            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Start";
        }
    }
}

