﻿namespace kangjiabase
{
    using System;
    //2.6.2.4.2	测量异常结束的协议
    public class Cmd_S_ReportErr : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x14;
            this.CommandData[4] = 0x15;

            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_ReportErr";
        }
    }
}

