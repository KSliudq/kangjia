﻿namespace kangjiabase
{
    using System;
    //2.6.2.4.2	测量异常结束的协议
    public class Cmd_X_ReportErr : Command
    {
        public override byte[] GetData()
        {
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
            base.CommandData = pData;
        }

        public override string ToString()
        {
            return "Device Command: Cmd_X_ReportErr";
        }
    }
}

