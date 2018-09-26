namespace kangjiabase
{
    using System;
    //3.5	下位机向上位机请求固件数据的协议
    public class Cmd_S_GetData : Command
    {
        public int packagePos = 0;//包数
        public byte[] package;//固件数据
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 5 + package.Length];
            base.SetHeader();

            int len = 3 + package.Length;
            byte[] intBuff = BitConverter.GetBytes(len);
            this.CommandData[2] = intBuff[0];//帧长

            this.CommandData[3] = 0x23;

            byte[] packagePosbyte = BitConverter.GetBytes(packagePos);

            this.CommandData[4] = packagePosbyte[1];//包数
            this.CommandData[5] = packagePosbyte[0];//包数

            for (int i = 0; i < package.Length; i++)
            {
                this.CommandData[6 + i] = package[i];
            }
            base.SetCRC();//设定校验位

            base.SetFooter();

      
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_GetData";
        }
    }
}

