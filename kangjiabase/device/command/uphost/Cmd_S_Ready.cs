namespace kangjiabase
{
    using System;
    //3.3	上位机查询下位机是否准备就绪的协议
    public class Cmd_S_Ready : Command
    {
        public override byte[] GetData()
        {
            //开头2位，结尾2位
            base.CommandData = new byte[4 + 3];
            base.SetHeader();
            this.CommandData[2] = 0x01;
            this.CommandData[3] = 0x21;
            this.CommandData[4] = 0x22;

            base.SetFooter();
            
            return base.CommandData;
        }

        public override void PutData(byte[] pData)
        {
        }

        public override string ToString()
        {
            return "Host Command: Cmd_S_Ready";
        }
    }
}

