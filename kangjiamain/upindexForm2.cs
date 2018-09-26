using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Collections;
using System.Diagnostics;
using kangjiabase;
using SharpCompress.Archive;
using SharpCompress.Common;
using System.IO.Ports;
using System.Management;



namespace kangjiamain
{
    public partial class upindexForm2 : Form
    {
        #region 变量

        #region 初始化变量
        System.Timers.Timer Init_timer = null;//初始化时检测是否成功,如果不成功循环检测
        int totalcount = (1000 * 5 * 60) / 2000;//5分钟/2秒
        bool Init_Check_Main = false;//判断初始化是否成功,不成功则继续运行线程,否则停止线程
        bool Init_Check_Maining = false;
        bool Init_Check_Device1 = false;//串口打开
        bool Init_Check_Device1ing = false;
        bool Init_Check_Device2 = true;//设备是否初始化成功
        bool Init_Check_Device2ing = false;
        bool Init_Check_SN = false;//是否获取SN
        bool Init_Check_SNing = false;

        bool Init_Check_Net = false;//网络是否初始化成功
        bool Init_Check_Neting = false;

        bool Init_Device_doing = false;


        int iweb_type = 0;//4g模式  1华为 2移远
        string com_4g = "";//4g模块端口号
        #endregion

        #region 串口变量
        public SerialPort serialPort1 = new SerialPort();//定义一个串口类的串口变量 

        private DeviceManager devicemanager = new DeviceManager();
        private static object comLockObj = new object();//防止并发
        #endregion

        #region 定时器变量
        System.Timers.Timer netcheckTimer;//实例化Timer类，验证网络是否畅通
        #endregion

        #region 升级用
        private byte[] testuploadByte = null;//当前正在上传的文件数组
        private int testuploadbaselength = Convert.ToInt32(yoyoConst.FILE_LEN);


        private string up_mcpVersion_url = Application.StartupPath + "\\backup\\mcpVersion\\";
        private string up_appVersion_url = Application.StartupPath + "\\backup\\appVersion\\";
        private string up_resVersion_url = Application.StartupPath + "\\backup\\resVersion\\";
        private retV1UploadU01 model;
        Cmd_S_GetNew cmdnew = new Cmd_S_GetNew();
        #endregion


        #endregion

        #region 构造函数load
        public upindexForm2()
        {
            InitializeComponent();
            this.TopMost = false;//是否在最前
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            //设定画面可以运行线程
            CheckForIllegalCrossThreadCalls = false;
            //初始化画面,默认运行starting.swf
            this.axShockwaveFlash1.Movie = Application.StartupPath + "\\res\\yoyo.swf";
            this.axShockwaveFlash1.Play();
        }

        private void indexForm2_Load(object sender, EventArgs e)
        {
            Chenck_4g();

            if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则定位
            {
                com_port(com_4g);
                SendData3();
            }
            //启动线程,进行初始化
            Init_timer = new System.Timers.Timer(2000);
            Init_timer.Elapsed += new System.Timers.ElapsedEventHandler((s, ex) => init_Tick(s, ex));
            Init_timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            Init_timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  

            // //第一次调用初始化方法,如果成功则画面到广告页,否则启动线程间隔时间再调用初始化
            // Thread Init_thread = new Thread(Init_Main);
            // Init_thread.Start();
            // yoyoConst.EQU_SN = "KJ103IS00000211";
            flash_change2(yoyoConst.YoyoStep.upgrade);
        }

        private void indexForm2_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }
        #endregion

        #region 初始化
        //初始化方法
        private void Init_Main()
        {
            try
            {



                //网络检测
                if (!this.Init_Check_Net)
                {
                    netcheck_delay();
                }

                //硬件检测
                if (!this.Init_Check_Device1)
                {
                    if (!Init_Check_Device1ing)
                    {
                        Init_Check_Device1ing = true;
                        this.Init_Check_Device1 = Init_Com1();
                    }
                }



                if (this.Init_Check_Device1 && this.Init_Check_Device2 && !this.Init_Check_SN && !Init_Device_doing)
                {
                    if (!this.Init_Check_SNing)
                    {
                        // this.Init_Check_SNing = true;
                        //获取设备编号
                        devicemanager.Write(new Cmd_S_SN());
                    }
                }

                //liudq test
                //yoyoConst.EQU_SN = "KJ103IS00000311";//设备信息（SN码）
                //this.Init_Check_SN = true;
                //yoyoConst.VERSION = "1111111111";




                //yoyoConst.EQU_SN = "KJ103IS00000211";
                //this.Init_Check_SN = true;
                //this.Init_Check_Device1 = true;

                this.Init_Check_Main = this.Init_Check_Net && this.Init_Check_Device1
                    && this.Init_Check_Device2 && this.Init_Check_SN;

                LogisTrac.WriteLog("kangjiamain--网络状态" + this.Init_Check_Net);
                LogisTrac.WriteLog("kangjiamain--串口状态" + this.Init_Check_Device1);
                //LogisTrac.WriteLog("kangjiamain--自检状态" + this.Init_Check_Device2);
                LogisTrac.WriteLog("kangjiamain--SN获取状态" + this.Init_Check_SN);
                if (this.Init_Check_Main && !Init_Check_Maining && !Init_Device_doing)
                {
                    // devicemanager.Write(new Cmd_S_Success());
                    Init_Check_Maining = true;

                    bool flag = this.update_Main();//判断是否需要升级

                    if (!Init_Device_doing)
                    {
                        update_start();
                    }
                    Init_Check_Maining = false;
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        //初始化失败后定时器调用
        private void init_Tick(object sender, EventArgs e)
        {
            try
            {
                totalcount--;
                if (totalcount > 0)
                {
                    LogisTrac.WriteLog("main倒数第" + totalcount + "次,尝试启动");
                    Init_Main();
                }
                else
                {
                    Init_timer.Enabled = false;//停止自动检测
                    LogisTrac.WriteLog("main无法启动,停止启动线程");
                    
                    update_start();
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        /// <summary>
        /// 枚举win32 api
        /// </summary>
        public enum HardwareEnum
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity,//all device
        }

        public static string[] MulGetHardwareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value != null)
                        {
                            if (hardInfo.Properties[propKey].Value.ToString().Contains("COM"))
                            {
                                strs.Add(hardInfo.Properties[propKey].Value.ToString());
                            }
                        }
                    }
                    searcher.Dispose();
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            { strs = null; }
        }

        public void SendData3()
        {
            if (this.serialPort1.IsOpen)
            {
                //发送数据
                SubSendData3();
                this.serialPort1.Close();
            }
            else
            {

                this.serialPort1.Open();

                if (this.serialPort1.IsOpen)
                { //发送数据
                    SubSendData3();
                    this.serialPort1.Close();
                }
                else
                { LogisTrac.WriteLog("串口打开失败"); }

            }
            this.serialPort1.Close();
        }

        private void SubSendData3()
        {
            this.serialPort1.DiscardInBuffer();
            this.serialPort1.Write("AT^MONSC\r");

            LogisTrac.WriteLog("开机先查询4g模块信息");

            Thread.Sleep(100);



            this.serialPort1.DiscardInBuffer();

            this.serialPort1.Write("AT^HCSQ?\r");

            Thread.Sleep(100);

            this.serialPort1.DiscardInBuffer();

            this.serialPort1.Write("AT^SYSINFOEX\r");

            Thread.Sleep(100);


            this.serialPort1.DiscardInBuffer();

            this.serialPort1.Write("AT^ICCID?\r");

            Thread.Sleep(100);
        }

        public void com_port(string com)
        {
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.PortName = com;
            this.serialPort1.DataBits = 8;
        }

        //判断4g版本
        private void Chenck_4g()
        {  //通过WMI获取COM端口
            string[] ss = MulGetHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name");

            int j = -1;

            for (int i = 0; i < ss.Length; i++)
            {
                string[] sArray = ss[i].Split('(');
                string comname = sArray[0];

                if (sArray[0] == "HUAWEI Mobile Connect - PC UI Interface ")//华为
                {
                    string[] sArray1 = sArray[1].Split(')');
                    com_4g = sArray1[0];

                    iweb_type = 1;
                    j = i;
                    break;
                }

                if (sArray[0] == "Quectel USB AT Port ")//移远
                {
                    string[] sArray1 = sArray[1].Split(')');
                    com_4g = sArray1[0];

                    iweb_type = 2;
                    j = i;
                    break;
                }

                if (comname == "Prolific USB-to-Serial Comm Port ")//测试
                {
                    string[] sArray1 = sArray[1].Split(')');
                    com_4g = sArray1[0];

                    iweb_type = 3;
                    j = i;
                    break;
                }
            }


            if (j != -1)
            {
                LogisTrac.WriteLog("4g模块名称：" + ss[j] + "  com: " + com_4g);
                // MessageBox.Show(ss[j]);
            }
            else
            {
                LogisTrac.WriteLog("判断4g模块版本错误,未找到匹配串口");
                // MessageBox.Show("no");
                return;
            }
        }
        //打开串口
        private bool Init_Com1()
        {
            try
            {
                ////打开串口
                devicemanager.PortName = OperateIniFile.ReadVersionIniData("COM_PORT");
                bool comcheck = devicemanager.Start();
                if (comcheck)
                {
                    //串口读取事件
                    devicemanager.CmdMngr.CommandReceivedHandler = new CommandManager.CommandReceivedDelegate(this.CommandReceivedDelegate);

                }
                Init_Check_Device1ing = false;
                return comcheck;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }
        }
        //自检
        private bool Init_Com2()
        {
            try
            {
                devicemanager.Write(new Cmd_S_Check());
                return false;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }
        }

        //初始化定时器
        private void Init_AllTimer()
        {
            try
            {

                //定时检查网络
                netcheckTimer = new System.Timers.Timer(5000);
                netcheckTimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => netcheckTimer_Tick(s, e));
                netcheckTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
                netcheckTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        #endregion

        #region 网络监测
        /// <summary>
        /// 间隔时间执行画面切换
        /// </summary>
        /// <param name="stepname"></param>
        /// <param name="time"></param>
        private void netcheck_delay()
        {
            try
            {
                if (Init_Check_Neting)
                {
                    return;
                }

                System.Timers.Timer t = new System.Timers.Timer(500);//实例化Timer类，设置时间间隔
                t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => netcheck_delay_fun(s, e));
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        private void netcheck_delay_fun(object sender, EventArgs e)
        {
            //检测多次网络是否畅通
            netcheck_fun();
        }

        private void netcheck_fun()
        {
            try
            {
                //检测多次网络是否畅通
                if (Init_Check_Neting)
                {
                    return;
                }
                int count = 5;
                bool flag = false;
                Init_Check_Neting = true;
                while (count > 0)
                {
                    count--;
                    flag = Internet.IsConnectInternet();
                    if (!flag)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        count = 0;
                    }
                }
                this.Init_Check_Net = flag;


                Init_Check_Neting = false;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        #endregion

        #region 串口方法
        //串口设备不同状态下与程序间通信
        private void CommandReceivedDelegate(Command cmd)
        {
            try
            {
                //if (cmd is Cmd_X_Check)
                //{
                //    #region 上位机请求下位机自检的协议
                //    //2.10	上位机请求下位机自检的协议
                //    //0x00：生物电绿光自检均不正常
                //    //0x01：生物电自检正常
                //    //0x02：绿光自检正常
                //    //0x03：生物电、绿光自检均正常

                //    if (cmd.GetData()[4] == 0x03)
                //    {
                //        this.Init_Check_Device1 = true;
                //    }
                //    this.Init_Check_Device2ing = false;
                //    #endregion
                //}
                //else
                if (cmd is Cmd_X_SN)
                {
                    #region 3.2	上位机查询下位机固件版本和SN的协议
                    byte[] list = cmd.GetData();
                    string x = "";
                    for (int i = 4; i < list.Length - 2; i++)
                    {
                        x += "," + list[i];
                    }
                    // Console.WriteLine("report--" + x);
                    //this.textBox2.Text = x;
                    byte[] list2 = DeviceHelper.Com_getSubData(list);

                    ASCIIEncoding encoder = new ASCIIEncoding();
                    string tmp = encoder.GetString(list2, 0, list2.Length);
                    //  this.textBox2.Text = tmp;
                    yoyoConst.VERSION = tmp.Substring(0, 17);//版本号
                    yoyoConst.EQU_SN = tmp.Substring(17, tmp.Length - 17);//设备信息（SN码）
                    if (!DeviceHelper.Com_checkSN())
                    {
                        devicemanager.Write(new Cmd_S_SN());
                        this.Init_Check_SN = false;
                        this.Init_Check_SNing = false;
                    }
                    else
                    {
                        this.Init_Check_SN = true;
                        this.Init_Check_SNing = false;
                    }

                    #endregion
                }

                else if (cmd is Cmd_X_GetNew)
                {
                    //3.4	下位机查向上位机请求新版固件信息的协议

                    //temp.hardLength = FileHelper.FileSize(testuploadfile);
                    if (string.IsNullOrEmpty(yoyoConst.EQU_SN) && cmdnew.hardLength <= 0)
                    {
                        bool flag = cmdReset();
                        if (flag)
                        {
                            devicemanager.Write(cmdnew);
                        }
                    }
                    else if (!string.IsNullOrEmpty(yoyoConst.EQU_SN))
                    {
                        devicemanager.Write(cmdnew);
                    }
                }
                else if (cmd is Cmd_X_GetData)
                {
                    if (string.IsNullOrEmpty(yoyoConst.EQU_SN) && cmdnew.hardLength <= 0)
                    {
                        bool flag = cmdReset();
                    }
                    else
                    {
                        #region 3.5	下位机向上位机请求固件数据的协议
                        // string x = cmd.GetData()[4].ToString();
                        byte[] cmddata = cmd.GetData();
                        cmd.show(cmddata);
                        byte[] intBuff = new byte[2];
                        intBuff[0] = cmddata[5];
                        intBuff[1] = cmddata[4];
                        int pos = BitConverter.ToInt16(intBuff, 0);//取得包数
                        int start = pos * testuploadbaselength;
                        int end = (start + testuploadbaselength) >= testuploadByte.Length ? testuploadByte.Length : (start + testuploadbaselength);

                        Cmd_S_GetData data = new Cmd_S_GetData();
                        data.packagePos = pos;
                        data.package = new byte[end - start];
                        int packagepos = 0;
                        for (int i = start; i < end; i++)
                        {
                            data.package[packagepos] = testuploadByte[i];
                            packagepos++;
                        }
                        devicemanager.Write(data);
                        #endregion
                    }
                }
                else if (cmd is Cmd_X_Success)
                {
                    //3.6	下位机告知上位机固件下载成功的协议
                    devicemanager.Write(new Cmd_S_Success());
                }
                else if (cmd is Cmd_X_UpdateSuccess)
                {
                    //3.7	下位机通知上位机升级成功的协议
                    devicemanager.Write(new Cmd_S_UpdateSuccess());
                    //更新为最新版本
                    FileHelper.DeleteFolder(this.up_mcpVersion_url);
                    Init_Device_doing = false;
                    LogisTrac.WriteLog("固件文件更新成功,版本" + model.data.mcpVersion);
                    update_start();
                    //flash_change2(yoyoConst.YoyoStep.version);
                }
            }
            catch (Exception ex)
            {
                yoyoConst.WriteLog(ex.ToString());
            }
        }
        private bool cmdReset()
        {
            //升级过程中异常中断的时候处理
            string filename = FileHelper.getFilename(this.up_mcpVersion_url, ".bin");
            if (!string.IsNullOrEmpty(filename))
            {
                FileInfo file = new FileInfo(filename);
                yoyoConst.VERSION = file.Name.Substring(0, 17);
                testuploadByte = FileHelper.GetFileData(filename);
                cmdnew.hardLength = FileHelper.FileSize(filename);
                return true;
            }
            else
            {
                LogisTrac.WriteLog("kangjiamain--固件升级文件不是bin文件");
                return false;
            }
        }
        #endregion

        #region 定时器

        /// <summary>
        /// 定时检查网络
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void netcheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                netcheck_fun();
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        #endregion

        #region http方法

        /// <summary>
        /// 设备升级
        /// </summary>
        private retV1UploadU01 htt_Upload()
        {

            try
            {
                string url = "?token=" + htConst.getToken();
                string parurl = htConst.url_V1_UPLOAD + url;

                htV1UploadU01 req = new htV1UploadU01();
                req.deviceSN = yoyoConst.EQU_SN;
                req.timeStamp = DateTime.Now.ToString("yyyyMMddHHssmm");

                string ret = htConst.Post(parurl, req);

                //注册成功
                retV1UploadU01 model = JsonHelper.FromJSON<retV1UploadU01>(ret);
                if (model != null && model.data != null && model.success == true)
                {

                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        #endregion

        #region 升级
        /// <summary>
        /// 检测升级
        /// </summary>
        private bool update_Main()
        {
            try
            {
                LogisTrac.WriteLog("kangjiamain--开始检查升级状态");
                string basth = yoyoConst.KANGJIA_PATH;

                model = htt_Upload();

                bool doflag = false;
                if (model == null)
                {
                    LogisTrac.WriteLog("kangjiamain--无法连接服务判断");
                    return true;
                }
                string resVersion = OperateIniFile.ReadVersionIniData("RES_VERSION");
                string appVersion = OperateIniFile.ReadVersionIniData("MAIN_VERSION");
                string mcpVersion = OperateIniFile.ReadVersionIniData("HARD_VERSION");
                if (model.data.resVersion != null && resVersion != null && model.data.resVersion.Trim() != resVersion.Trim())
                {
                    //下载解压
                    doflag = update_downloadAndUnZipFile(model.data.resUrl, this.up_resVersion_url);
                    if (!doflag)
                    {
                        LogisTrac.WriteLog("kangjiamain--资源文件下载解压失败,版本" + model.data.resVersion);
                        return doflag;
                    }
                    LogisTrac.WriteLog("kangjiamain--资源文件下载解压成功" + resVersion + "end");//model.data.
                    //复制到指定目录，删除原始文件
                    doflag = update_replaceFile(this.up_resVersion_url, basth + "\\res");
                    if (!doflag)
                    {
                        LogisTrac.WriteLog("kangjiamain--资源文件替换失败,版本" + model.data.resVersion + "end");
                        return doflag;
                    }
                    FileHelper.DeleteFolder(this.up_resVersion_url);
                    OperateIniFile.WriteVersionIniData("RES_VERSION", model.data.resVersion);
                    LogisTrac.WriteLog("kangjiamain--资源文件更新成功,版本" + model.data.resVersion + "end");

                }
                if (model.data.appVersion != null && appVersion != null && model.data.appVersion.Trim() != appVersion.Trim())
                {
                    //下载解压
                    doflag = update_downloadAndUnZipFile(model.data.appUrl, this.up_appVersion_url);
                    if (!doflag)
                    {
                        LogisTrac.WriteLog("kangjiamain--主程序文件下载解压失败,版本" + model.data.appVersion + "end");
                        return doflag;
                    }
                    //复制到指定目录，删除原始文件
                    doflag = update_replaceFile(this.up_appVersion_url, basth);
                    if (!doflag)
                    {
                        LogisTrac.WriteLog("kangjiamain--主程序文件替换失败,版本" + model.data.appVersion + "end");
                        return doflag;
                    }
                    FileHelper.DeleteFolder(this.up_appVersion_url);
                    OperateIniFile.WriteVersionIniData("MAIN_VERSION", model.data.appVersion);
                    LogisTrac.WriteLog("kangjiamain--主程序文件更新成功,版本" + model.data.appVersion + "end");

                }
                if (model.data.mcpVersion.Length <= 9 || yoyoConst.VERSION.Length <= 9)
                {
                    return false;
                }
                //if (model.data.mcpVersion != yoyoConst.VERSION.Substring(9))//TODO

                string[] list1 = new string[2];//yoyoConst.VERSION.Split('H');
                list1[0] = yoyoConst.VERSION.Substring(0, 9);
                list1[1] = yoyoConst.VERSION.Substring(9);

                string[] list2 = new string[2];//model.data.mcpVersion.Split('H');
                list2[0] = model.data.mcpVersion.Trim().Substring(0, 9);
                list2[1] = model.data.mcpVersion.Trim().Substring(9);

                if (list1.Length == 2 && list1.Length == list2.Length && list1[0] != list2[0] && list1[1] == list2[1])
                {
                    //下载解压
                    doflag = update_downloadAndUnZipFile(model.data.mcpUrl, this.up_mcpVersion_url);
                    if (!doflag)
                    {
                        LogisTrac.WriteLog("kangjiamain--固件文件下载解压失败,版本" + model.data.mcpVersion + "end");
                        return doflag;
                    }
                    string filename = FileHelper.getFilename(this.up_mcpVersion_url, ".bin");

                    if (!string.IsNullOrEmpty(filename))
                    {
                        Init_Device_doing = true;
                        testuploadByte = FileHelper.GetFileData(filename);
                        cmdnew.hardLength = FileHelper.FileSize(filename);
                        devicemanager.Write(new Cmd_S_Ready());
                    }
                    else
                    {
                        LogisTrac.WriteLog("kangjiamain--固件升级文件不是bin文件");
                        return true;
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }

        }

        private bool update_Unzip(string filename, string tagetpath)
        {
            try
            {
                ExtractOptions option = ExtractOptions.None;
                //option = ExtractOptions.None;
                //option = ExtractOptions.ExtractFullPath;

                var archive = ArchiveFactory.Open(filename);
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(tagetpath, option | ExtractOptions.Overwrite);
                    }
                }
                archive.Dispose();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 先把需要更新的文件下载到本地,并且解压
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool update_downloadAndUnZipFile(string url, string tagetpath)
        {
            try
            {
                String onlyFileName = Path.GetFileName(url);
                //下载
                bool falg = FtpUpLoadFiles.Download(url, tagetpath, onlyFileName);
                if (!falg)
                {
                    yoyoConst.WriteLog("文件下载失败" + url);
                }
                //TODO解压
                string name = tagetpath + onlyFileName;
                falg = update_Unzip(name, tagetpath);
                if (falg)
                {
                    bool del = true;
                    while (del)
                    {
                        try
                        {
                            File.Delete(name);//解压后删除
                            del = false;
                        }
                        catch
                        {
                            Thread.Sleep(200);
                        }
                    }
                }
                return falg;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }
        }

        //复制或覆盖到指定文件夹
        private bool update_replaceFile(string sourcePath, string destPath)
        {
            try
            {
                if (!Directory.Exists(sourcePath))
                {
                    return false;
                }
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建  
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        yoyoConst.WriteLog("创建目标目录失败" + destPath);
                        return false;
                    }
                }
                //获得源文件下所有文件  
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(destPath, Path.GetFileName(c));
                    //覆盖模式  
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    File.Move(c, destFile);
                });
                //获得源文件下所有目录文件  
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(destPath, Path.GetFileName(c));
                    //Directory.Move必须要在同一个根目录下移动才有效，不能在不同卷中移动。  
                    //Directory.Move(c, destDir);  
                    //采用递归的方法实现  
                    update_replaceFile(c, destDir);
                });

            }
            catch
            {
                return false;
            }
            return true;
        }

        private void update_start()
        {
            try
            {
                this.devicemanager.Stop();
                this.devicemanager = null;
                // 升级完成，启动主程序
                string StrExe = yoyoConst.KANGJIA_PATH + "kangjia.exe";
                LogisTrac.WriteLog(StrExe);
                if (File.Exists(StrExe))
                {
                    System.Diagnostics.Process.Start(StrExe);
                }
                this.Hide();
                //关闭升级程序
                System.Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }


        #endregion

        #region flash相关方法
        /// <summary>
        /// 触发flash声音提示
        /// </summary>
        /// <param name="o"></param>
        private void flash_Play(object o)
        {
            try
            {
                string url = (string)o;
                axShockwaveFlash1.CallFunction(url);
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        /// <summary>
        /// 播放按钮声音
        /// </summary>
        private void flash_playButton()
        {
            try
            {
                string flash = "<invoke name=\"button\" returntype=\"xml\"></invoke>";
                Thread tt = new Thread(flash_Play);
                tt.Start(flash);
                tt.Join();
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        /// <summary>
        /// 切换flash
        /// </summary>
        /// <param name="step"></param>
        /// <param name="para"></param>
        private void flash_change2(yoyoConst.YoyoStep step, params string[] para)
        {
            try
            {
                if (yoyoConst.currtenStep != null && step == yoyoConst.currtenStep.step)
                {
                    //首页点击按钮无效
                    return;
                }

                flashmodel mode = new flashmodel();
                bool falg = yoyoConst.dict.TryGetValue(step, out mode);
                yoyoConst.currtenStep = mode;
                string url = mode.url;
                if (para != null && para.Length > 0)
                {
                    url = String.Format(mode.url, para[0]);
                }
                if (falg)
                {

                    if (yoyoConst.currtenStep != null)
                    {
                        if (!string.IsNullOrEmpty(url))
                        {
                            try
                            {
                                axShockwaveFlash1.CallFunction(url);
                            }
                            catch
                            {
                                flash_change2(yoyoConst.YoyoStep.yoyo);
                                return;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        #endregion


    }
}
