namespace kangjiabase
{
    using System;

    public abstract class Command
    {
        public byte[] CommandData = null;
        public byte DataOffset = 3;
        public const byte H1 = 0xa5;
        public const byte H2 = 0x5a;

        public abstract byte[] GetData();
      

        public abstract void PutData(byte[] pData);
        //校验位 校验过程将略过包头的两个字节 从第三个字节开始
        protected void SetCRC()
        {
            byte num = 0;
            for (int i = 2; i < (this.CommandData.Length - 3); i++)
            {
                num = (byte) (num + this.CommandData[i]);
            }
            this.CommandData[this.CommandData.Length - 3] = num;
        }
       

        protected void SetHeader()
        {
            this.CommandData[0] = 0xa5;
            this.CommandData[1] = 0x5a;
        }

        protected void SetFooter()
        {
            
            this.CommandData[this.CommandData.Length - 2] = 0x0D;
            this.CommandData[this.CommandData.Length - 1] = 0x0A;

            this.show(CommandData);
        }
        //输出串
        public void show(byte[] pData)
        {
            if (kangjiabase.yoyoConst.DEBUG)
            {
                string str = "";
                for (int i = 0; i < pData.Length; i++)
                {
                    str += " " + String.Format("{0:X}", pData[i]).PadLeft(2, '0');
                }
                Console.WriteLine(str);
            }
        }
        /// <summary>
        /// 将string类型转为字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] getByte(string str) {
            byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(str);
            return byteArray;
        }
        public abstract override string ToString();

        public byte getByteBystr(string str) {
            byte[] byteArray = BitConverter.GetBytes(Convert.ToInt32(str));

            return byteArray[1];
        }
    }
}

