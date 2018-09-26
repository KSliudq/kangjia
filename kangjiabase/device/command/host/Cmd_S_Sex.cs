namespace kangjiabase
{
    using System;
    //2.6.1.1	上位机告知下位机进入选男女界面的协议
    public class Cmd_S_Sex : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x0F;
            this.CommandData[4] = 0x10;

            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Sex";
        }
    }
}

