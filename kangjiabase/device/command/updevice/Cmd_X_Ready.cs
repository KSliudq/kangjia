namespace kangjiabase
{
    using System;
    //3.3	上位机查询下位机是否准备就绪的协议
    public class Cmd_X_Ready : Command
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
            return "Host Command: Cmd_X_Ready";
        }
    }
}

