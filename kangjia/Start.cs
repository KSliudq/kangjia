using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using kangjiabase;
namespace kangjia
{
    internal static class Start
    {
        private const int WS_SHOWNORMAL = 1;
        [STAThread]
        private static void Main()
        {
            try
            {
                // 注册全局线程异常处理事件侦听
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Process instance = Start.RunningInstance();
                if (instance == null)
                {
                    yoyoConst.Initproject(true);
                    LogisTrac.WriteLog("程序启动");

                    string mainversion = OperateIniFile.ReadIniData_main("MAIN_VERSION");
                    
                    string res = OperateIniFile.ReadVersionIniData("RES_VERSION");

                    LogisTrac.WriteLog("kangjia版本V" + mainversion);

                    LogisTrac.WriteLog("res版本V" + res);

                    LogisTrac.WriteLog("StartupPath" + Application.StartupPath);
                    LogisTrac.WriteLog("GetCurrentDirectory" + System.IO.Directory.GetCurrentDirectory());
                    //  Application.Run(new indexform());
                    Application.Run(new indexForm2());//indexForm2
                    KillProcess("kangjiamain");
                    return;
                }
                Start.HandleRunningInstance(instance);
            }
            catch (Exception ex)
            {
                Application.Run(new indexForm2());
                LogisTrac.WriteLog(ex.Message);
            }
        }
        public static Process RunningInstance()
        {
            try
            {
                Process current = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(current.ProcessName);
                Process[] array = processes;
                for (int i = 0; i < array.Length; i++)
                {
                    Process process = array[i];
                    if (process.Id != current.Id && Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        process.Kill();
                        break;
                        //return process;
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// 关闭进程
        /// </summary>
        /// <param name="processName">进程名</param>
        private static void KillProcess(string processName)
        {
            try
            {
                Process[] myproc = Process.GetProcesses();
                foreach (Process item in myproc)
                {
                    if (item.ProcessName == processName)
                    {
                        LogisTrac.WriteLog("删除" + processName);
                        item.Kill();
                        item.Dispose();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
        }
        private static void HandleRunningInstance(Process instance)
        {

            //MessageBox.Show("该程序已经在运行！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            Start.ShowWindowAsync(instance.MainWindowHandle, 1);
            Start.SetForegroundWindow(instance.MainWindowHandle);
        }
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #region 全局线程异常处理事件
        /// <summary> 
        /// 全局线程异常处理事件 
        /// </summary> 
        /// <param name="sender"> </param> 
        /// <param name="e"> </param> 
        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

            LogisTrac.WriteLog("系统捕获异常---" + e.Exception);
            if (e.Exception.Message.IndexOf("内存") >= 0)
            {
                update_start();
            }
            //  Functions.ShowMessageBox("系统异常，请联系管理员！ ", Const.EVENT_INFO);
        }
        public static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            LogisTrac.WriteLog("系统捕获异常,未知异常---" + e.ExceptionObject.ToString());
            if (e.ExceptionObject.ToString().IndexOf("内存") >= 0)
            {
                update_start();
            }
            // Functions.ShowMessageBox("系统异常，请联系管理员！ ", Const.EVENT_INFO);
        }
        private static void update_start()
        {
            try
            {
                // 升级完成，启动主程序
                string StrExe = yoyoConst.KANGJIA_MAIN_PATH + "kangjiamain.exe";
                if (System.IO.File.Exists(StrExe))
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
        #endregion
    }
}
