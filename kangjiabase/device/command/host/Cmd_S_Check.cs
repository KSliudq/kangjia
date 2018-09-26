namespace kangjiabase
{
    using System;
    //2.10	上位机请求下位机自检的协议
    public class Cmd_S_Check : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x09;
            this.CommandData[4] = 0x0A;

            base.SetFooter();
            show(base.CommandData);
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Check";
        }
    }
}

