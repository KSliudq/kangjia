using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
namespace kangjiabase
{
    public static class yoyoConst
    {
        //版本号
        public static string VERSION = "";
        //设备sn编号
        public static string EQU_SN = "";
        //设备sn编号,写日志用
        public static string EQU_SN_LOG = "";
        //设备sn编号
        public static string EQU_TYPE = "";
        //mac
        public static string MAC = "";
        #region 从Ini读取
        /// <summary>
        /// 服务器地址
        /// </summary>
        public static string SERVER_IP = "";

        public static string QR_URL = "";

        public static string PRINT = "0";

        public static int FACE_LENGTH = 4;//检测面部距离 4-6

        public static int HAND_LEAVE_TIME = 100;//双手离开多长时间返回首页

        public static int FINISH_TIME = 3000;//报告上传结束后多长时间返回首页

        public static int INDEX_TIME = 20 * 1000;//返回首页的时间
        #endregion
        public static string SPLIT = " ";
        //当前选中性别
        public static string CURRTEN_SEX = "1";

        public static string age = "0";
        //当前用户编号
        public static string CURRTEN_USERNO = "";

        public static string BAR_URL = "";

        public static bool DEBUG = true;

        public static double FILE_LEN = 128;//更新时的每个包的大小

        public static DateTime TOKEN_EXP_TIME;//token过期时间

        public static string TOKEN;

        public static string deviceName;
        public static string productKey;
        public static string deviceSecret;

        public static string KANGJIA_PATH;
        public static string KANGJIA_MAIN_PATH;

        public static string faceid;

        private static string getPath()
        {
            //AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.LastIndexOf(@"\")) 
            //string path = new DirectoryInfo("../").FullName;//当前应用程序路径的上级目录
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        /// <summary>
        /// 当前画面
        /// </summary>
        public static flashmodel currtenStep;
        public enum YoyoStep : int
        {
            yoyo = 1,
            sex_man = 2,
            sex_woman = 3,
            testing = 4,
            playing = 5,
            connect_error = 6,
            playing_qrcode = 7,
            uploading = 8,
            uploading_error = 9,
            result = 10,
            upgrade = 11,
            upgrade_error = 12,
            version = 13,
            exception = 14,
            staring = 15
        }


        public static Dictionary<YoyoStep, flashmodel> dict = new Dictionary<YoyoStep, flashmodel>();
        /// <summary>
        /// 初始化项目
        /// </summary>
        public static void Initproject(bool flag)
        {

            if (flag)
            {
                KANGJIA_PATH = AppDomain.CurrentDomain.BaseDirectory;
                System.IO.Directory.SetCurrentDirectory(KANGJIA_PATH);
                if (KANGJIA_PATH.LastIndexOf("\\kangjia\\") > 0)
                {
                    KANGJIA_MAIN_PATH = KANGJIA_PATH.Substring(0, KANGJIA_PATH.LastIndexOf("\\kangjia\\")) + "\\";
                }
                SERVER_IP = OperateIniFile.ReadIniData("HTTPSERVER_IP");
                QR_URL = OperateIniFile.ReadIniData("QR_URL");


                PRINT = OperateIniFile.ReadIniData("PRINT");// OperateIniFile.ReadIniData("QR_URL");

                LogisTrac.WriteLog(" KANGJIA_MAIN_PATH--" + KANGJIA_MAIN_PATH);
                LogisTrac.WriteLog(" KANGJIA_PATH--" + KANGJIA_PATH);
                try
                {
                    FACE_LENGTH = Convert.ToInt32(OperateIniFile.ReadIniData("FACE_LENGTH"));
                }
                catch { }
                try
                {
                    HAND_LEAVE_TIME = Convert.ToInt32(OperateIniFile.ReadIniData("HAND_LEAVE_TIME"));
                }
                catch { }


                try
                {
                    FINISH_TIME = Convert.ToInt32(OperateIniFile.ReadIniData("FINISH_TIME")) * 1000;
                }
                catch { }
                try
                {
                    INDEX_TIME = Convert.ToInt32(OperateIniFile.ReadIniData("INDEX_TIME")) * 1000;
                }
                catch { }
                try
                {
                    //初始化面部识别引擎
                    if (!AFDFunction.initEng())
                    {
                    }
                }
                catch (Exception ex)
                {
                    LogisTrac.WriteLog(ex);
                }
                try
                {
                    yoyoConst.EQU_SN_LOG = OperateIniFile.ReadSNIniData("EQU_SN_LOG");
                    yoyoConst.initEquType(yoyoConst.EQU_SN_LOG);

                }
                catch
                {

                }
            }
            else
            {
                KANGJIA_PATH = AppDomain.CurrentDomain.BaseDirectory + "\\kangjia\\";
                KANGJIA_MAIN_PATH = AppDomain.CurrentDomain.BaseDirectory;
                SERVER_IP = OperateIniFile.ReadVersionIniData("HTTPSERVER_IP");
                try
                {
                    yoyoConst.EQU_SN_LOG = OperateIniFile.ReadSNIniData("EQU_SN_LOG");
                    yoyoConst.initEquType(yoyoConst.EQU_SN_LOG);
                }
                catch
                {
                }
            }
            //设定flash的信息
            initFlash();

            MAC = NetTools.GetMAC();

        }
        public static void initEquType(string equsn)
        {
            if (string.IsNullOrEmpty(equsn)) return;




            if (equsn.IndexOf("temp") != -1)
            {
                yoyoConst.EQU_TYPE = "KJ103";
                return;
            }


            if (equsn == "KJ00001")
            {
                yoyoConst.EQU_TYPE = "KJ103";
                return;
            }

            if (equsn.Substring(0, 1) == "D")
            {
                yoyoConst.EQU_TYPE = "KJ103";
                return;
            }

            if ((equsn.Length < 6)) return;// && (equsn.IndexOf("temp") == -1)

            yoyoConst.EQU_TYPE = equsn.Substring(0, 5);


            if (yoyoConst.EQU_TYPE == "KJ101")
            {
                yoyoConst.EQU_TYPE = "KJ103";
            }
            else if (yoyoConst.EQU_TYPE == "KJ102")
            {
                yoyoConst.EQU_TYPE = "KJ104";
            }

        }

        private static void initFlash()
        {
            //无效<invoke name=\"button\" returntype=\"xml\"></invoke> 
            //无效<invoke name=\"version\" returntype=\"xml\"><arguments><string>{0}</string></arguments></invoke>
            //<invoke name=\"connect_error\" returntype=\"xml\"><arguments><string>{0}</string></arguments></invoke>
            //<invoke name=\"playing\" returntype=\"xml\"></invoke>
            //<invoke name=\"sex_select\" returntype=\"xml\"><arguments><true/></arguments></invoke>
            //<invoke name=\"sex_select\" returntype=\"xml\"><arguments><false/></arguments></invoke>
            //<invoke name=\"playingqrcode\" returntype=\"xml\"><arguments><string>{0}</string></arguments></invoke>
            //<invoke name=\"testing\" returntype=\"xml\"><arguments><number>{0}</number></arguments></invoke>
            //<invoke name=\"uploading\" returntype=\"xml\"></invoke>
            //<invoke name=\"uploading_error\" returntype=\"xml\"><arguments><string>{0}</string></arguments></invoke>
            //<invoke name=\"result\" returntype=\"xml\"></invoke>
            //<invoke name=\"facein\" returntype=\"xml\"></invoke>


            flashmodel flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.yoyo;
            flashmodel.flashname = "playing";
            flashmodel.url = "<invoke name=\"playing\" returntype=\"xml\"></invoke>";
            flashmodel.muiscname = "yoyo";
            dict.Add(flashmodel.step, flashmodel);

            //flashmodel = new flashmodel();
            //flashmodel.step = YoyoStep.facein;
            //flashmodel.flashname = "facein";
            //flashmodel.url = "<invoke name=\"facein\" returntype=\"xml\"></invoke>";
            //flashmodel.muiscname = "facein";
            //dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.sex_man;
            flashmodel.flashname = "sex_select";
            flashmodel.url = "<invoke name=\"sex_select\" returntype=\"xml\"><arguments><true/></arguments></invoke>";
            flashmodel.muiscname = "sex_male";
            dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.sex_woman;
            flashmodel.flashname = "sex_select";
            flashmodel.url = "<invoke name=\"sex_select\" returntype=\"xml\"><arguments><false/></arguments></invoke>";
            flashmodel.muiscname = "sex_female";
            dict.Add(flashmodel.step, flashmodel);


            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.testing;
            flashmodel.flashname = "testing_female";
            flashmodel.url = "<invoke name=\"testing\" returntype=\"xml\"><arguments><number>{0}</number></arguments></invoke>";
            flashmodel.muiscname = "didi";
            dict.Add(flashmodel.step, flashmodel);

            //flashmodel = new flashmodel();
            //flashmodel.step = YoyoStep.testing_woman;
            //flashmodel.flashname = "testing_male";
            //flashmodel.url = "<invoke name=\"testing\" returntype=\"xml\"><arguments><number>{0}</number></arguments></invoke>";
            //flashmodel.muiscname = "start_male";
            //dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.playing;
            flashmodel.flashname = "playing";
            flashmodel.url = "<invoke name=\"testing\" returntype=\"xml\"><arguments><number>{0}</number></arguments></invoke>";
            flashmodel.muiscname = "testing";
            dict.Add(flashmodel.step, flashmodel);


            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.connect_error;
            flashmodel.flashname = "connect_error";
            flashmodel.url = "<invoke name=\"connect_error\" returntype=\"xml\"><arguments><string></string></arguments></invoke>";
            flashmodel.muiscname = "connect_error";
            dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.playing_qrcode;
            flashmodel.flashname = "playing_qrcode";
            flashmodel.url = "<invoke name=\"playingqrcode\" returntype=\"xml\"><arguments><string>{0}</string></arguments></invoke>";
            flashmodel.muiscname = "playing_qrcode";
            dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.uploading;
            flashmodel.flashname = "uploading";
            flashmodel.url = "<invoke name=\"uploading\" returntype=\"xml\"></invoke>";
            flashmodel.muiscname = "test_uploading";
            dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.uploading_error;
            flashmodel.flashname = "uploading_error";
            //flashmodel.url = "<invoke name=\"uploading_error\" returntype=\"xml\"></invoke>";
            flashmodel.url = "<invoke name=\"uploading_error\" returntype=\"xml\"><arguments><string></string></arguments></invoke>";
            flashmodel.muiscname = "uploading_error";
            dict.Add(flashmodel.step, flashmodel);


            if ((PRINT == "1")|| (PRINT == "2"))
            {
                flashmodel = new flashmodel();
                flashmodel.step = YoyoStep.result;
                flashmodel.flashname = "result";
                flashmodel.url = "<invoke name=\"result\" returntype=\"xml\"><arguments><string>{0}</string></arguments></invoke>";
                flashmodel.muiscname = "test_result2";
                dict.Add(flashmodel.step, flashmodel);
            }
            if (PRINT == "0")
            {
                flashmodel = new flashmodel();
                flashmodel.step = YoyoStep.result;
                flashmodel.flashname = "result";
                flashmodel.url = "<invoke name=\"result\" returntype=\"xml\"></invoke>";
                flashmodel.muiscname = "test_result2";
                dict.Add(flashmodel.step, flashmodel);
            }

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.upgrade;
            flashmodel.flashname = "upgrade";
            flashmodel.url = "<invoke name=\"upgrade\" returntype=\"xml\"></invoke>";
            flashmodel.muiscname = "upgrade";
            dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.upgrade_error;
            flashmodel.flashname = "upgrade_error";
            flashmodel.url = "<invoke name=\"upgrade_error\" returntype=\"xml\"></invoke>";
            flashmodel.muiscname = "upgrade_error";
            dict.Add(flashmodel.step, flashmodel);

            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.version;
            flashmodel.flashname = "version";
            flashmodel.url = "<invoke name=\"version\" returntype=\"xml\"><arguments><string>v1.1.25 v1.1.1</string></arguments></invoke> ";
            flashmodel.muiscname = "version";
            dict.Add(flashmodel.step, flashmodel);



            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.exception;
            flashmodel.flashname = "exception";
            flashmodel.url = "<invoke name=\"exception\" returntype=\"xml\"><arguments><number>{0}</number></arguments></invoke>";
            flashmodel.muiscname = "exception";
            dict.Add(flashmodel.step, flashmodel);


            flashmodel = new flashmodel();
            flashmodel.step = YoyoStep.staring;
            flashmodel.flashname = "staring";
            flashmodel.url = "<invoke name=\"staring\" returntype=\"xml\"></invoke>";
            flashmodel.muiscname = "staring";
            dict.Add(flashmodel.step, flashmodel);
        }

        private static readonly log4net.ILog m_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary> 
        /// 输出日志
        /// </summary> 
        /// <param name="message"> 日志信息</param> 
        public static void WriteLog(string message)
        {

            LogisTrac.WriteLog(message);
        }



    }
}
