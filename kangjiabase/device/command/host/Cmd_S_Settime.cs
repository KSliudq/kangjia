namespace kangjiabase
{
    using System;
    //2.10	上位机告知下位机服务器时间的协议
    public class Cmd_S_Settime : Command
    {
        private string _start;
        public Cmd_S_Settime(string start)
        {
            this._start = start;
        }
      
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 9];
            base.SetHeader();
            this.CommandData[2] = 0x07;
            this.CommandData[3] = 0x17;

            byte[] hard = getByte(this._start);

            //for (int i = 0; i < this._start.Length; i=i+2)
            //{
            //    this.CommandData[4 + i] = BitConverter.GetBytes(Convert.ToInt32(str)); 
            //}
            this.CommandData[4] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(0,2)))[0];
            this.CommandData[5] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(2, 2)))[0];
            this.CommandData[6] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(4, 2)))[0];
            this.CommandData[7] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(6, 2)))[0];
            this.CommandData[8] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(8, 2)))[0];
            this.CommandData[9] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(10, 2)))[0]; 
            base.SetCRC();
            base.SetFooter();
            show(base.CommandData);
            return base.CommandData;
        }
       
        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_TimingPower";
        }
    }
}

