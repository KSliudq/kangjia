using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using kangjiabase;
namespace kangjiamain
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
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                KillProcess("kangjia");
                Process instance = Start.RunningInstance();
                if (instance == null)
                {
                    yoyoConst.Initproject(false);
                    
                    LogisTrac.WriteLog("更新启动");
                    string mainversion = OperateIniFile.ReadIniData("MAIN_VERSION");
                    if (string.IsNullOrEmpty(mainversion))
                    {
                        LogisTrac.WriteLog("kangjiamain版本V1.1");
                    }
                    else {
                        LogisTrac.WriteLog("kangjiamain版本V" + mainversion);
                    }
                    Application.Run(new upindexForm2());//indexForm2
                 
                    return;
                }
                Start.HandleRunningInstance(instance);
            }
            catch(Exception ex) {
                Application.Run(new upindexForm2());
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
                    }
                }
            }
            catch { }
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
            catch { }
        }
        private static void HandleRunningInstance(Process instance)
        {
            MessageBox.Show("该程序已经在运行！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

            // 显示全局异常提示 
            //ServerCommon.ServerFunctions serverFunction = new ServerCommon.ServerFunctions();
            //serverFunction.WriteLog(e.Exception.Message);


            LogisTrac.WriteLog(e.Exception);
            //  Functions.ShowMessageBox("系统异常，请联系管理员！ ", Const.EVENT_INFO);
        }
        public static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            LogisTrac.WriteLog(e.ExceptionObject.ToString());
            // Functions.ShowMessageBox("系统异常，请联系管理员！ ", Const.EVENT_INFO);
        }

        #endregion
    }
}
