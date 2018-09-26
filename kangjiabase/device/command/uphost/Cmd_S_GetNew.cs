namespace kangjiabase
{
    using System;
    using kangjiabase;
    //3.4	下位机查向上位机请求新版固件信息的协议
    public class Cmd_S_GetNew : Command
    {
        public int hardLength = 0;//固件长度
        public override byte[] GetData()
        {
            string version = yoyoConst.VERSION;
            byte[] byverson = System.Text.Encoding.ASCII.GetBytes(version);

            //开头2位，结尾2位
            base.CommandData = new byte[4 + 7 + version.Length];
            base.SetHeader();
            int len = 5+ version.Length;
            byte[] intBuff = BitConverter.GetBytes(len);
            this.CommandData[2] = intBuff[0];
            this.CommandData[3] = 0x22;

            for (int i = 0; i < byverson.Length; i++) {
                this.CommandData[4 + i] = byverson[i];
            }
            byte[] hard = BitConverter.GetBytes(hardLength);
            this.CommandData[this.CommandData.Length - 7] = hard[3];//四个字节hex，高位在前
            this.CommandData[this.CommandData.Length - 6] = hard[2];//四个字节hex，高位在前
            this.CommandData[this.CommandData.Length - 5] = hard[1];//年月日数字(硬件版本)（boot版本）
            this.CommandData[this.CommandData.Length - 4] = hard[0];//四个字节hex，高位在前
            base.SetCRC();//设定校验位
            base.SetFooter();
          
      
            
            return base.CommandData;
        }

     
        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_GetNew";
        }
    }
}

