namespace kangjiabase
{
    using System;
    //上位机开始测量
    //2.3	绿光模式协议
    public class Cmd_S_Green : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 6];
            base.SetHeader();
            this.CommandData[2] = 0x04;
            this.CommandData[3] = 0x01;
            this.CommandData[4] = 0x31;
            this.CommandData[5] = 0x00;
            this.CommandData[6] = 0x00;
            base.SetCRC();
            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Green";
        }
    }
}

