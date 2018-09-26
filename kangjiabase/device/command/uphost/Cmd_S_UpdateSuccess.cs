namespace kangjiabase
{
    using System;
    //3.7	下位机通知上位机升级成功的协议
    public class Cmd_S_UpdateSuccess : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x25;
            this.CommandData[4] = 0x26;

            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_UpdateSuccess";
        }
    }
}

