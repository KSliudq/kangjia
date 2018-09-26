namespace kangjiabase
{
    using System;
    //上位机开始测量
    //2.12	锁定状态通知
    public class Cmd_S_Lock : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x0B;
            this.CommandData[4] = 0x0C;
            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Lock";
        }
    }
}

