namespace kangjiabase
{
    using System;
    using kangjiabase;
    public class CommandBroker
    {
        public static Command GetCommand(byte[] pData)
        {
            Command command = null;
            string str = "";
            if (yoyoConst.DEBUG)
            {
                if (pData != null)
                {
                    for (int i = 0; i < pData.Length; i++)
                    {
                        str += " " + String.Format("{0:X}", pData[i]).PadLeft(2, '0');
                    }
                  //  Console.WriteLine("comdata---" + str);
                   LogisTrac.WriteLog("comdata---" + str);
                }
            }

            if (pData != null && pData.Length >=5)
            {
                if (!checkData(pData))
                {
                    return command;
                }
                if ((pData[0] != 165) || (pData[1] != 90))
                {
                    return command;
                }
                byte pos3 = pData[2];
                byte pos4 = pData[3];
                byte pos5 = pData[4];
               
                if (pos3 == 0x04 && pos4 == 0x01)
                {
                    command = new Cmd_X_Reset();
                }
                else if (pos4 == 0x09)
                {
                    //2.10	上位机请求下位机自检的协议
                    command = new Cmd_X_Check();
                }
                else if (pos4 == 0x0F)
                {
                    //2.6.1.1	上位机告知下位机进入选男女界面的协议
                    command = new Cmd_X_Sex();
                }
                else if (pos4 == 0x10)
                {
                    //2.6.2.1	开启测量的软件协议
                    command = new Cmd_X_Start();
                }
                else if (pos4 == 0x11)
                {
                    //2.6.2.2	上报是否接触电极的软件协议
                    command = new Cmd_X_Tuch();
                }
                else if (pos4 == 0x12)
                {
                    //2.6.2.3	上报测量百分比的软件协议
                    command = new Cmd_X_Persent();
                }
                else if (pos4 == 0x13)
                {
                    //2.6.2.4.1	测量正常结束上传数据的协议
                    command = new Cmd_X_Report();
                }
                else if (pos4 == 0x14)
                {
                    //2.6.2.4.2	测量异常结束的协议
                    command = new Cmd_X_ReportErr();
                }
                else if (pos4 == 0x17)
                {
                    //2.10	上位机告知下位机服务器时间的协议
                    command = new Cmd_X_Settime();
                }
                else if (pos4 == 0x20)
                {
                    //3.2	上位机查询下位机固件版本和SN的协议
                    command = new Cmd_X_SN();
                }
                else if (pos4 == 0x21)
                {
                    //3.3	上位机查询下位机是否准备就绪的协议
                    command = new Cmd_X_Ready();
                }
                else if (pos4 == 0x22)
                {
                    //3.4	下位机查向上位机请求新版固件信息的协议
                    command = new Cmd_X_GetNew();
                }
                else if (pos4 == 0x23)
                {
                    //3.5	下位机向上位机请求固件数据的协议
                    command = new Cmd_X_GetData();
                }
                else if (pos4 == 0x24)
                {
                    //3.6	下位机告知上位机固件下载成功的协议
                    command = new Cmd_X_Success();
                }
                else if (pos4 == 0x25)
                {
                    //3.7	下位机通知上位机升级成功的协议
                    command = new Cmd_X_UpdateSuccess();
                }
                else if (pos4 == 0x07)
                {
                    //2.5.1	下位机通知上位机收到开关机指令的协议
                    command = new Cmd_X_BootShut();
                }
                else if (pos4 == 0x08)
                {
                    //2.5.2	下位机通知上位机收到定时开关机指令的协议
                    command = new Cmd_X_TimingPower();
                }
                else if (pos4 == 0x0A)
                {
                    //2.6	下位机通知上位机收到重启指令的协议
                    command = new Cmd_X_ReBoot();
                }
                else if (pos4 == 0x16)
                {
                    //2.9	下位机通知上位机收到上位机自升级指令的协议
                    command = new Cmd_X_Upgrade();
                }
                else if (pos4 == 0x0B)
                {
                    //2.12	下位机通知上位机收到上位机锁定指令的协议
                    command = new Cmd_X_Lock();
                }
                if (command != null)
                {
                    command.CommandData = new byte[pData.Length];
                    command.PutData(pData);
                }
            }
            return command;
        }

        public static bool checkData(byte[] pData)
        {
            //if (yoyoConst.currtenStep==null || yoyoConst.currtenStep.step != yoyoConst.YoyoStep.upgrade)
            //{
            //    return  true;
            //}
            byte num = 0;
            for (int i = 2; i < (pData.Length - 3); i++)
            {
                num = (byte)(num + pData[i]);
            }
            byte check = pData[pData.Length - 3];

            if (num != check) {
                return false;
            }

            int len = pData.Length - 6;
            byte[] intBuff = BitConverter.GetBytes(len);
            byte zhenchang = intBuff[0];//帧长

            if (zhenchang != pData[2]) {
                return false;
            }
            return true;
        }
    }
}

