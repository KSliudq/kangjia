using System;
using System.Collections.Generic;
using System.Text;
//方法一
using System.Runtime;
using System.Runtime.InteropServices;
//方法二 Net2.0新增类库
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace kangjiabase
{
    public class Internet
    {
        //判断网络是否通畅
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(int Description, int ReservedValue);

        [DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
        #region 方法一
        /// <summary>
        /// 用于检查网络是否可以连接互联网,true表示连接成功,false表示连接失败 
        /// </summary>
        /// <returns></returns>
        public static bool IsConnectInternet()
        {
            int Description = 0;
            return InternetGetConnectedState(Description, 0);
            //string ip = yoyoConst.SERVER_IP.ToLower().Replace("http://", "");
            //if (ip.IndexOf(":") > 0)
            //{
            //    ip = ip.Substring(0, ip.IndexOf(":"));
            //}
            //return PingIpOrDomainName(ip);
        }
        #endregion

        #region 方法二  不好用，用方法3
        /// <summary>
        /// 用于检查IP地址或域名是否可以使用TCP/IP协议访问(使用Ping命令),true表示Ping成功,false表示Ping失败 
        /// </summary>
        /// <param name="strIpOrDName">输入参数,表示IP地址或域名</param>
        /// <returns></returns>
        public static bool PingIpOrDomainName(string strIpOrDName)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 120;// 120;
                PingReply objPinReply = objPingSender.Send(strIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion


        #region 方法三
        /// <summary>
        /// 运行cmd命令
        /// 不显示命令窗口
        /// </summary>
        /// <param name="cmdStr">执行命令行参数</param>
        public static bool RunCmd_Ping(string strIp)
        {
            bool result = false;
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardInput.WriteLine("ping -n 1 " + strIp);
                p.StandardInput.WriteLine("exit");

                string strRst = p.StandardOutput.ReadToEnd();

                if (strRst.IndexOf("(0% 丢失)") != -1)
                    result = true;

                p.Close();

                if (!result)
                    LogisTrac.WriteLog("ping失败：" + strRst);
            }
            catch (Exception e)
            {
                LogisTrac.WriteLog("启动命令" + strIp + "异常");
                LogisTrac.WriteLog(e.Message);

            }
            return result;
        }
        #endregion

        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;

        /// <summary>
        /// 判断本地的连接状态TODO
         /// </summary>
        /// <returns></returns>
        public static int LocalConnectionStatus()
        {
            System.Int32 dwFlag = new Int32();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
               // Console.WriteLine("LocalConnectionStatus--未连网!");
                return 0;
            }
            else
            {
                if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
                {
                   // Console.WriteLine("LocalConnectionStatus--采用调制解调器上网。");
                    return 1;
                }
                else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
                {
                   // Console.WriteLine("LocalConnectionStatus--采用网卡上网。");
                    return 2;
                }
            }
            return 0;
        }
    }
}