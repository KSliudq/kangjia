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
using Camera_NET;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using MqttLib;
using System.Net.NetworkInformation;
using System.Collections;
using System.Diagnostics;
using kangjiabase;
using System.Runtime.InteropServices;
using SharpCompress.Archive;
using SharpCompress.Common;
using System.Security;
using Spire.Pdf;
using kangjiabase.http.model;
using System.IO.Ports;
using System.Management;
using System.Drawing.Drawing2D;

namespace kangjia
{
    public partial class indexForm2 : Form
    {
        int a = 0;
        #region 变量old3
        #region 摄像头变量
        CameraChoice _CameraChoice = new CameraChoice();
        //是否正在人脸识别
        private bool Face_checking = false;
        //人脸识别异常
        private bool Face_Error = false;
        private bool Face_Error_doing = false;
        private int Face_Time_out_count = 10;
        private int Face_Time_out_count_check = 0;

        private bool Camera_change = false;
        private int ichange = 0;
        #endregion

        #region 初始化变量
        System.Timers.Timer Init_timer = null;//初始化时检测是否成功,如果不成功循环检测
        int totalcount = (1000 * 5 * 60) / 2000;//5分钟/2秒
        bool Init_Check_Main = false;//判断初始化是否成功,不成功则继续运行线程,否则停止线程
        bool Init_Check_Maining = false;
        bool Init_Check_Device1 = false;//串口打开
        bool Init_Check_Device1ing = false;
        bool Init_Check_Device2 = false;//设备是否初始化成功
        // bool Init_Check_Device2ing = false;
        bool Init_Check_SN = false;//是否获取SN
        //  bool Init_Check_SNing = false;
        bool Init_Check_Camera = false;//摄像头初始化是否成功
        bool Init_Check_Camera2 = false;//摄像头初始化是否成功
        bool Init_Check_Cameraing = false;
        bool Init_Check_Cameraing2 = false;

        bool Init_Check_Net = false;//网络是否初始化成功
        bool Init_Check_Neting = false;
        bool Init_Check_Register = false;//设备是注册成功
        bool Init_Check_Registering = false;

        int iweb_type = 0;//4g模式  1华为 2移远
        string com_4g = "";//4g模块端口号
        string Cell_ID = "";
        string MCC = "";
        string MNC = "";
        string LAC = "";

        string YYS = "";//运营商
        string CARD_NO = "";



        bool Init_Check_Long = false;//长连接
        bool Init_Check_Longing = false;//长连接
        bool Init_check_log_uploading = false;


        bool Init_Check_equState = false;//开机状态
        bool Init_Check_equStateing = false;

        bool Init_Check_ComputerTime = false;//电脑时间
        bool Init_Check_ComputerTimeing = false;//电脑时间

        bool Init_Check_equState_lock = false;
        bool Init_Check_equState_locking = false;
        bool Init_Finish = false;//初始化是否完成

        bool Init_Check_update = false;//是否更新kangjiamain
        bool Init_Check_updating = false;
        private string NowDate = DateTime.Now.ToString("yyyyMMdd");
        #endregion

        #region 串口变量
        public SerialPort serialPort1 = new SerialPort();//定义一个串口类的串口变量 

        private DeviceManager devicemanager = new DeviceManager();
        private static object comLockObj = new object();//防止并发
        private string COM_WAIT_TXT = @"tempfile";
        private bool Testing = false;//是否测试中
        private bool SEND_REPORT = false;//是否发送报告

        private string REPORT_id = "1";//报告id  用于生成二维码
        #endregion

        #region 定时器变量
        System.Timers.Timer indexformtimer = new System.Timers.Timer(yoyoConst.INDEX_TIME);//实例化Timer类，设置时间间隔
        System.Timers.Timer uploadresulttimer = new System.Timers.Timer(yoyoConst.INDEX_TIME);//实例化Timer类，设置文件上传线程
        System.Timers.Timer netcheckTimer = new System.Timers.Timer(1000);//实例化Timer类，验证网络是否畅通
        System.Timers.Timer linkTimer;//实例化Timer类，长连接待执行命令
        System.Timers.Timer handleaveTimer = new System.Timers.Timer(1000);//实例化Timer类，双手离开时间返回主画面
        System.Timers.Timer facetimer = new System.Timers.Timer(1000);//定时检测人脸
        System.Timers.Timer restartnettimer;//重启网络线程
        private bool FLASH_CHANGING = false;//画面是否切换中
        private bool LINK_DOING = false;//长连接线程是否处理中
        private int HAND_LEAVE = 0;//双手离开计时器
        private bool HAS_FACE = false;//是否有人脸
        private int NET_LOG_COUNT = 12;//输出日志计时器
        private bool NET_LOG_ERROR = false;//是否存在网络异常,日志输出用
        private int FACE_CHECK = 10;//人脸检测10秒钟不存在则退出测量
        private int FACE_CHECK_COUNT = 30;//检测次数 FACE_CHECK_COUNT =  OperateIniFile.ReadIniData("FACE_CHECK");
        private int IPRINT = 0;
        #endregion

        #region 长连接

        string net_flash;
        IMqtt _client;
        string stopic = "";//订阅主题  用 , 分隔
        string ptopic = "";//发布主题
        List<string> LinkList = new List<string>();//待执行远程设备命令
        #endregion

        #region 升级变量
        private string up_resVersion_url = yoyoConst.KANGJIA_MAIN_PATH + "\\backup\\kangjiamainVersion\\";
        #endregion

        #endregion

        #region 构造函数load
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern int ShowCursor(bool bShow);

        public indexForm2()
        {
            InitializeComponent();
            this.TopMost = false;//是否在最前
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panlversion.Location = new Point(0, 0);
            this.panlversion.Size = new System.Drawing.Size(60, 70);
            #region 初始化控件
            cameraControl.Visible = false;
            //设定画面可以运行线程
            CheckForIllegalCrossThreadCalls = false;
            #endregion
            //初始化画面,默认运行starting.swf
            this.axShockwaveFlash1.Movie = Application.StartupPath + "\\res\\yoyo.swf";
            this.axShockwaveFlash1.Play();
        }

        private void indexForm2_Load(object sender, EventArgs e)
        {
            //restartNet();
            Init_Camera_Timer.Enabled = true;
            ShowCursor(false);//隐藏鼠标
            lblversion.Text = OperateIniFile.ReadIniData("MAIN_VERSION") + "\n";
            lblversion.Text += OperateIniFile.ReadVersionIniData("MAIN_VERSION") + "\n";
            lblversion.Text += OperateIniFile.ReadVersionIniData("RES_VERSION") + "\n";


            //liudq2018 08 18
            string flash = "<invoke name=\"show_version_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_version") + "' color='" + OperateIniFile.ReadIniData("color_version") + "'>" + OperateIniFile.ReadIniData("MAIN_VERSION") + "\r  " + OperateIniFile.ReadVersionIniData("MAIN_VERSION") + "\r  " + OperateIniFile.ReadVersionIniData("RES_VERSION") + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_version") + "</number><number>" + OperateIniFile.ReadIniData("y_version") + "</number><number>" + OperateIniFile.ReadIniData("width_version") + "</number><number>" + OperateIniFile.ReadIniData("height_version") + "</number></arguments></invoke>";
            axShockwaveFlash1.CallFunction(flash);
            LogisTrac.WriteLog("版本flash：" + flash);


            Chenck_4g();

            if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则定位
            {
                com_port(com_4g);
                SendData3();
            }




            //初始化摄像头图片位置
            Init_PicturePosion();

            string path = yoyoConst.KANGJIA_PATH + "Unknown.log";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Init_timer = new System.Timers.Timer(2000);
            Init_timer.Elapsed += new System.Timers.ElapsedEventHandler((s, ex) => init_Tick(s, ex));
            Init_timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            Init_timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  

            //定时执行长连接命令
            linkTimer = new System.Timers.Timer(1000);
            linkTimer.Elapsed += new System.Timers.ElapsedEventHandler((s, ex) => LinkTimer_Tick(s, ex));
            linkTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            linkTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件 


            //定时执行重启网卡命令,网络异常时启动命令
            restartnettimer = new System.Timers.Timer(1000 * 60 * 2);//2分钟
            restartnettimer.Elapsed += new System.Timers.ElapsedEventHandler((s, ex) => restartnettimer_Tick(s, ex));
            restartnettimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            restartnettimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件 


            //第一次调用初始化方法,如果成功则画面到广告页,否则启动线程间隔时间再调用初始化
            //Thread Init_thread = new Thread(Init_Main);
            //Init_thread.Start();
            //开始验证摄像头,摄像头必须放到控件线程中,否则无法初始化

            //yoyoConst.EQU_SN = "KJ103IS00000211";
            ////yoyoConst.EQU_SN = "KJ104IS0004234C";
            //this.Init_Check_SN = true;
            //this.Init_Check_Register = true;
            //Init_Check_Device1 = true;
            //Init_Check_Device2 = true;
            ////this.Init_Check_updating = true;

            // LinkList.Add("A007");
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
                try
                {
                    FACE_CHECK_COUNT = Convert.ToInt32(OperateIniFile.ReadIniData("FACE_LEAVE_TIME"));
                }
                catch { }

                try
                {
                    IPRINT = Convert.ToInt32(OperateIniFile.ReadIniData("PRINT"));
                }
                catch { }

                if (FACE_CHECK_COUNT < 15)
                { FACE_CHECK_COUNT = 15; }




                //Chenck_4g();

                //if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则定位
                //{
                //    com_port(com_4g);
                //    SendData3();
                //}




                //网络检测
                if (!this.Init_Check_Net)
                {
                    netcheck_delay();
                }

                //if (!this.Init_Check_updating && this.Init_Check_Net && htConst.checkEQUSN("设备升级"))
                //{
                //    Init_Check_updating = true;
                //    LogisTrac.WriteLog("检测kangjiamain更新----开始");
                //    Init_Check_update = this.update_kangjiamain();
                //    LogisTrac.WriteLog("检测kangjiamain更新----结束");

                //    //判断需要升级kangjiamain则升级后重新启动
                //    if (Init_Check_update)
                //    {
                //        Init_timer.Enabled = false;//停止自动检测
                //        update_start();
                //        return;
                //    }
                //    //Init_Check_updating = false; //只执行一次
                //}
                //if (Init_Check_updating)
                //{
                //    return;
                //}



                //硬件检测
                if (!this.Init_Check_Device1)
                {
                    if (!Init_Check_Device1ing)
                    {
                        Init_Check_Device1ing = true;
                        this.Init_Check_Device1 = Init_Com1();
                        Init_Check_Device1ing = false;
                    }
                }

                if (this.Init_Check_Device1 && !this.Init_Check_Device2)
                {
                    devicemanager.Write(new Cmd_S_Check());
                    // this.Init_Check_Device2 = true;//TODO
                }

                if (this.Init_Check_Device1 && this.Init_Check_Device2 && !this.Init_Check_SN)
                {
                    //获取设备编号
                    devicemanager.Write(new Cmd_S_SN());
                }

                if (!this.Init_Check_Register)
                {
                    if (!this.Init_Check_Registering)
                    {
                        this.Init_Check_Registering = true;
                        this.Init_Check_Register = htt_Register();
                        this.Init_Check_Registering = false;
                    }
                }

                if (Init_Check_Net && !Init_Check_Long)
                {
                    if (!Init_Check_Longing)
                    {
                        //长连接设定
                        Init_Check_Longing = true;
                        Init_Check_Long = htt_long();
                        Init_Check_Longing = false;
                    }
                }
                if (!Init_Check_equState)
                {
                    if (!Init_Check_equStateing)
                    {
                        Init_Check_equStateing = true;
                        //上报开机状态
                        retIFI02 model = htt_equState(htConst.YoyoEqu.start_state);
                        if (model != null)
                        {
                            Init_Check_equState = true;
                        }
                        Init_Check_equStateing = false;
                    }
                }
                if (!Init_Check_ComputerTime)
                {
                    if (!Init_Check_ComputerTimeing)
                    {
                        Init_Check_ComputerTimeing = true;
                        //设定电脑时间
                        Init_Check_ComputerTime = Init_ComputerTime();
                        Init_Check_ComputerTimeing = false;
                    }
                }
                if (Init_Check_Net && this.Init_Check_SN && !Init_Check_equState_lock)
                {
                    if (!Init_Check_equState_locking)
                    {
                        Init_Check_equState_locking = true;
                        //上报锁屏状态
                        retIFI02 model = htt_equState(htConst.YoyoEqu.lock_state);
                        if (model != null)
                        {
                            Init_Check_equState_lock = true;
                        }
                        Init_Check_equState_locking = false;
                    }
                }

                this.Init_Check_Main = this.Init_Check_Camera && Init_Check_Camera2
                    && this.Init_Check_Net && this.Init_Check_Device1
                    && this.Init_Check_Device2 && this.Init_Check_SN
                    && this.Init_Check_Register && this.Init_Check_Long
                    && this.Init_Check_equState && this.Init_Check_equState_lock
                    && this.Init_Check_ComputerTime;

                LogisTrac.WriteLog("==============================");
                LogisTrac.WriteLog("网络状态" + this.Init_Check_Net);
                LogisTrac.WriteLog("串口状态" + this.Init_Check_Device1);
                LogisTrac.WriteLog("自检状态" + this.Init_Check_Device2);
                LogisTrac.WriteLog("SN获取状态" + this.Init_Check_SN);

                LogisTrac.WriteLog("照相机状态" + this.Init_Check_Camera2);
                LogisTrac.WriteLog("注册状态" + this.Init_Check_Register);
                LogisTrac.WriteLog("长连接" + this.Init_Check_Long);
                LogisTrac.WriteLog("上报开机状态" + this.Init_Check_equState);
                LogisTrac.WriteLog("上报锁屏状态" + this.Init_Check_equState_lock);
                LogisTrac.WriteLog("设置电脑时间" + this.Init_Check_ComputerTime);


                //if (!this.Init_Check_Long)
                //{
                //    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "长连接失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                //    axShockwaveFlash1.CallFunction(flash);
                //}

                //if (!this.Init_Check_Camera2)
                //{
                //    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "加载摄像头失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                //    axShockwaveFlash1.CallFunction(flash);
                //}

                //if (!this.Init_Check_Device2)
                //{
                //    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "自检失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                //    axShockwaveFlash1.CallFunction(flash);
                //}

                //if (!this.Init_Check_Device1)
                //{
                //    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "串口状态失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                //    axShockwaveFlash1.CallFunction(flash);
                //}


                //if (!this.Init_Check_Net)
                //{
                //    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "网络连接失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                //    axShockwaveFlash1.CallFunction(flash);
                //}



                //this.Init_Check_Main = this.Init_Check_Camera && Init_Check_Camera2
                //    && this.Init_Check_Net && this.Init_Check_SN && this.Init_Check_Long
                //    && this.Init_Check_equState && this.Init_Check_equState_lock ;
                if (this.Init_Check_Main && !Init_Check_Maining)
                {

                    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "AAA" + "<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                    axShockwaveFlash1.CallFunction(flash);


                    if (!this.Init_Check_updating && this.Init_Check_Net && htConst.checkEQUSN("设备升级"))
                    {
                        string flash1 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "检测升级..." + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash1);

                        Init_Check_updating = true;
                        LogisTrac.WriteLog("检测kangjiamain更新----开始");
                        Init_Check_update = this.update_kangjiamain();
                        LogisTrac.WriteLog("检测kangjiamain更新----结束");

                        string flash2 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "AAA" + "<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash2);

                        //判断需要升级kangjiamain则升级后重新启动
                        if (Init_Check_update)
                        {
                            Init_timer.Enabled = false;//停止自动检测
                            update_start();
                            return;
                        }
                        //Init_Check_updating = false; //只执行一次
                    }

                    Init_Check_Maining = true;
                    Init_Camera_Timer.Enabled = false;
                    //上报开机状态，上网途径TODO
                    //htt_netrootCheck();




                    //上传日志
                    log_upload_delay();

                    ////开启面部识别检测             
                    //face_takePic = new Face_TakePic_delegate(Face_TakePicMethod);
                    //Thread objThread = new Thread(new ThreadStart(delegate { Face_ThreadMethodPic(); }));
                    //objThread.Start();

                    //定时器启动
                    Init_AllTimer();

                    flash_change(yoyoConst.YoyoStep.yoyo);

                    Init_Check_Maining = false;

                    Init_timer.Enabled = false;//停止自动检测
                    Init_Finish = true;


                    //判断4g模块版本  liudq
                    Chenck_4g();

                    if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则定位
                    {

                        {
                            // this.locationing = true;

                            com_port(com_4g);

                            int i = 0;
                            while (i < 60)
                            {
                                SendData();

                                if (MCC == "")
                                {
                                    i++;
                                    Thread.Sleep(5000);
                                }
                                else
                                { i = 60; }

                            }

                            if (MCC == "")
                            {
                                LogisTrac.WriteLog("获取基站信息失败");
                                return;
                            }

                            //网络配置信息
                            htt_WEBinfo();

                            //上传基站信息
                            htt_location();

                            // this.locationing = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                // LogisTrac.WriteLog(ex.Message);
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
                    LogisTrac.WriteLog("倒数第" + totalcount + "次,尝试启动");
                    Init_Main();
                }
                else
                {   //liudq 2018 08 28

                    //判断4g模块
                    Chenck_4g();

                    if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则定位
                    {
                        com_port(com_4g);

                    }

                    //启动定时返回
                    Init_INDEX_TIME_Timer();

                    if (!this.Init_Check_Long)
                    {
                        string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "长连接失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);
                    }

                    if (!this.Init_Check_Camera2)
                    {
                        string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "加载摄像头失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);
                    }

                    if (!this.Init_Check_Device2)
                    {
                        string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "自检失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);
                    }

                    if (!this.Init_Check_Device1)
                    {
                        string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "串口状态失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);
                    }


                    if (!this.Init_Check_Net)
                    {
                        string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "网络连接失败" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);

                        SendData2();
                    }

                    Init_timer.Enabled = false;//停止自动检测
                    LogisTrac.WriteLog("无法启动,停止启动线程");


                    string url = yoyoConst.SERVER_IP.Replace("http://", "").Replace("/", "");

                    if (url.IndexOf(":") > 0)
                    {
                        url = url.Substring(0, url.IndexOf(":"));
                    }

                    if (Internet.RunCmd_Ping(url))//IsConnectInternet
                    {
                        LogisTrac.WriteLog("网络畅通,但无法启动");
                        log_upload(DateTime.Now);
                    }
                    else
                    {
                        LogisTrac.WriteLog("网络异常,无法启动");
                        flash_change(yoyoConst.YoyoStep.connect_error);
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void Init_Camera_Main_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!this.Init_Check_Camera)
                {
                    this.Init_Check_Camera = Init_Camera1();
                }
                if (this.Init_Check_Camera && !this.Init_Check_Camera2)
                {
                    this.Init_Check_Camera2 = Init_Camera2();
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private bool Init_Camera1()
        {

            if (!Init_Check_Cameraing)
            {
                Init_Check_Cameraing = true;
                try
                {

                    _CameraChoice.UpdateDeviceList();
                    // To get an example of camera and resolution change look at other code samples 
                    if (_CameraChoice.Devices.Count > 0)
                    {
                        Init_Check_Cameraing = false;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    yoyoConst.WriteLog(ex.ToString());
                }
                Init_Check_Cameraing = false;
            }
            return false;
        }
        // 摄像头验证,初始化
        private bool Init_Camera2()
        {
            if (!Init_Check_Cameraing2)
            {
                Init_Check_Cameraing2 = true;
                try
                {

                    // Get List of devices (cameras)
                    if (!cameraControl.CameraCreated)
                    {

                        _CameraChoice.UpdateDeviceList();
                        // To get an example of camera and resolution change look at other code samples 
                        if (_CameraChoice.Devices.Count > 0)
                        {
                            // Run first camera if we have one
                            // var camera_moniker = _CameraChoice.Devices[0].Mon;
                            //cameraControl.SetCamera(_CameraChoice.Devices[0].Mon, null);
                            //// Set selected camera to camera control with default resolution
                            //Resolution resolution = new Resolution(320, 240);
                            //ResolutionList resolutions = Camera.GetResolutionList(cameraControl.Moniker);

                            //for (int index = 0; index < resolutions.Count; index++)
                            //{
                            //    //Resolution resolution = new Resolution(1024, 576);
                            //    if (resolutions[index].Width == CAMERA_width)
                            //    {
                            //        resolution = resolutions[index];
                            //        break;
                            //    }
                            //}
                            //cameraControl.SetCamera(cameraControl.Moniker, resolution);

                            var camera_moniker = _CameraChoice.Devices[0].Mon;
                            Resolution resolution = new Resolution(320, 240);
                            cameraControl.SetCamera(camera_moniker, resolution);
                            Init_Check_Cameraing2 = false;
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    yoyoConst.WriteLog(ex.ToString());
                }
                Init_Check_Cameraing2 = false;
            }
            return false;
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



        public void com_port(string com)
        {
            this.serialPort1.BaudRate = 115200;
            this.serialPort1.PortName = com;
            this.serialPort1.DataBits = 8;
        }

        //定位
        public void SendData()
        {
            if (this.serialPort1.IsOpen)
            {
                //发送数据
                SubSendData();
                this.serialPort1.Close();
            }
            else
            {

                this.serialPort1.Open();

                if (this.serialPort1.IsOpen)
                { //发送数据
                    SubSendData();
                    this.serialPort1.Close();
                }
                else
                { LogisTrac.WriteLog("串口打开失败"); }

            }
            this.serialPort1.Close();
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
            this.serialPort1.Write("AT^CURC=0\r");

            LogisTrac.WriteLog("关闭信号值主动上报");

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

        private void SubSendData()
        {


            //运营商
            this.serialPort1.DiscardInBuffer();

            this.serialPort1.Write("AT+COPS?\r");
            LogisTrac.WriteLog("串口指令：AT+COPS?\r");

            Thread.Sleep(1000);

            string indata2 = this.serialPort1.ReadExisting();

            string l_strResult2 = indata2.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

            LogisTrac.WriteLog("串口返回值：" + l_strResult2);

            string[] sArray2 = l_strResult2.Split((new char[1] { ',' }));

            for (int index = 0; index < sArray2.Length; index++)
            {
                if ((index == 2))
                { YYS = sArray2[index].ToString(); }
            }



            if (iweb_type == 1)
            {
                //卡号
                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT^ICCID?\r");
                LogisTrac.WriteLog("串口指令：AT^ICCID?\r");

                Thread.Sleep(1000);

                string indata1 = this.serialPort1.ReadExisting();

                string l_strResult1 = indata1.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "").Replace("OK", "");

                LogisTrac.WriteLog("串口返回值：" + l_strResult1);

                string[] sArray1 = l_strResult1.Split((new char[1] { ':' }));

                for (int index = 0; index < sArray1.Length; index++)
                {
                    if ((index == 1))
                    { CARD_NO = sArray1[index].ToString(); }
                }




                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT^MONSC\r");
                LogisTrac.WriteLog("串口指令：AT^MONSC\r");

                Thread.Sleep(1000);

                string indata = this.serialPort1.ReadExisting();

                string l_strResult = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值：" + l_strResult);

                string[] sArray = l_strResult.Split((new char[1] { ',' }));

                for (int index = 0; index < sArray.Length; index++)
                {
                    if ((index == 1))
                    { MCC = sArray[index].ToString(); }

                    if ((index == 2))
                    { MNC = sArray[index].ToString(); }

                    if ((index == 4))
                    { Cell_ID = Convert.ToInt32(sArray[index], 16).ToString(); }

                    if ((index == 6))
                    { LAC = Convert.ToInt32(sArray[index], 16).ToString(); }
                }
            }

            if (iweb_type == 2)
            {
                //卡号
                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT+ICCID\r");
                LogisTrac.WriteLog("串口指令：AT+ICCID\r");

                Thread.Sleep(1000);

                string indata1 = this.serialPort1.ReadExisting();

                string l_strResult1 = indata1.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "").Replace("OK", "");

                LogisTrac.WriteLog("串口返回值：" + l_strResult1);

                string[] sArray1 = l_strResult1.Split((new char[1] { ':' }));

                for (int index = 0; index < sArray1.Length; index++)
                {
                    if ((index == 1))
                    { CARD_NO = sArray1[index].ToString(); }
                }



                this.serialPort1.DiscardInBuffer();
                this.serialPort1.Write("AT+CREG=2\r");
                LogisTrac.WriteLog("串口指令：AT+CREG=2\r");

                Thread.Sleep(1000);

                this.serialPort1.DiscardInBuffer();
                this.serialPort1.Write("AT+CREG?\r");
                LogisTrac.WriteLog("串口指令：AT+CREG?\r");

                Thread.Sleep(1000);

                string indata = this.serialPort1.ReadExisting();

                string l_strResult = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值：" + l_strResult);

                string[] sArray = l_strResult.Split((new char[1] { ',' }));

                for (int index = 0; index < sArray.Length; index++)
                {
                    if ((index == 3))
                    { Cell_ID = Convert.ToInt32(sArray[index], 16).ToString(); }

                    if ((index == 2))
                    { LAC = Convert.ToInt32(sArray[index], 16).ToString(); }
                }

                if (Cell_ID != "")
                {
                    MCC = "460";
                    MNC = "00";
                }

            }
        }

        //重启
        public void SendData1()
        {
            if (this.serialPort1.IsOpen)
            {
                //发送数据
                SubSendData1();
                this.serialPort1.Close();
            }
            else
            {

                this.serialPort1.Open();

                if (this.serialPort1.IsOpen)
                { //发送数据
                    SubSendData1();
                    this.serialPort1.Close();
                }
                else
                { LogisTrac.WriteLog("串口打开失败"); }

            }
            this.serialPort1.Close();
        }

        private void SubSendData1()
        {
            this.serialPort1.DiscardInBuffer();

            this.serialPort1.Write("AT+CFUN=1,1\r");
            LogisTrac.WriteLog("串口指令：    AT+CFUN=1,1\r");
        }

        //模块信息
        public void SendData2()
        {
            if (this.serialPort1.IsOpen)
            {
                //发送数据
                SubSendData2();
                this.serialPort1.Close();
            }
            else
            {

                this.serialPort1.Open();

                if (this.serialPort1.IsOpen)
                { //发送数据
                    SubSendData2();
                    this.serialPort1.Close();
                }
                else
                { LogisTrac.WriteLog("串口打开失败"); }

            }
            this.serialPort1.Close();
        }

        private void SubSendData2()
        {
            if (iweb_type == 1)
            {
                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT^HCSQ?\r");
                LogisTrac.WriteLog("串口指令：AT^HCSQ?\r");

                Thread.Sleep(1000);

                string indata = this.serialPort1.ReadExisting();

                string l_strResult = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值,网络模式与信号质量（NOSERVICE为异常）：" + l_strResult);




                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT^SYSINFOEX\r");
                LogisTrac.WriteLog("串口指令：AT^SYSINFOEX\r");

                Thread.Sleep(1000);

                indata = this.serialPort1.ReadExisting();

                string l_strResult1 = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值，（服务状态（0无服务）、服务域（0无服务）、漫游、sim卡状态（0无效卡））：" + l_strResult1);
                int pos = l_strResult1.IndexOf("AT^SYSINFOEX");
              //  l_strResult1 = l_strResult1.;


                string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>当前网络状态：\r" + l_strResult + "\r" + l_strResult1 + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                axShockwaveFlash1.CallFunction(flash);


                net_flash = l_strResult + "\r" + l_strResult1;
            }

            if (iweb_type == 2)
            {
                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT+CSQ\r");
                LogisTrac.WriteLog("串口指令：AT+CSQ\r");

                Thread.Sleep(1000);

                string indata = this.serialPort1.ReadExisting();

                string l_strResult = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值,信号质量（rssi，ber）（参数都是99为异常）：" + l_strResult);



                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT+CGREG?\r");
                LogisTrac.WriteLog("串口指令：AT+CGREG?\r");

                Thread.Sleep(1000);

                indata = this.serialPort1.ReadExisting();

                string l_strResult1 = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值,注册状态（n，stat）（返回 0，1 正常）：" + l_strResult1);




                this.serialPort1.DiscardInBuffer();

                this.serialPort1.Write("AT+CPIN?\r");
                LogisTrac.WriteLog("串口指令：AT+CPIN?\r");

                Thread.Sleep(1000);

                indata = this.serialPort1.ReadExisting();

                string l_strResult2 = indata.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\"", "");

                LogisTrac.WriteLog("串口返回值,SIM卡状态（READY正常）：" + l_strResult2);



                string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>当前网络状态：\r" + l_strResult + "\r" + l_strResult1 + "\r" + l_strResult2 + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                axShockwaveFlash1.CallFunction(flash);

                net_flash = l_strResult + "\r" + l_strResult1 + "\r" + l_strResult2;

            }
        }




        //打开串口
        private bool Init_Com1()
        {
            try
            {
                ////打开串口
                devicemanager.PortName = OperateIniFile.ReadIniData("COM_PORT");
                bool comcheck = devicemanager.Start();
                if (comcheck)
                {
                    //串口读取事件
                    devicemanager.CmdMngr.CommandReceivedHandler = new CommandManager.CommandReceivedDelegate(this.CommandReceivedDelegate);

                }

                return comcheck;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("串口打开异常");
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

        /// <summary>
        /// 设定摄像头显示位置
        /// </summary>
        private void Init_PicturePosion()
        {
            try
            {
                int SH = Screen.PrimaryScreen.Bounds.Height;
                int SW = Screen.PrimaryScreen.Bounds.Width;

                #region 1024

                double oneperweight = SW / (1024 * 1.0);
                double oneperheight = SH / (768 * 1.0);

                int basewidth = 308;//摄像头控件满屏显示时的宽度230
                int baseheight = 173;//摄像头控件满屏显示时的高度


                int fblwidth = Convert.ToInt32(basewidth * oneperweight);//根据分辨率计算满屏的宽度
                int fblheight = Convert.ToInt32(baseheight * oneperheight);//根据分辨率计算满屏的高度
                int left = SW / 2 - fblwidth / 2;//控件距左侧的距离

                if (SH == 720 && SW == 1280)
                {
                    left = left - 34;


                }
                this.cameraControl.Top = Convert.ToInt32(88 * oneperheight);
                this.cameraControl.Left = left;//距离最左侧的距离

                System.Drawing.Drawing2D.GraphicsPath shape = new System.Drawing.Drawing2D.GraphicsPath();
                int cwidth = Convert.ToInt32(baseheight * oneperweight);//原图的宽度 圆形的 高度
                int cheight = Convert.ToInt32(baseheight * oneperheight);//原图的高度 圆形的 宽度

                this.cameraControl.Width = fblwidth;
                this.cameraControl.Height = fblheight;

                int x = (fblwidth - fblheight) / 2;

                shape.AddEllipse(x, 0, cwidth, cheight);




                //Matrix matrix = new Matrix();
                //matrix.Translate(0, 0);

                //Region aaa = new Region(shape);
                //aaa.Transform(matrix);





                this.cameraControl.Region = new Region(shape);
               // SizeF a = new SizeF(1,0);
               bool a= this.cameraControl.IsMirrored;
                #endregion

            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }


        }

        //初始化定时器
        private void Init_AllTimer()
        {
            if (uploadresulttimer.Enabled == true) { LogisTrac.WriteLog("初始化timer过了，退出");  return; }//只启动一次timer

            LogisTrac.WriteLog("初始化timer");
           // indexformtimer = new System.Timers.Timer(yoyoConst.INDEX_TIME);
            indexformtimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => indexformtimer_Tick(s, e));
            indexformtimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            indexformtimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件   

           // uploadresulttimer = new System.Timers.Timer(yoyoConst.INDEX_TIME);
            uploadresulttimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => uploadresulttimer_Tick(s, e));
            uploadresulttimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            uploadresulttimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  

            //定时检查网络
           // netcheckTimer = new System.Timers.Timer(1000);
            netcheckTimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => netcheckTimer_Tick(s, e));
            netcheckTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            netcheckTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  

            ////定时执行长连接命令
            //linkTimer = new System.Timers.Timer(1000);
            //linkTimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => LinkTimer_Tick(s, e));
            //linkTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            //linkTimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  


           // handleaveTimer = new System.Timers.Timer(1000);
            handleaveTimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => handleaveTimer_Tick(s, e));
            handleaveTimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            handleaveTimer.Enabled = false;//是否执行System.Timers.Timer.Elapsed事件  

           // facetimer = new System.Timers.Timer(1000);
            facetimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => faceTimer_Tick(s, e));
            facetimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            facetimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  

        }


        //初始化定时器
        private void Init_INDEX_TIME_Timer()
        {
           // indexformtimer = new System.Timers.Timer(yoyoConst.INDEX_TIME);
            indexformtimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => indexformtimer_Tick(s, e));
            indexformtimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            indexformtimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  


          //  facetimer = new System.Timers.Timer(1000);
            facetimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => faceTimer_Tick(s, e));
            facetimer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)
            facetimer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件  
        }

        /// <summary>
        /// 硬件自检，获取服务器时间并设置pc时间
        /// </summary>
        private bool Init_ComputerTime()
        {
            if (!htt_net("获取服务器时间")) return false;
            if (!htConst.checkEQUSN("获取服务器时间"))
            {
                return false;
            }
            try
            {
                string url = htConst.getUrlPara();
                string parurl = htConst.url_IFI_05 + url;
                string ret = htConst.Post(parurl, "");

                retIFI05 model = JsonHelper.FromJSON<retIFI05>(ret);

                if (model != null && model.data != DateTime.MinValue)
                {
                    // 创建SystemTime结构体，用于接收用户设置的时间
                    //DatetimeHelper.SystemTime systemTime = new DatetimeHelper.SystemTime();
                    string time = model.data.ToString("yyMMddHHmmss");
                    devicemanager.Write(new Cmd_S_Settime(time));

                    string time1 = model.data.ToString("yyyyMMddHHmmss");
                    bool result1 = SetDate(time1);

                    if (result1)
                    { LogisTrac.WriteLog("设定电脑时间成功" + time1); }
                    else
                    { LogisTrac.WriteLog("设定电脑时间失败"); }

                    return true;
                }
                else
                {
                    LogisTrac.WriteLog("设定电脑时间失败");
                    return false;
                }
            }
            catch
            {
                LogisTrac.WriteLog("设定电脑时间失败");
                return false;
            }

        }

        [DllImport("kernel32.dll")]
        private static extern bool SetLocalTime(ref SYSTEMTIME time);

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short year;
            public short month;
            public short dayOfWeek;
            public short day;
            public short hour;
            public short minute;
            public short second;
            public short milliseconds;
        }

        public bool SetDate(string time)
        {

            SYSTEMTIME st;

            st.year = (short)int.Parse(time.Substring(0, 4));
            st.month = (short)int.Parse(time.Substring(4, 2));
            st.day = (short)int.Parse(time.Substring(6, 2));

            st.hour = (short)int.Parse(time.Substring(8, 2));
            st.minute = (short)int.Parse(time.Substring(10, 2));
            st.second = (short)int.Parse(time.Substring(12, 2));

            st.milliseconds = (short)DateTime.Now.Millisecond;
            st.dayOfWeek = (short)88;

            bool rt = SetLocalTime(ref st);

            return rt;
        }
        #endregion

        #region 人脸
        //人脸检测的方法。         
        private void Face_TakePicMethod()
        {
            try
            {
                bool checkflag = checkFace();
                this.HAS_FACE = checkflag;

                if (checkflag)
                {
                    this.FACE_CHECK = this.FACE_CHECK_COUNT;//存在人脸则重新计数
                }

                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_man
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_woman)
                {
                    if (this.indexformtimer.Enabled == checkflag)
                    {
                        this.indexformtimer.Enabled = !checkflag;
                    }
                }

                if (checkflag && (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.yoyo
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.connect_error
                    //  || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading_error
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode
                    ))
                {
                    //网络畅通时跳转二维码画面,否则跳转错误画面
                    if (this.Init_Check_Net)
                    {
                        htt_barcode();
                    }
                    else
                    {
                        flash_change(yoyoConst.YoyoStep.connect_error);
                    }

                }
                else if (!checkflag && (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing))
                {
                    FACE_CHECK--;
                    LogisTrac.WriteLog("测试过程中,人脸不在镜头中---" + FACE_CHECK);

                    if ((FACE_CHECK > 0))
                    {
                        if (FACE_CHECK_COUNT - FACE_CHECK > 3)//前3秒不播语音
                        {
                            string flash = "<invoke name=\"facein\" returntype=\"xml\"></invoke>";
                            axShockwaveFlash1.CallFunction(flash);
                        }
                    }
                    else
                    {
                        this.flash_change(yoyoConst.YoyoStep.yoyo);
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        // 摄像头验证,初始化
        private bool Face_Camera2()
        {
            try
            {
                GC.Collect();
                _CameraChoice.UpdateDeviceList();
                if (_CameraChoice.Devices.Count > 0)
                {

                    var camera_moniker = _CameraChoice.Devices[0].Mon;
                    Resolution resolution = new Resolution(320, 240);
                    cameraControl.SetCamera(camera_moniker, resolution);
                    GC.Collect();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                yoyoConst.WriteLog(ex.ToString());
            }
            return false;
        }

        /// <summary>
        /// 验证是否有人脸
        /// </summary>
        /// <returns></returns>
        private bool checkFace()
        {
            try
            {
                if (!cameraControl.CameraCreated)
                    return false;

                bool checkflag = false;

                if (Face_Error && !Face_Error_doing)
                {
                    Face_Error_doing = true;
                    #region
                    try
                    {
                        Face_Error = !Face_Camera2();
                        if (!Face_Error)
                        {
                            LogisTrac.WriteLog("摄像头恢复成功----");
                            string flash1 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "摄像头恢复成功----" + "<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                            axShockwaveFlash1.CallFunction(flash1);
                        }
                        else
                        {
                            LogisTrac.WriteLog("摄像头恢复失败----");
                            string flash1 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "摄像头恢复失败----" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                            axShockwaveFlash1.CallFunction(flash1);
                        }
                    }
                    catch (Exception ex1)
                    {
                        LogisTrac.WriteLog(ex1.Message);
                        LogisTrac.WriteLog("运行过程中摄像头初始化失败");
                        string flash1 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "摄像头恢复失败----" + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash1);
                    }
                    finally
                    {
                        Face_Error_doing = false;
                    }
                    #endregion
                }
                else
                {

                    Bitmap bitmap = null;
                    try
                    {
                        bitmap = cameraControl.SnapshotSourceImage();//.SnapshotOutputImage();

                        if (bitmap == null)
                        {
                            return false;
                        }

                        checkflag = AFDFunction.checkAndMarkFace(bitmap);
                        Face_Time_out_count_check = 0;


                        //liudq
                        if (checkflag && (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing) && (Camera_change == true) && (ichange == 1) && (cameraControl.Resolution.Height == 480))
                        {
                            var camera_moniker = _CameraChoice.Devices[0].Mon;
                            Resolution resolution2 = new Resolution(320, 240);
                            cameraControl.SetCamera(camera_moniker, resolution2);

                            Camera_change = false;
                            ichange = 2;
                            bitmap.Save("test.jpg");

                            //增加线程  调用接口                            
                            System.Timers.Timer t = new System.Timers.Timer(1000);//实例化Timer类，设置时间间隔
                            t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => upload_image(s, e));
                            t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
                            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
                        }

                        if (checkflag && (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing) && (Camera_change == false) && (ichange == 0))
                        {
                            var camera_moniker = _CameraChoice.Devices[0].Mon;
                            //  cameraControl.Resolution.Height = 480;
                            Resolution resolution3 = new Resolution(640, 480);
                            cameraControl.SetCamera(camera_moniker, resolution3);
                            Camera_change = true;//进入二维码界面归false；
                            ichange = 1;//进入二维码界面归0；
                        }

                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.IndexOf("Timeout while") >= 0)
                        {

                            Face_Time_out_count_check++;
                            if (Face_Time_out_count_check >= this.FACE_CHECK_COUNT)
                            {
                                Face_Error = true;//无法识别人脸
                                Face_Time_out_count_check = 0;
                            }
                        }
                        else if (ex.Message.IndexOf("内存") >= 0)
                        {
                            yoyoConst.WriteLog("无法识别人脸,摄像头异常,系统重新启动;" + ex.Message);
                            // update_start();
                            devicemanager.Write(new Cmd_S_ReBoot());
                        }
                        else
                        {
                            Face_Error = true;//无法识别人脸
                            yoyoConst.WriteLog("无法识别人脸,摄像头异常" + ex.Message);
                        }
                    }
                    finally
                    {
                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                            bitmap = null;
                        }
                    }
                }
                return checkflag;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }

        }

        private void upload_image(object sender, EventArgs e)
        {
            try
            {
                UploadRequest(yoyoConst.SERVER_IP + "/api/face/recognition/" + yoyoConst.EQU_SN + "/" + yoyoConst.CURRTEN_USERNO, "test.jpg");//KJ103IS00000311/8
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void UploadRequest(string url, string filePath)
        {
            // 时间戳，用做boundary
            string timeStamp = DateTime.Now.Ticks.ToString("x");

            //根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpReq.Method = "POST";
            httpReq.AllowWriteStreamBuffering = false; //对发送的数据不使用缓存
            httpReq.Timeout = 300000;  //设置获得响应的超时时间（300秒）
            httpReq.ContentType = "multipart/form-data; boundary=" + timeStamp;

            //文件
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            //头信息
            string boundary = "--" + timeStamp;
            string dataFormat = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n";
            string header = string.Format(dataFormat, "file", Path.GetFileName(filePath));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(header);

            //结束边界
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + timeStamp + "--\r\n");

            long length = fileStream.Length + postHeaderBytes.Length + boundaryBytes.Length;

            httpReq.ContentLength = length;//请求内容长度

            try
            {
                //每次上传4k
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数
                long offset = 0;
                int size = binaryReader.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    size = binaryReader.Read(buffer, 0, bufferLength);
                }

                //添加尾部边界
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应
                using (HttpWebResponse response = (HttpWebResponse)httpReq.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    string returnValue = readStream.ReadToEnd();

                    LogisTrac.WriteLog("拍照上传返回结果：" + returnValue);

                    retimage model = JsonHelper.FromJSON<retimage>(returnValue);

                    if (model != null && model.faceId != null)
                    {
                        yoyoConst.faceid = model.faceId;
                    }

                    response.Close();
                    readStream.Close();
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
            finally
            {
                fileStream.Close();
                binaryReader.Close();
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
            netcheck_fun_ping();
        }

        private void netcheck_fun_ping()
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
                string url = yoyoConst.SERVER_IP.Replace("http://", "").Replace("/", "");

                if (url.IndexOf(":") > 0)
                {
                    url = url.Substring(0, url.IndexOf(":"));
                }

                while (count > 0)
                {
                    count--;
                    flag = Internet.RunCmd_Ping(url);//使用ping的方式进行网络检测
                    if (!flag)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        count = 0;
                    }
                }

                NET_LOG_COUNT--;
                if (!flag && NET_LOG_COUNT <= 1)
                {
                    LogisTrac.WriteLog("网络异常.重启");
                    NET_LOG_COUNT = 12;
                    NET_LOG_ERROR = true;
                    if (!restartnettimer.Enabled)
                    {
                        if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则记录模块状态  liudq
                        {
                            LogisTrac.WriteLog("网络异常.重启1");
                            SendData2();
                            LogisTrac.WriteLog("网络异常.重启2");
                        }

                        restartnettimer.Enabled = true;//重启网络
                        restartNet();
                    }

                    LogisTrac.WriteLog("网络异常......");
                }

                if (flag && NET_LOG_ERROR)
                {
                    NET_LOG_ERROR = false;
                    restartnettimer.Enabled = false;
                    LogisTrac.WriteLog("网络恢复正常......");
                    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>当前网络状态<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                    axShockwaveFlash1.CallFunction(flash);
                }

                this.Init_Check_Net = flag;


                Init_Check_Neting = false;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void netcheck_fun()
        {
            try
            {
                //检测多次网络是否畅通
                if (Init_Check_Neting || _client == null)
                {
                    return;
                }

                int count = 5;
                bool flag = false;
                Init_Check_Neting = true;
                while (count > 0)
                {
                    count--;
                    flag = Internet.IsConnectInternet();//定时检测网络改为长连接  _client.IsConnected;
                    if (!flag)
                    {
                        //longConnection(htConst.getEQUSN());
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        count = 0;
                    }
                }

                NET_LOG_COUNT--;
                if (!flag && NET_LOG_COUNT <= 1)
                {
                    NET_LOG_COUNT = 12;
                    NET_LOG_ERROR = true;

                    if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则记录模块状态  liudq
                    {
                        SendData2();
                    }

                    if (!restartnettimer.Enabled)
                    {
                    

                        restartnettimer.Enabled = true;//重启网络
                        restartNet();

                        longConnection(htConst.getEQUSN());
                    }
                    LogisTrac.WriteLog("网络异常......");
                }

                if (flag && NET_LOG_ERROR)
                {
                    NET_LOG_ERROR = false;
                    restartnettimer.Enabled = false;//重启网络停止
                    LogisTrac.WriteLog("网络恢复正常......");
                    string flash = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>当前网络状态<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                    axShockwaveFlash1.CallFunction(flash);

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
                if (cmd is Cmd_X_Check)
                {
                    #region 上位机请求下位机自检的协议
                    //2.10	上位机请求下位机自检的协议
                    //0x00：生物电绿光自检均不正常
                    //0x01：生物电自检正常
                    //0x02：绿光自检正常
                    //0x03：生物电、绿光自检均正常

                    if (cmd.GetData()[4] == 0x03)
                    {
                        this.Init_Check_Device2 = true;
                    }
                    else
                    {
                        LogisTrac.WriteLog("自检状态 " + cmd.GetData()[4].ToString() + " False");
                    }
                    #endregion
                }

                else if (cmd is Cmd_X_SN)
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
                    yoyoConst.EQU_SN_LOG = yoyoConst.EQU_SN;
                    OperateIniFile.WriteSNIniData("EQU_SN_LOG", yoyoConst.EQU_SN_LOG);

                    yoyoConst.WriteLog("获取的sn号:" + yoyoConst.EQU_SN_LOG);
                    yoyoConst.initEquType(yoyoConst.EQU_SN_LOG);
                    lblversion.Text += yoyoConst.VERSION.Substring(4, 5);
                    yoyoConst.WriteLog("获取的固件版本号:" + yoyoConst.VERSION);

                    string flash = "<invoke name=\"show_version_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_version") + "' color='" + OperateIniFile.ReadIniData("color_version") + "'>" + OperateIniFile.ReadIniData("MAIN_VERSION") + "\r  " + OperateIniFile.ReadVersionIniData("MAIN_VERSION") + "\r  " + OperateIniFile.ReadVersionIniData("RES_VERSION") + "\r  " + yoyoConst.VERSION.Substring(4, 5) + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_version") + "</number><number>" + OperateIniFile.ReadIniData("y_version") + "</number><number>" + OperateIniFile.ReadIniData("width_version") + "</number><number>" + OperateIniFile.ReadIniData("height_version") + "</number></arguments></invoke>";
                    axShockwaveFlash1.CallFunction(flash);

                    if (!DeviceHelper.Com_checkSN())
                    {
                        devicemanager.Write(new Cmd_S_SN());
                        this.Init_Check_SN = false;
                    }
                    else
                    {
                        this.Init_Check_SN = true;
                    }

                    #endregion
                }

                else if (cmd is Cmd_X_Start)
                {
                    //2.6.2.1	开启测量的软件协议 返回值
                    SEND_REPORT = false;
                    flash_change(yoyoConst.YoyoStep.testing, "0");
                }

                else if (cmd is Cmd_X_Reset)
                {


                    #region 2.1	下位机上报按键和绿光数据的协议
                    //性别选择和复位
                    string x = cmd.GetData()[4].ToString();
                    if (x == "1")
                    {
                        flash_playButton();
                    }
                    else
                    {
                        if (yoyoConst.currtenStep != null && yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode)
                        {
                        }
                        else
                        {
                            flash_playButton();
                        }
                    }

                    if (yoyoConst.currtenStep != null && yoyoConst.currtenStep.step == yoyoConst.YoyoStep.yoyo && this.Init_Check_Net)
                    {
                        htt_barcode();
                        Camera_change = false;//进入二维码界面归false；
                        ichange = 0;//进入二维码界面归0；
                        return;
                    }



                    if (x == "4" && !this.Testing)
                    {
                        if (yoyoConst.currtenStep != null &&
                        (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_man
                        || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_woman)
                        )
                        {
                            //100 开始
                            this.Testing = true;
                            devicemanager.Write(new Cmd_S_Start());
                        }
                    }
                    else if (x == "2")
                    {
                        if (yoyoConst.currtenStep != null &&
                         (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_man
                         || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_woman)
                         )
                        {
                            //10男女
                            flash_changeSex();
                        }
                    }
                    else if (x == "1" && this.Init_Finish
                            && yoyoConst.currtenStep != null
                            && yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing)
                    {
                        LogisTrac.WriteLog("复位开始");
                        flash_change(yoyoConst.YoyoStep.yoyo);
                        LogisTrac.WriteLog("复位结束");
                    }
                    else if (this.Init_Finish
                        && yoyoConst.currtenStep != null
                        && yoyoConst.currtenStep.step != yoyoConst.YoyoStep.uploading
                        && yoyoConst.currtenStep.step != yoyoConst.YoyoStep.testing)
                    {
                        LogisTrac.WriteLog("复位" + x + "开始前&运算");
                        int temp = Convert.ToInt32(x) & 1;

                        if (temp == 1)
                        {
                            LogisTrac.WriteLog("复位" + x + "开始");
                            flash_change(yoyoConst.YoyoStep.yoyo);
                            LogisTrac.WriteLog("复位" + x + "结束");
                        }
                    }

                    #endregion
                }
                else if (cmd is Cmd_X_Settime)
                {
                    string x = cmd.GetData()[4].ToString();
                }
                else if (cmd is Cmd_X_Sex)
                {
                    //2.10	上位机请求下位机自检的协议\
                    //0x00：生物电绿光自检均不正常
                    //0x01：生物电自检正常
                    //0x02：绿光自检正常
                    //0x03：生物电、绿光自检均正常

                    string x = cmd.GetData()[4].ToString();
                    // this.textBox1.Text = x;
                }
                else if (cmd is Cmd_X_Tuch)
                {
                    //2.6.2.2	上报是否接触电极的软件协议
                    //未接触电极：0x01
                    //接触电极：0x02
                    string x = cmd.GetData()[4].ToString();

                    if (x == "1")
                    {
                        this.handleaveTimer.Enabled = true;
                        this.HAND_LEAVE = 0;
                    }
                    else
                    {
                        this.handleaveTimer.Enabled = false;
                        this.HAND_LEAVE = 0;
                    }
                }
                else if (cmd is Cmd_X_Persent)
                {
                    if (!this.Testing)
                    {
                        return;
                    }
                    #region 2.6.2.3	上报测量百分比的软件协议
                    string x = cmd.GetData()[4].ToString();
                    //  Console.WriteLine("11--" + cmd.GetData()[4] + "--" + cmd.GetData()[5]);


                    flash_change(yoyoConst.YoyoStep.testing, x);
                    #endregion

                }
                else if (cmd is Cmd_X_Report)
                {
                    #region 2.6.2.4.1	测量正常结束上传数据的协议

                    devicemanager.Write(new Cmd_S_Report());
                    Thread.Sleep(100);//防止反复接收串口命令
                    if (yoyoConst.currtenStep != null && yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading)
                    {
                        return;
                    }
                    LogisTrac.WriteLog("开始报告123........");
                    if (!SEND_REPORT)
                    {
                        LogisTrac.WriteLog("开始报告........");
                        SEND_REPORT = true;
                        byte[] list = cmd.GetData();
                        //cmd.show(list);

                        flash_change(yoyoConst.YoyoStep.uploading);

                        String mxStr = "";
                        for (int k = 4; k < 167 - 4; k++)
                        {
                            if (k > 4)
                            {
                                mxStr += yoyoConst.SPLIT;
                            }
                            try
                            {
                                mxStr += Convert.ToInt32(list[k]);
                            }
                            catch
                            {
                            }
                        }

                        mxStr += yoyoConst.SPLIT + yoyoConst.CURRTEN_SEX;
                        Thread mythread = new Thread(Com_startSendReport);
                        mythread.Start(mxStr);
                    }

                    #endregion
                }
                else if (cmd is Cmd_X_ReportErr)
                {
                    //2.6.2.4.2	测量异常结束的协议
                    devicemanager.Write(new Cmd_S_ReportErr());
                    LogisTrac.WriteLog("测量异常，返回");
                    flash_change(yoyoConst.YoyoStep.yoyo);
                }
                else if (cmd is Cmd_X_Ready)
                {
                    //3.3	上位机查询下位机是否准备就绪的协议
                    string x = cmd.GetData()[4].ToString();
                    //0x01表示准备就绪，可以升级；0x00表示下位机未准备就绪

                }
                else if (cmd is Cmd_X_BootShut)
                {
                    //2.5.1	开机关机
                }
                else if (cmd is Cmd_X_TimingPower)
                {
                    //2.5.2	定时开关机
                }
                else if (cmd is Cmd_X_ReBoot)
                {
                    //2.6	远程重启
                }
                else if (cmd is Cmd_X_Upgrade)
                {
                    //2.9	升级开始结束
                }
                else if (cmd is Cmd_X_Lock)
                {
                    //2.12	锁定状态
                }

            }
            catch (Exception ex)
            {

                yoyoConst.WriteLog(ex.ToString());
            }
        }


        #region 报告上传
        private void Com_startSendReport(object o)
        {
            LogisTrac.WriteLog("开始报告上传");
            try
            {
                REPORT_id = "";
                SEND_REPORT = true;
                //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //stopwatch.Start(); //  开始监视代码
                string mxStr = (string)o;
                lock (comLockObj)
                {
                    #region 调用exe生成字符
                    //0代表女性，1代表男
                    //mxStr += yoyoConst.SPLIT + yoyoConst.CURRTEN_SEX;
                    Com_WriteTextForFile("text_read_imp.txt", mxStr);
                    //2 运行exe
                    // LogisTrac.WriteLog("GetCurrentDirectory111" + System.IO.Directory.GetCurrentDirectory());
                    System.IO.Directory.SetCurrentDirectory(yoyoConst.KANGJIA_PATH);
                    Process p = new Process();
                    p.StartInfo.WorkingDirectory = yoyoConst.KANGJIA_PATH;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = "ConsoleFile.exe";
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    // LogisTrac.WriteLog("GetCurrentDirectory222" + System.IO.Directory.GetCurrentDirectory());
                    //Process p = Process.Start("ConsoleFile.exe");
                    p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                    p.Dispose();
                    //3 读取text_write_ind.txt结果
                    string[] datalist = new string[0];
                    if (File.Exists(yoyoConst.KANGJIA_PATH + "text_write_ind.txt"))
                    {
                        datalist = File.ReadAllLines(yoyoConst.KANGJIA_PATH + "text_write_ind.txt");
                    }
                    if (File.Exists(yoyoConst.KANGJIA_PATH + "text_read_val2.txt"))
                    {
                        //4 用text_read_val2.txt替换text_read_val.txt的内容
                        string val2 = File.ReadAllText(yoyoConst.KANGJIA_PATH + "text_read_val2.txt");
                        Com_WriteTextForFile("text_read_val.txt", val2);
                    }

                    #endregion

                    #region json设定
                    string time = DateTime.Now.ToString("yyMMddHHssfff");
                    htReport model = new htReport();
                    model.macId = yoyoConst.MAC;
                    model.reportDate = DateTime.Now.ToString("yyyy-MM-dd HHss"); //"2017-12-22 00:07";
                    model.reportId = yoyoConst.EQU_SN + "" + time + htt_getRadom();

                    REPORT_id = model.reportId;

                    model.deviceSN = yoyoConst.EQU_SN;
                    model.timeStamp = DateTime.Now.ToString("yyyyMMddHHssmm");
                    model.type = yoyoConst.EQU_TYPE;//长连接版本

                    htReportUserbean userbean = new htReportUserbean();
                    userbean.glasses = "0";
                    userbean.heartRate = "";
                    try
                    {
                        userbean.age = int.Parse(yoyoConst.age.Substring(0, 2));
                    }
                    catch
                    {
                        userbean.age = 0;
                        LogisTrac.WriteLog("年龄异常:" + yoyoConst.age);
                    }

                    try
                    {
                        userbean.faceId = yoyoConst.faceid;
                        LogisTrac.WriteLog("faceId:" + userbean.faceId);
                    }
                    catch
                    {
                        userbean.faceId = "";
                        LogisTrac.WriteLog("faceId异常");
                    }

                    userbean.height = "";
                    userbean.heightUnit = "-cm";
                    userbean.sex = yoyoConst.CURRTEN_SEX;
                    userbean.weight = "";
                    userbean.weightUnit = "-kg";
                    userbean.userNo = yoyoConst.CURRTEN_USERNO;

                    Hashtable targetList = new Hashtable();
                    foreach (string subdata in datalist)
                    {
                        string[] list2 = subdata.Split(' ');
                        if (!string.IsNullOrEmpty(list2[0]))
                        {
                            string value = list2[1];
                            int key = Convert.ToInt32(list2[0]);
                            bool addfalg = true;
                            if (yoyoConst.CURRTEN_SEX == "1")
                            {
                                if (key >= 3153 && key <= 3162)
                                {
                                    // value = "0";
                                    addfalg = false;
                                }
                                else if (key >= 3224 && key <= 3225)
                                {
                                    // value = "0";
                                    addfalg = false;
                                }
                                else if (key >= 3268 && key <= 3269)
                                {
                                    // value = "0";
                                    addfalg = false;
                                }
                                else if (key >= 3283 && key <= 3284)
                                {
                                    //value = "0";
                                    addfalg = false;
                                }
                            }
                            else if (yoyoConst.CURRTEN_SEX == "0")
                            {
                                if (key >= 3145 && key <= 3151)
                                {
                                    //value = "0";
                                    addfalg = false;
                                }

                            }
                            if (addfalg)
                            {
                                targetList.Add(list2[0], Convert.ToDouble(value));
                            }
                        }
                    }

                    model.targetList = targetList;
                    model.userBean = userbean;
                    #endregion

                    //int start = 3000;
                    LogisTrac.WriteLog("开始报告上传-网络检测");

                    int count = 5;
                    bool uploadflag = false;
                    // bool waitflag = true;//如果没有返回值,则画面不再等待5秒
                    while (count > 0 && !uploadflag)
                    {
                        count--;
                        string ret = htConst.Post(htConst.url_htreport, model);
                        LogisTrac.WriteLog("开始报告上传");
                        htRetBasemodel retmodel = JsonHelper.FromJSON<htRetBasemodel>(ret);
                        LogisTrac.WriteLog("报告上传调用接口成功");
                        if (retmodel != null && retmodel.success == "true")
                        {
                            count = 0;
                            uploadflag = true;
                            SEND_REPORT = false;
                            LogisTrac.WriteLog("画面切换到成功画面");


                            //liudq 
                            if (this.IPRINT == 0)
                            { flash_change(yoyoConst.YoyoStep.result); }
                            else if (this.IPRINT == 1)
                            { flash_change(yoyoConst.YoyoStep.result, "http://akso.wrmjk.com/doprint/" + model.reportId); }
                            else if (this.IPRINT == 2)
                            { flash_change(yoyoConst.YoyoStep.result, retmodel.data); }


                            flash_change_delay(yoyoConst.YoyoStep.yoyo, yoyoConst.FINISH_TIME);
                            Com_Delet_text_write_ind();//删除text_write_ind.txt
                            LogisTrac.WriteLog("上传报告成功" + count + "次,报告id：" + REPORT_id);

                            return;
                            //  break;
                        }
                        else
                        {
                            LogisTrac.WriteLog("上传报告不成功" + count + "次,返回值" + retmodel);
                            // if (retmodel != null)
                            {
                                Thread.Sleep(5000);
                            }
                        }
                    }

                    if (!uploadflag)
                    {
                        REPORT_id = "";
                        string jsonParas = JsonHelper.objectToJson(model);
                        string filename = COM_WAIT_TXT + "\\" + yoyoConst.CURRTEN_USERNO + "_" + DateTime.Now.ToString("yyyyMMddHHssmm") + ".txt";
                        LogisTrac.WriteLog("上传报告不成功,生成文件--" + filename);
                        Com_WriteTextForFile(filename, jsonParas);
                    }
                    //stopwatch.Stop(); //  停止监视
                    //TimeSpan timeSpan = stopwatch.Elapsed; //  获取总时间
                    //double hours = timeSpan.TotalHours; // 小时
                    //double minutes = timeSpan.TotalMinutes;  // 分钟
                    //double seconds = timeSpan.TotalSeconds;  //  秒数
                    //double milliseconds = timeSpan.TotalMilliseconds;  //  毫秒数

                    LogisTrac.WriteLog("上传报告不成功");
                    Com_Delet_text_write_ind();//删除text_write_ind.txt
                    flash_change(yoyoConst.YoyoStep.uploading_error);
                    flash_change_delay(yoyoConst.YoyoStep.yoyo, yoyoConst.FINISH_TIME);
                    SEND_REPORT = false;
                    REPORT_id = "";


                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("上传报告不成功111:" + ex.Message);
            }
        }


        /// <summary> 
        /// text_read_imp
        /// </summary> 
        /// <param name="message"> text_read_imp</param> 
        private void Com_WriteTextForFile(string filename, string message)
        {
            try
            {
                string LogPath = yoyoConst.KANGJIA_PATH + filename;
                if (File.Exists(LogPath))
                {
                    File.Delete(LogPath);
                }
                string logFile = LogPath;
                StreamWriter sw = new StreamWriter(logFile, true);
                sw.WriteLine(message);
                sw.Close();
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
            return;
        }
        //删除文件
        private void Com_Delet_text_write_ind()
        {
            try
            {
                if (File.Exists(yoyoConst.KANGJIA_PATH + "text_write_ind.txt"))
                {
                    File.Delete(yoyoConst.KANGJIA_PATH + "text_write_ind.txt");
                }
            }
            catch
            {

            }

        }
        #endregion

        #endregion

        #region 定时器
        /// <summary>
        /// 定时跳转到起始页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void indexformtimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //当不是当天日期的时候,上传日志
                string tempNowDate = DateTime.Now.ToString("yyyyMMdd");

                if (tempNowDate != this.NowDate)
                {
                    this.log_upload_delay();
                    this.NowDate = DateTime.Now.ToString("yyyyMMdd");
                }

                if (yoyoConst.currtenStep == null)
                {
                    return;
                }

                // 判断都哪些画面需要自动迁移到启示页
                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_man
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_woman)
                {
                    if (this.HAS_FACE)
                    {
                        return;
                    }
                }


                if (
                     yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing//liudq 
                      || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading//liudq 2018 08 23 上传时候不返回主界面
                      || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading_error//liudq 2018 08 23 上传失败不返回主界面
                    )
                {

                    {
                        LogisTrac.WriteLog("注意！！！测量、上传 过程中，触发15秒回主界面！！！限制成功！！！");
                        return;
                    }
                }


                if (yoyoConst.currtenStep.step != yoyoConst.YoyoStep.yoyo
                    && yoyoConst.currtenStep.step != yoyoConst.YoyoStep.result)
                {
                    LogisTrac.WriteLog("定时，返回");
                    flash_change(yoyoConst.YoyoStep.yoyo);
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

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

        /// <summary>
        /// 定时自动上传未处理的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadresulttimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (yoyoConst.currtenStep == null) return;
                // 判断都哪些画面需要自动迁移到启示页
                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.yoyo || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode)
                {
                    bool checkflag = Internet.IsConnectInternet();
                    if (checkflag)
                    {
                        //上传待上传的数据
                        if (Directory.Exists(COM_WAIT_TXT))
                        {
                            DirectoryInfo TheFolder = new DirectoryInfo(COM_WAIT_TXT);
                            foreach (FileInfo NextFile in TheFolder.GetFiles())
                            {
                                string val2 = File.ReadAllText(NextFile.FullName);
                                string ret = htConst.PostJson(htConst.url_htreport, val2);
                                htRetBasemodel model = JsonHelper.FromJSON<htRetBasemodel>(ret);
                                // File.Delete(WAIT_TXT);
                                if (model.success == "true")
                                {
                                    yoyoConst.WriteLog(NextFile.FullName + "----上传成功");
                                    NextFile.Delete();
                                }
                                else
                                {
                                    yoyoConst.WriteLog(NextFile.FullName + "----上传失败");
                                }
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

        /// <summary>
        /// 定时执行长连接命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (devicemanager == null)
                {
                    LogisTrac.WriteLog("串口未打开");
                }
                else
                {
                    if (!devicemanager.Port.IsOpen)
                    {
                        LogisTrac.WriteLog("串口未打开或出现异常");
                    }
                }
                if (LINK_DOING)
                {
                    return;
                }
                if ((yoyoConst.currtenStep == null ||
                    (
                     yoyoConst.currtenStep != null &&
                    (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.yoyo
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode
                    )
                    )
                    )
                    && this.LinkList != null && LinkList.Count > 0)
                {
                    LINK_DOING = true;
                    string backstr = LinkList[0];
                    LogisTrac.WriteLog("执行命令:" + backstr);
                    // foreach (string backstr in LinkList)
                    {

                        if (backstr == "A003")
                        {
                            #region 锁屏指令
                            flash_change(yoyoConst.YoyoStep.yoyo);
                            #endregion
                        }
                        else if (backstr == "A004")
                        {
                            bool flag = this.update_Check();//判断是否需要升级
                            if (flag)
                            {
                                indexformtimer.Enabled = false;//实例化Timer类，设置时间间隔
                                uploadresulttimer.Enabled = false;//实例化Timer类，设置文件上传线程
                                netcheckTimer.Enabled = false;//.Enabled=falseTimer类，验证网络是否畅通
                                linkTimer.Enabled = false;//实例化Timer类，长连接待执行命令
                                handleaveTimer.Enabled = false;//实例化Timer类，双手离开时间返回主画面
                                update_start();
                            }

                        }
                        else if (backstr == "A005")
                        {
                            #region 上传日志
                            //log_upload_delay();
                            log_upload(DateTime.Now);
                            #endregion
                        }
                        else if (backstr == "A006")
                        {
                            #region 系统自检
                            devicemanager.Write(new Cmd_S_Check());
                            #endregion
                        }
                        else if (backstr == "A007")
                        {
                            #region 系统重启
                            devicemanager.Write(new Cmd_S_ReBoot());
                            #endregion
                        }
                        else if (backstr == "A008")
                        {
                            #region 系统关机
                            devicemanager.Write(new Cmd_S_Shut());
                            #endregion
                        }
                        else if (backstr.IndexOf("off") >= 0)
                        {
                            backstr = backstr.ToLower();
                            string[] list = backstr.Split('-');

                            #region 关机或定时关机
                            string start = list[0].IndexOf("on") >= 0 ? list[0].Replace("on", "") : list[1].Replace("on", "");
                            string end = list[0].IndexOf("off") >= 0 ? list[0].Replace("off", "") : list[1].Replace("off", "");
                            //定时关机
                            devicemanager.Write(new Cmd_S_TimingPower(start, end));
                            ////即时关机
                            //devicemanager.Write(new Cmd_S_Shut());
                            #endregion
                        }
                        else if (backstr.IndexOf("A009") >= 0)//liudq 2018 07 02 
                        {
                            #region 打印指令
                            backstr = backstr.Replace("A009", "");
                            Load_and_print(backstr);//下载打印
                            #endregion
                        }

                        LinkList.RemoveAt(0);
                    }
                    LINK_DOING = false;
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        /// <summary>
        /// 双手离开计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleaveTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing)
                {
                    //双手离开则开始计时

                    if (this.HAND_LEAVE >= yoyoConst.HAND_LEAVE_TIME)
                    {
                        LogisTrac.WriteLog("手离开，返回");
                        flash_change(yoyoConst.YoyoStep.yoyo);
                    }
                    else
                    {
                        this.HAND_LEAVE++;

                        //  if (this.HAND_LEAVE == 5)
                        if ((this.HAND_LEAVE % 5 == 0) && (this.HAND_LEAVE > 3))
                        {     //程序播放语音
                            string flash = "<invoke name=\"playMusic\" returntype=\"xml\"><arguments><string>ball_hold.mp3</string></arguments></invoke>";
                            axShockwaveFlash1.CallFunction(flash);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        /// <summary>
        /// 人脸离开计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void faceTimer_Tick(object sender, EventArgs e)
        {
            if (Face_checking || yoyoConst.currtenStep == null) return;

            Face_checking = true;
            try
            {
                Face_TakePicMethod();
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
            Face_checking = false;
        }

        /// <summary>
        /// 重启网络定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void restartnettimer_Tick(object sender, EventArgs e)
        {
            try
            {
                //重启网络
                restartNet();
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        #endregion

        #region 下载报告并打印

        private void Load_and_print(string backstr)
        {
            try
            {
                string strImageURL = backstr;
                System.Net.WebClient webClient = new System.Net.WebClient();
                webClient.DownloadFile(strImageURL, "baogao.pdf");
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("下载失败：" + ex.Message);
                //反馈下载失败
                if (!htt_printerState(REPORT_id, "P11"))
                {
                    LogisTrac.WriteLog("上传打印状态失败3");
                }
                return;
            }


            try
            {
                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

                LogisTrac.WriteLog("打印机名称：" + pd.PrinterSettings.PrinterName);

                PdfDocument doc = new PdfDocument();

                string filePath = "baogao.pdf";
                doc.LoadFromFile(filePath);
                doc.PrinterName = pd.PrinterSettings.PrinterName;

                //加上这句不显示第几页
                doc.PrintDocument.PrintController = new System.Drawing.Printing.StandardPrintController();

                doc.PrintDocument.Print();

                //启动检查打印结果线程并反馈结果
                System.Timers.Timer t = new System.Timers.Timer(1000);//实例化Timer类，设置时间间隔
                t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => result_upload(s, e));
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件

                //删除报告
                if (File.Exists("baogao.pdf"))
                {
                    File.Delete("baogao.pdf");
                }

            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("打印失败：" + ex.Message);
                //反馈打印失败
                if (!htt_printerState(REPORT_id, "P12"))
                {
                    LogisTrac.WriteLog("上传打印状态失败2");
                }
                return;
            }
        }

        private void result_upload(object sender, EventArgs e)
        {
            Thread.Sleep(20000);
            try
            {
                System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();

                string intValue = GetPrinterStatus(pd.PrinterSettings.PrinterName);

                //反馈打印结果
                if (!htt_printerState(REPORT_id, intValue))
                {
                    LogisTrac.WriteLog("上传打印状态失败");
                }
                else
                { LogisTrac.WriteLog("上传打印状态成功：" + REPORT_id); }

            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("上传打印状态失败1：" + ex.Message);
            }
        }

        #endregion

        #region 日志上传
        /// <summary>
        /// 间隔时间执行画面切换
        /// </summary>
        /// <param name="stepname"></param>
        /// <param name="time"></param>
        private void log_upload_delay()
        {
            try
            {
                if (Init_check_log_uploading)
                {
                    return;
                }
                Init_check_log_uploading = true;

                System.Timers.Timer t = new System.Timers.Timer(500);//实例化Timer类，设置时间间隔
                t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => log_upload(s, e));
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        private void log_upload(object sender, EventArgs e)
        {
            try
            {
                log_upload(DateTime.Now.AddDays(-1));
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void log_upload(DateTime date)
        {
            try
            {
                LogisTrac.WriteLog("上传日志开始");
                string LOG_DIR = LogisTrac.LOG_DIR;
                string nowlogfile = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                if (string.IsNullOrEmpty(yoyoConst.EQU_SN_LOG))
                {
                    return;
                }
                if (!htConst.checkEQUSN("上传日志"))
                {
                    return;
                }
                //遍历日志文件,所有内容都上传
                DirectoryInfo theFolder = new DirectoryInfo(yoyoConst.KANGJIA_PATH + "\\" + LOG_DIR);

                foreach (FileInfo file in theFolder.GetFiles())
                {
                    string path = "/yoyologs" + "/" + htConst.getEQUSN() + "/";// +file.Name.Replace(".log", "") + "/";

                    //当前天的日志是无法删除的,需要复制到临时文件夹,然后再处理
                    if (file.Name != nowlogfile)
                    {
                        bool flag = FtpUpLoadFiles.UploadFile(path, file.FullName, file.Name);
                        file.Delete();
                    }
                    else
                    {
                        string logfiletemp = yoyoConst.KANGJIA_MAIN_PATH + "\\" + file.Name;
                        file.CopyTo(logfiletemp, true);

                        bool flag = FtpUpLoadFiles.UploadFile(path, logfiletemp, file.Name);
                        File.Delete(logfiletemp);
                    }
                }
                LogisTrac.WriteLog("上传日志结束");

                Init_ComputerTime();//liudq 每晚设定时间
                Init_check_log_uploading = false;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        #endregion

        #region 长连接

        private bool longConnection(string sn)
        {
            try
            {
                LogisTrac.WriteLog("长连接创建开始...");
                if (_client != null && _client.IsConnected)
                {
                    return true;
                }
                //阿里云物联网套件登录信息
                string secretKey = yoyoConst.deviceSecret;  // 需要改成他们给的deviceSecret OperateIniFile.ReadIniData("LINK_ACCESSKEYSECRET");//
                string deviceName = yoyoConst.deviceName;                          // 需要改成他们给的deviceNameOperateIniFile.ReadIniData("LINK_ACCESSKEYID");//
                string productKey = OperateIniFile.ReadIniData("LINK_PRODUCTKEY"); //yoyoConst.productKey;                      // 需要改成他们给的productKey
                stopic = "/" + productKey + "/" + deviceName + "/get,/" + productKey + "/" + deviceName + "/data";  //需要改成他们给的订阅主题，多个的话用 , 分隔
                ptopic = "/" + productKey + "/" + deviceName + "/update";                         //需要改成他们给的发布主题，多个的话用 , 分隔

                string host = productKey + ".iot-as-mqtt.cn-shanghai.aliyuncs.com";
                int port = 1883;
                string timestamp = DateTime.Now.Millisecond + "";
                string clientId = sn;//实际应换成读取下位机SN
                string wclientId = clientId + "|securemode=3,signmethod=hmacsha1,timestamp=" + timestamp + "|";
                string username = deviceName + "&" + productKey;//账号的deviceName&productKey，在阿里云控制台查看
                String password = HMACSHA1(secretKey, "clientId" + clientId + "deviceName" + deviceName + "productKey" + productKey + "timestamp" + timestamp);

                _client = MqttClientFactory.CreateClient("tcp://" + host + ":" + port, wclientId, username, password, null);
                _client.Connected += new ConnectionDelegate(client_Connected);
                _client.ConnectionLost += new ConnectionDelegate(_client_ConnectionLost);
                _client.PublishArrived += new PublishArrivedDelegate(client_PublishArrived);
                _client.KeepAliveInterval = 80;

                try
                {
                    _client.Connect(true);
                    LogisTrac.WriteLog("长连接创建结束...");
                }
                catch { }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }

            if (_client.IsConnected)
            {//长连接失败信息消隐
                string flash2 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "检测升级..." + "<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
                axShockwaveFlash1.CallFunction(flash2);
            }

            return _client.IsConnected;
        }

        /// <summary>
        /// 取得服务器信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        /// 
        public bool client_PublishArrived(object sender, PublishArrivedArgs e)
        {


            try
            {
                string backstr = e.Payload.ToString();

                //if (backstr == "A002")//liudq 测试
                //{
                //    backstr = backstr.Replace("A002", "A009http://akso.wrmjk.com/pdf/2018/07/05/15/KJ101HB5S0572180704214818078.pdf");
                //}

                if (backstr == "A002")
                {
                    #region 解屏指令
                    if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.playing_qrcode)
                    {
                        //上报解开锁屏状态
                        retIFI02 model = htt_equState(htConst.YoyoEqu.testing_state);
                        if (model != null && model.success && model.data != null && model.data.userId != null)
                        {
                            yoyoConst.CURRTEN_USERNO = model.data.userId;
                            yoyoConst.age = model.data.userAge;
                            yoyoConst.faceid = "";
                            LogisTrac.WriteLog("扫码返回年龄为:" + yoyoConst.age);
                            if (model.data.userSex == "1.0" || model.data.userSex == "1")
                            {
                                yoyoConst.CURRTEN_SEX = "1";
                                flash_change(yoyoConst.YoyoStep.sex_man);
                                //程序播放语音
                                string flash = "<invoke name=\"playMusic\" returntype=\"xml\"><arguments><string>sex_male.mp3</string></arguments></invoke>";
                                axShockwaveFlash1.CallFunction(flash);
                            }
                            else
                            {
                                yoyoConst.CURRTEN_SEX = "0";
                                flash_change(yoyoConst.YoyoStep.sex_woman);
                                //程序播放语音
                                string flash = "<invoke name=\"playMusic\" returntype=\"xml\"><arguments><string>sex_female.mp3</string></arguments></invoke>";
                                axShockwaveFlash1.CallFunction(flash);
                            }
                            devicemanager.Write(new Cmd_S_Sex());
                        }
                        else
                        {
                            LogisTrac.WriteLog("解锁失败");
                            htt_equState(htConst.YoyoEqu.unlock_error);//解锁失败
                        }

                    }
                    else
                    {
                        //不可以解锁
                        htt_equState(htConst.YoyoEqu.cant_state);//设备正在使用中
                    }
                    #endregion
                }
                else
                {
                    LogisTrac.WriteLog("收到命令---" + backstr);
                    LinkList.Add(backstr);
                }

            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }

            return true;
        }

        private void client_Connected(object sender, EventArgs e)
        {
            if (!htConst.checkEQUSN("创建长连接"))
            {
                return;
            }
            // yoyoConst.WriteLog("长连接建立");
            // yoyoConst.WriteLog("长连接订阅主题：" + stopic);
            try
            {
                RegisterOurSubscriptions(stopic);
                PublishSomething(ptopic, htConst.getEQUSN() + " connected!");
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void _client_ConnectionLost(object sender, EventArgs e)
        {
            LogisTrac.WriteLog("长连接断开，开始重连。。。");


            if ((iweb_type == 1) || (iweb_type == 2))//查询网络状态
            {
                LogisTrac.WriteLog("长连接断开，检查网络");
                SendData2();
                LogisTrac.WriteLog("长连接断开，检查网络完成");
            }

            string flash1 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "长连接断开。。。"+ "\r" + net_flash + "<br/></font></string><true/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
            axShockwaveFlash1.CallFunction(flash1);

            try
            {
                _client.Connect(false);
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("重连失败：" + ex.Message);
                return;
            }

            LogisTrac.WriteLog("长连接重连完成");

            string flash2 = "<invoke name=\"show_info_tip\" returntype=\"xml\"><arguments><string><font size='" + OperateIniFile.ReadIniData("size_info") + "' color='" + OperateIniFile.ReadIniData("color_info") + "'>" + "检测升级..." + "<br/></font></string><false/><number>" + OperateIniFile.ReadIniData("x_info") + "</number><number>" + OperateIniFile.ReadIniData("y_info") + "</number><number>" + OperateIniFile.ReadIniData("width_info") + "</number><number>" + OperateIniFile.ReadIniData("height_info") + "</number></arguments></invoke>";
            axShockwaveFlash1.CallFunction(flash2);
        }

        private void RegisterOurSubscriptions(string topics)
        {
            try
            {
                if (topics.IndexOf(",") >= 0)
                {
                    string[] topica = topics.Split(',');
                    foreach (string atopic in topica)
                    {
                        try
                        {
                            _client.Subscribe(atopic, QoS.BestEfforts);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void PublishSomething(string topics, string message)
        {
            try
            {
                if (topics.IndexOf(",") >= 0)
                {
                    string[] topica = topics.Split(',');
                    foreach (string atopic in topica)
                    {
                        _client.Publish(atopic, message, QoS.BestEfforts, false);
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private static string HMACSHA1(string key, string dataToSign)
        {
            try
            {
                byte[] byteKey = UTF8Encoding.UTF8.GetBytes(key);
                HMACSHA1 myhmacsha1 = new HMACSHA1(byteKey);
                byte[] byteArray = Encoding.UTF8.GetBytes(dataToSign);
                // MemoryStream stream = new MemoryStream(byteArray);
                var hashValue = myhmacsha1.ComputeHash(byteArray);
                return hashValue.Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region http方法
        //打印机状态
        private bool htt_printerState(string reportCode, string status)
        {
            if (!htt_net("上传打印状态")) return false;
            if (!htConst.checkEQUSN("上传状态"))
            {
                return false;
            }
            try
            {
                string url = htConst.getUrlPara();
                url += "&reportCode=" + reportCode;
                url += "&status=" + status;//
                string parurl = yoyoConst.SERVER_IP + "/IFI/V1/P_01" + url;
                string ret = htConst.Post(parurl, "");

                htprinter model = JsonHelper.FromJSON<htprinter>(ret);

                if ((model.msg == "success") && (model.success == true))
                { return true; }
                else
                { return false; }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("上报打印状态失败:" + ex.Message);
                return false;
            }
        }

        //上传定位
        private bool htt_location()
        {
            if (!htt_net("上传位置信息")) return false;

            if (!htConst.checkEQUSN("上传状态"))
            {
                return false;
            }

            try
            {

                string parurl = yoyoConst.SERVER_IP + "/position/v1/amapPosition";


                htlocation req = new htlocation();

                req.robotSn = htConst.getEQUSN();
                req.acesstype = 0;
                req.bts = MCC + "," + MNC + "," + LAC + "," + Cell_ID + ",-60";

                req.cdma = 0;
                req.imei = "111";
                req.macs = "";
                req.mmac = "";
                req.nearbts = "";
                req.serviceip = "";
                req.smac = "";

                LogisTrac.WriteLog("上报基站信息:" + req.bts);

                string ret = htConst.Post(parurl, req);

                LogisTrac.WriteLog("上报位置返回值:" + ret);


                return true;

            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("上报位置失败:" + ex.Message);
                return false;
            }
        }

        private bool htt_WEBinfo()
        {
            if (!htt_net("上传web信息")) return false;

            if (!htConst.checkEQUSN("上传web状态"))
            {
                return false;
            }

            try
            {

                string parurl = yoyoConst.SERVER_IP + "/base/V1/INF_01";


                htweb req = new htweb();

                req.deviceSN = htConst.getEQUSN();
                req.lteCardNumber = CARD_NO;
                req.@operator = YYS;

                if (Cell_ID != "")
                { req.state = "OK"; }

                if (iweb_type == 1)
                { req.supplier = "huawei"; }

                if (iweb_type == 2)
                { req.supplier = "ce20"; }

                req.timeStamp = DateTime.Now.ToString("yyyyMMddHHssmm");

                LogisTrac.WriteLog("上报网络信息:卡号：" + req.lteCardNumber + "  4g模块供应商：" + req.supplier);

                string ret = htConst.Post(parurl, req);

                LogisTrac.WriteLog("上报网络返回值:" + ret);


                return true;

            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog("上报web失败:" + ex.Message);
                return false;
            }
        }


        private void htt_equStateThread()
        {
            if (!htt_net("上传状态")) return;
            htt_equState(htConst.YoyoEqu.lock_state);
        }

        /// <summary>
        /// 设定设备状态
        /// </summary>
        private retIFI02 htt_equState(htConst.YoyoEqu equstate)
        {
            if (!htt_net("上传状态")) return null;
            if (!htConst.checkEQUSN("上传状态"))
            {
                return null;
            }
            try
            {
                string url = htConst.getUrlPara();
                url += "&msg";
                url += "&deviceStatus=" + ((int)equstate).ToString();
                string parurl = htConst.url_IFI_02 + url;
                string ret = htConst.Post(parurl, "");

                retIFI02 model = JsonHelper.FromJSON<retIFI02>(ret);
                return model;
            }
            catch
            {
                LogisTrac.WriteLog("上报状态失败");
                return null;
            }
        }

        /// <summary>
        /// 上报开机状态，上网途径
        /// </summary>
        private void htt_netrootCheck()
        {
            if (!htt_net("上报开机状态")) return;
            try
            {
                //取得上网途径
                int falg = Internet.LocalConnectionStatus();
                if (falg == 0)
                {
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private bool htt_long()
        {
            if (!htt_net("创建长连接")) return false;

            if (!htConst.checkEQUSN("创建长连接"))
            {
                return false;
            }
            try
            {
                string url = "?token=" + htConst.getToken();
                //设备锁屏二维码请求接口
                string parurl = htConst.url_IFI_2_03 + url;
                htIV2_IFI03 req = new htIV2_IFI03();
                req.deviceSN = htConst.getEQUSN();
                req.deviceType = "4G";
                req.productKey = OperateIniFile.ReadIniData("LINK_PRODUCTKEY");//"ukbEPYoprL6";
                req.timeStamp = DateTime.Now.ToString("yyyyMMddHHssmm");
                string ret = htConst.Post(parurl, req);

                retV2IFI03 model = JsonHelper.FromJSON<retV2IFI03>(ret);
                if (model != null && model.data != null && model.data.mqtt != null)
                {
                    yoyoConst.deviceName = model.data.mqtt.deviceName;
                    yoyoConst.productKey = model.data.mqtt.productKey;
                    yoyoConst.deviceSecret = model.data.mqtt.deviceSecret;
                    yoyoConst.BAR_URL = model.data.qrUrl;
                }
                else
                {
                    LogisTrac.WriteLog("长连接启动失败");
                    return false;
                }

                if (_client == null || !_client.IsConnected)
                {
                    //创建长连接
                    return longConnection(htConst.getEQUSN());
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 设备信息接口
        /// </summary>
        private void htt_barcode()
        {
            LogisTrac.WriteLog("开始转二维码界面");
            //重置返回线程
            //this.indexformtimer.Enabled = false;
            //this.indexformtimer.Enabled =true;

            this.FACE_CHECK = this.FACE_CHECK_COUNT;//liudq  进入二维码界面就重置

            if (!htt_net("获取二维码")) return;

            if (!htConst.checkEQUSN("获取二维码"))
            {
                return;
            }
            try
            {

                if (string.IsNullOrEmpty(yoyoConst.BAR_URL))
                {
                    this.htt_long();
                    flash_change(yoyoConst.YoyoStep.playing_qrcode, yoyoConst.BAR_URL);
                    Camera_change = false;//进入二维码界面归false；
                    ichange = 0;//进入二维码界面归0；

                }
                else if (Internet.IsConnectInternet())
                {
                    flash_change(yoyoConst.YoyoStep.playing_qrcode, yoyoConst.BAR_URL);
                    Camera_change = false;//进入二维码界面归false；
                    ichange = 0;//进入二维码界面归0；
                }

                if (_client == null || !_client.IsConnected)
                {
                    //创建长连接
                    longConnection(htConst.getEQUSN());
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        /// <summary>
        /// 设备注册激活
        /// </summary>
        private bool htt_Register()
        {
            if (!htt_net("设备注册激活")) return false;
            if (!htConst.checkEQUSN("设备注册激活"))
            {
                return false;
            }
            try
            {
                retV1UploadU01 upmodel = htt_Upload();

                if (upmodel == null || upmodel.data == null) return false;
                string url = "?token=" + htConst.getToken();
                string parurl = htConst.url_REGISTER + url;


                htRegisterV1R01 req = new htRegisterV1R01();
                req.deviceSN = htConst.getEQUSN();
                req.deviceMac = yoyoConst.MAC;
                req.type = yoyoConst.EQU_TYPE;
                req.timeStamp = DateTime.Now.ToString("yyyyMMddHHssmm");
                req.appVersion = upmodel.data.appVersion;
                req.mcpversion = upmodel.data.mcpVersion;
                req.resVersion = upmodel.data.resVersion;
                string ret = htConst.Post(parurl, req);
                //注册成功
                htRetBasemodel model = JsonHelper.FromJSON<htRetBasemodel>(ret);
                bool falg = false;
                if (model != null && model.success == "true")
                {
                    falg = true;
                }
                return falg;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 设备升级
        /// </summary>
        private retV1UploadU01 htt_Upload()
        {
            if (!htt_net("设备升级")) return null;
            if (!htConst.checkEQUSN("设备升级"))
            {
                return null;
            }
            try
            {

                string url = "?token=" + htConst.getToken();
                string parurl = htConst.url_V1_UPLOAD + url;

                htV1UploadU01 req = new htV1UploadU01();
                req.deviceSN = yoyoConst.EQU_SN; //htConst.getEQUSN(); liudq
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
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return null;
            }

        }

        private string htt_getRadom()
        {
            try
            {
                Random rd = new Random();

                int x = rd.Next(1, 100);

                return x.ToString().PadLeft(2, '0');
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return "";
            }
        }

        private bool htt_net(string stepname)
        {
            if (!this.Init_Check_Net)
            {
                LogisTrac.WriteLog("网络不通，无法继续" + stepname);
                return false;
            }
            return true;
        }
        #endregion

        #region 升级完成，重启main
        private void update_start()
        {
            try
            {
                // 升级完成，启动主程序
                string StrExe = yoyoConst.KANGJIA_MAIN_PATH + "kangjiamain.exe";
                if (File.Exists(StrExe))
                {
                    System.Diagnostics.Process.Start(StrExe);
                }

                //关闭升级程序
                System.Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        /// <summary>
        /// 检测升级
        /// </summary>
        private bool update_Check()
        {
            try
            {
                retV1UploadU01 model = htt_Upload();
                if (model != null)
                {
                    string res_verion = OperateIniFile.ReadVersionIniData("RES_VERSION");
                    string main_verion = OperateIniFile.ReadVersionIniData("MAIN_VERSION");
                    if (model.data.resVersion != null && res_verion != null && model.data.resVersion.Trim() != res_verion.Trim())
                    {
                        return true;
                    }
                    if (model.data.appVersion != null && main_verion != null && model.data.appVersion.Trim() != main_verion.Trim())
                    {
                        return true;
                    }
                    string[] list1 = new string[2];//yoyoConst.VERSION.Split('H');
                    list1[0] = yoyoConst.VERSION.Substring(0, 9);
                    list1[1] = yoyoConst.VERSION.Substring(9);
                    string[] list2 = new string[2];//model.data.mcpVersion.Split('H');
                    list2[0] = model.data.mcpVersion.Trim().Substring(0, 9);
                    list2[1] = model.data.mcpVersion.Trim().Substring(9);
                    if (list1.Length == 2 && list1.Length == list2.Length && list1[0] != list2[0] && list1[1] == list2[1])
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }
        }
        #endregion

        #region 升级kangjiamain

        /// <summary>
        /// 升级kangjiamain程序
        /// </summary>
        /// <returns></returns>
        private bool update_kangjiamain()
        {
            try
            {
                retV1UploadU01 model = htt_Upload();
                bool doflag = false;
                string basth = yoyoConst.KANGJIA_MAIN_PATH;// +"\\test\\"; //解压后复制到的文件路径

                if (model != null)
                {

                    string main_verion = OperateIniFile.ReadIniData("MAIN_VERSION");

                    if (model.data.bootloaderVersion != null && main_verion != null && model.data.bootloaderVersion.Trim() != main_verion.Trim())
                    {
                        string newversion = model.data.bootloaderVersion;
                        //下载解压
                        doflag = update_downloadAndUnZipFile(model.data.bootloaderUrl, this.up_resVersion_url);
                        if (!doflag)
                        {
                            LogisTrac.WriteLog("kangjia--kiangjiamain文件下载解压失败,版本" + newversion);
                            return doflag;
                        }
                        LogisTrac.WriteLog("kangjia--kiangjiamain文件下载解压成功" + newversion + "end");
                        //复制到指定目录，删除原始文件
                        //因为压缩文件里面包含一层目录,需要将该目录下的所有信息复制到指定位置
                        string copypath = getDirectoryNext(this.up_resVersion_url);
                        doflag = update_replaceFile(copypath, basth);
                        if (!doflag)
                        {
                            LogisTrac.WriteLog("kangjia--kiangjiamain文件替换失败,版本" + newversion + "end");
                            return doflag;
                        }
                        FileHelper.DeleteFolder(this.up_resVersion_url);
                        OperateIniFile.WriteIniData("MAIN_VERSION", newversion);
                        LogisTrac.WriteLog("kangjia--kiangjiamain文件更新成功,版本" + newversion + "end");
                        return true;
                    }
                    else
                    {
                        LogisTrac.WriteLog("kangjia升级kiangjiamain失败,原因：服务器版本：" + model.data.bootloaderVersion + "  本地版本：" + main_verion);
                        if (model.data.bootloaderVersion != null)
                        {
                            OperateIniFile.WriteIniData("MAIN_VERSION", model.data.bootloaderVersion);
                        }
                    }


                    return false;
                }
                else
                {
                    LogisTrac.WriteLog("kangjia升级kiangjiamain失败,原因：获取服务器main版本出错");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
                return false;
            }
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
                //解压
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
        //解压
        private bool update_Unzip(string filename, string tagetpath)
        {
            try
            {
                ExtractOptions option = ExtractOptions.None;
                //option = ExtractOptions.None;
                option = ExtractOptions.ExtractFullPath;//是表示连目录解压

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

        //获得指定路径下所有子目录名
        private string getDirectoryNext(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                return d.FullName;
            }
            return "";
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
        private void flash_change(yoyoConst.YoyoStep step, params string[] para)
        {
            try
            {


                if (yoyoConst.currtenStep != null && step == yoyoConst.currtenStep.step && step != yoyoConst.YoyoStep.testing)
                {
                    //首页点击按钮无效
                    this.FLASH_CHANGING = false;
                    return;
                }


                GC.Collect();
                if (FLASH_CHANGING)
                {
                    return;
                }

                flash_startIndexTimer();

                // GC.Collect();

                this.FLASH_CHANGING = true;
                flashmodel mode = new flashmodel();
                bool falg = yoyoConst.dict.TryGetValue(step, out mode);
                yoyoConst.currtenStep = mode;

                string url = mode.url;
                if (para != null && para.Length > 0)
                {
                    url = String.Format(mode.url, para[0]);
                }
                if (yoyoConst.currtenStep.step != yoyoConst.YoyoStep.testing)
                {
                    this.Testing = false;
                }

                if (falg)
                {
                    if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.yoyo)
                    {
                        //上报锁屏状态
                        Thread mythread = new Thread(htt_equStateThread);
                        mythread.Start();
                        //通知下位机
                        devicemanager.Write(new Cmd_S_Lock());
                    }

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
                                flash_change(yoyoConst.YoyoStep.yoyo);
                                this.FLASH_CHANGING = false;
                                LogisTrac.WriteLog("画面转换错误");
                                return;
                            }
                        }

                    }
                }
                #region 根据不同的画面，设定画面显示状态，照相机是否显示

                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_man
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_woman
                    )
                {
                    this.cameraControl.Visible = true;
                }
                else
                {
                    this.cameraControl.Visible = false;
                }

                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.yoyo)
                {
                    yoyoConst.CURRTEN_SEX = "1";
                    yoyoConst.CURRTEN_USERNO = "";
                }



                #endregion
                this.FLASH_CHANGING = false;
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }

        private void flash_startIndexTimer()
        {
            try
            {
                bool indexflag = true;

                if (yoyoConst.currtenStep != null)
                {
                    if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.testing
                      || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading
                      || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.upgrade
                      || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.result
                      )
                    {
                        indexflag = false;
                    }
                    else
                    {
                        indexflag = true;

                    }
                }


                if (indexformtimer != null)
                {
                    try
                    {
                        indexformtimer.Enabled = false;
                    }
                    catch (Exception ex)
                    { LogisTrac.WriteLog("终止线程失败：" + ex.Message); }

                    //非测量状态时，启动线程
                    if (indexformtimer != null && indexflag)
                    {
                        LogisTrac.WriteLog("重新启动线程");
                        
                        indexformtimer.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        /// <summary>
        /// 间隔时间执行画面切换
        /// </summary>
        /// <param name="stepname"></param>
        /// <param name="time"></param>
        private void flash_change_delay(yoyoConst.YoyoStep stepname, int time)
        {
            try
            {
                LogisTrac.WriteLog("开始画面延时" + time);
                System.Timers.Timer t = new System.Timers.Timer(time);//实例化Timer类，设置时间间隔
                t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => flash_change_delay_fun(s, e, stepname));
                t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        private void flash_change_delay_fun(object source, System.Timers.ElapsedEventArgs e, yoyoConst.YoyoStep stepname)
        {
            try
            {
                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.result
                    || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.uploading_error
                    )
                {
                    flash_change(stepname);
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        /// <summary>
        /// 性别选择
        /// </summary>
        private void flash_changeSex()
        {
            try
            {
                if (yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_man || yoyoConst.currtenStep.step == yoyoConst.YoyoStep.sex_woman)
                {
                    yoyoConst.CURRTEN_SEX = yoyoConst.CURRTEN_SEX == "0" ? "1" : "0";

                    if (yoyoConst.CURRTEN_SEX == "0")
                    {
                        flash_change(yoyoConst.YoyoStep.sex_woman);
                        //程序播放语音
                        string flash = "<invoke name=\"playMusic\" returntype=\"xml\"><arguments><string>sex_female2.mp3</string></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);
                    }
                    if (yoyoConst.CURRTEN_SEX == "1")
                    {
                        flash_change(yoyoConst.YoyoStep.sex_man);
                        //程序播放语音
                        string flash = "<invoke name=\"playMusic\" returntype=\"xml\"><arguments><string>sex_male2.mp3</string></arguments></invoke>";
                        axShockwaveFlash1.CallFunction(flash);
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        #endregion

        #region 重启网络
        //重启网络适配器
        private void restartNet()
        {
            if ((iweb_type == 1) || (iweb_type == 2))//取得4g版本则重启模块
            {
                SendData1();
                return;
            }

            // Thread.Sleep(60000);

            string cmdStr = "netsh interface set interface \"{0}\" {1} &exit";

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string name = "移动宽带连接";
            //string name = "\"WLAN\"";//TODO

            foreach (NetworkInterface adapter in adapters)
            {
                int i = adapter.Name.IndexOf("移动宽带连接");
                if (i >= 0)//
                {
                    name = adapter.Name;
                }

            }

            string str = "";
            str = string.Format(cmdStr, name, "disable");
            RunCmd(str);

            Thread.Sleep(1000);

            str = string.Format(cmdStr, name, "enable");
            RunCmd(str);


        }

        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdStr">执行命令行参数</param>
        private bool RunCmd(string cmdStr)
        {
            bool result = false;
            //string cmdStr = "netsh interface set interface WLAN disable &exit";
            try
            {
                LogisTrac.WriteLog("启动命令" + cmdStr + "开始");
                using (Process myPro = new Process())
                {
                    myPro.StartInfo.FileName = "cmd.exe";
                    myPro.StartInfo.UseShellExecute = false;
                    myPro.StartInfo.RedirectStandardInput = true;
                    myPro.StartInfo.RedirectStandardOutput = true;
                    myPro.StartInfo.RedirectStandardError = true;
                    myPro.StartInfo.CreateNoWindow = true;
                    myPro.Start();
                    //如果调用程序路径中有空格时，cmd命令执行失败，可以用双引号括起来 ，在这里两个引号表示一个引号（转义）
                    //string str = string.Format(@"""{0}"" {1} {2}", cmdExe, cmdStr, "&exit");

                    myPro.StandardInput.WriteLine(cmdStr);
                    myPro.StandardInput.AutoFlush = true;
                    myPro.WaitForExit();
                    myPro.Dispose();
                    result = true;
                }
            }
            catch (Exception e)
            {
                LogisTrac.WriteLog("启动命令" + cmdStr + "异常");
                LogisTrac.WriteLog(e.Message);

            }
            return result;
        }

        #endregion

        #region 打印机状态
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct structPrinterDefaults
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public String pDatatype;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.I4)]
            public int DesiredAccess;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pServerName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pShareName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPortName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pComment;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pSepFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDatatype;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }


        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true,
    ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool GetPrinter(
         IntPtr hPrinter,
         int dwLevel,
         IntPtr pPrinter,
         int dwBuf,
         out int dwNeeded
         );



        [DllImport("winspool.Drv", EntryPoint = "OpenPrinter", SetLastError = true,
    CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
  SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPTStr)]
   string printerName,
         out IntPtr phPrinter,
         ref structPrinterDefaults pd);




        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true,
    CharSet = CharSet.Unicode, ExactSpelling = false,
    CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool ClosePrinter(IntPtr phPrinter);



        public static string GetPrinterStatus(string PrinterName)
        {
            int intValue = GetPrinterStatusInt(PrinterName);

            string strRet = string.Empty;
            switch (intValue)
            {
                case 0://打印成功，打印机处于准备状态
                    strRet = "P00";
                    break;
                case 0x00000200:
                    strRet = "P04";//忙
                    break;
                case 0x00400000:
                    strRet = "P05";//被打开
                    break;
                case 0x00000002:
                    strRet = "P06";//错误
                    break;
                case 0x0008000:
                    strRet = "P07";//初始化
                    break;
                case 0x00000100:
                    strRet = "P08";//正在输入,输出
                    break;
                case 0x00000020:
                    strRet = "P09";//手工送纸
                    break;
                case 0x00040000:
                    strRet = "P10";//无墨粉
                    break;
                case 0x00001000:
                    strRet = "P12";//不可用
                    break;
                case 0x00000080:
                    strRet = "P01";//脱机
                    break;
                case 0x00200000:
                    strRet = "P13";//内存溢出
                    break;
                case 0x00000800:
                    strRet = "P14";//输出口已满
                    break;
                case 0x00080000:
                    strRet = "P15";//当前页无法打印
                    break;
                case 0x00000008:
                    strRet = "P03";//塞纸
                    break;
                case 0x00000010:
                    strRet = "P02";//打印纸用完
                    break;
                case 0x00000040:
                    strRet = "P16";//纸张问题
                    break;
                case 0x00000001:
                    strRet = "P17";//暂停
                    break;
                case 0x00000004:
                    strRet = "P18";//正在删除
                    break;
                case 0x00000400:
                    strRet = "P00";//正在打印
                    break;
                case 0x00004000:
                    strRet = "P00";//正在处理
                    break;
                case 0x00020000:
                    strRet = "P19";//墨粉不足
                    break;
                case 0x00100000:
                    strRet = "P21";//需要用户干预
                    break;
                case 0x20000000:
                    strRet = "P22";//等待
                    break;
                case 0x00010000:
                    strRet = "P23";//热机中
                    break;
                default:
                    strRet = "P20";//未知状态
                    break;
            }
            return strRet;
        }


        internal static int GetPrinterStatusInt(string PrinterName)
        {
            int intRet = 0;
            IntPtr hPrinter;
            structPrinterDefaults defaults = new structPrinterDefaults();

            if (OpenPrinter(PrinterName, out hPrinter, ref defaults))
            {
                int cbNeeded = 0;
                bool bolRet = GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out cbNeeded);

                if (cbNeeded > 0)
                {
                    IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
                    bolRet = GetPrinter(hPrinter, 2, pAddr, cbNeeded, out cbNeeded);
                    if (bolRet)
                    {
                        PRINTER_INFO_2 Info2 = new PRINTER_INFO_2();

                        Info2 = (PRINTER_INFO_2)Marshal.PtrToStructure(pAddr, typeof(PRINTER_INFO_2));

                        intRet = System.Convert.ToInt32(Info2.Status);
                    }
                    Marshal.FreeHGlobal(pAddr);
                }
                ClosePrinter(hPrinter);
            }

            return intRet;
        }
        #endregion


    }
}
