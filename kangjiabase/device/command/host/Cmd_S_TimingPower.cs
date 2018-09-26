namespace kangjiabase
{
    using System;
    //上位机开始测量
    //2.5.2	定时开关机
    public class Cmd_S_TimingPower : Command
    {
        private string _start;
        private string _end;
        public  Cmd_S_TimingPower(string start, string end)
        {
            this._start = start;
            this._end = end;
            //GetData();
        }
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 7];
            base.SetHeader();
            this.CommandData[2] = 0x05;
            this.CommandData[3] = 0x08;
            if (string.IsNullOrEmpty(this._start) || this._start=="ffff")
            {
                this.CommandData[4] = 0xff;//todo 开机时间
                this.CommandData[5] = 0xff;//todo 开机时间
            }
            else {
               // byte[] hard = BitConverter.GetBytes(Convert.ToInt32(this._start));
                this.CommandData[4] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(0, 2)))[0];
                this.CommandData[5] = BitConverter.GetBytes(Convert.ToInt32(this._start.Substring(2, 2)))[0]; 
            }
            if (string.IsNullOrEmpty(this._end) || this._end == "ffff")
            {

                this.CommandData[6] = 0xff;
                this.CommandData[7] = 0xff;
            }
            else
            {
                //byte[] hard = BitConverter.GetBytes(Convert.ToInt32(this._start));
                this.CommandData[6] = BitConverter.GetBytes(Convert.ToInt32(this._end.Substring(0, 2)))[0];
                this.CommandData[7] = BitConverter.GetBytes(Convert.ToInt32(this._end.Substring(2, 2)))[0];
            }
            base.SetCRC();
            base.SetFooter();
            show(CommandData);
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

