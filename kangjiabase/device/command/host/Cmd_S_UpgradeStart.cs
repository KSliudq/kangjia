namespace kangjiabase
{
    using System;
    //上位机开始测量
    //2.9   开始升级
    public class Cmd_S_UpgradeStart : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 4];
            base.SetHeader();
            this.CommandData[2] = 0x02;
            this.CommandData[3] = 0x16;
            this.CommandData[4] = 0x01;
            base.SetCRC();
            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_UpgradeStart";
        }
    }
}

