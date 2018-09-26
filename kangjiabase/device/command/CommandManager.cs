namespace kangjiabase
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using kangjiabase;

    public class CommandManager
    {
        public const int BUFFERSIZE = 1400;// 0x400;//必须是7的倍数
        public CommandReceivedDelegate CommandReceivedHandler = null;
        public DeviceManager Device;
        //private int iCur = 0;
        //private int iLength = 0;
        //private byte[] pBuffer = new byte[BUFFERSIZE];

        public CommandManager(DeviceManager _device)
        {
            this.Device = _device;
        }

        private void Pop(int len)
        {
            //int num = Math.Min(this.iLength, len);
            //this.iLength -= num;
            //this.iCur = (this.iCur + num) % BUFFERSIZE;
        }

        public Command ProcessBuffer(byte[] pDataAll)
        {
            if (yoyoConst.DEBUG)
            {
                if (pDataAll != null)
                {
                    string str = "";
                    for (int i = 0; i < pDataAll.Length; i++)
                    {
                        str += " " + String.Format("{0:X}", pDataAll[i]).PadLeft(2, '0');
                    }
                     //Console.WriteLine("Allcomdata---" + str);
                    LogisTrac.WriteLog("Allcomdata---" + str);
                }
            }
            Command cmd = null;

            List<byte> templist = new List<byte>();
            try
            {
                for (int i = 0; i < pDataAll.Length - 1 && pDataAll.Length > 6; )
                {
                    if (pDataAll[i] == 0xA5 && pDataAll[i + 1] == 0x5A)
                    {
                        int length = Convert.ToInt32(Convert.ToString(pDataAll[i + 2], 16), 16) + 3;
                        templist.Add(pDataAll[i]);
                        templist.Add(pDataAll[i + 1]);
                        templist.Add(pDataAll[i + 2]);
                        for (int k = i + 3; k < i + 3 + length && k < pDataAll.Length; k++)
                        {
                            templist.Add(pDataAll[k]);
                        }
                        if (3 + length > pDataAll.Length) {
                            templist.Add(0x0D);
                            templist.Add(0x0A);
                            LogisTrac.WriteLog("补位结尾字符--");
                        }
                        byte[] pData = templist.ToArray();
                        if (CommandBroker.checkData(pData))
                        {
                            cmd = CommandBroker.GetCommand(pData);

                            templist = new List<byte>();

                            if ((this.CommandReceivedHandler != null) && (cmd != null))
                            {
                                this.CommandReceivedHandler(cmd);
                            }
                        }
                        i = i + pData.Length - 1;
                    }
                    else
                    {
                        i++;
                    }


                }
            }
            catch { 
            }

            return cmd;
        }

        public void Push(byte[] pData)
        {
            //for (int i = 0; i < pData.Length; i++)
            //{
            //    this.pBuffer[((this.iCur + this.iLength) + i) % BUFFERSIZE] = pData[i];
            //}
            //this.iLength += pData.Length;
        }


        public void Clear()
        {
            //iCur = 0;
            //iLength = 0;
            //pBuffer = new byte[BUFFERSIZE];
        }
        public delegate void CommandReceivedDelegate(Command cmd);
    }
}

