using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace kangjiabase
{
    public class OperateIniFile
    {
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

      //  private static string iniFilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\kangjia.ini";//获取INI文件路径
        
        #endregion

        #region 读Ini文件

        public static string ReadIniData(string Key)
        {
            try
            {
                string iniFilePath = yoyoConst.KANGJIA_PATH + "\\kangjia.ini";
                string Section = Path.GetFileNameWithoutExtension(iniFilePath);

                if (File.Exists(iniFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024);
                    GetPrivateProfileString(Section, Key, "", temp, 1024, iniFilePath);
                    return temp.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
            catch {
                return String.Empty;
            }
        }

        public static string ReadIniData_main(string Key)
        {
            try
            {
                string iniFilePath = yoyoConst.KANGJIA_MAIN_PATH + "\\kangjiamain.ini";
                string Section = Path.GetFileNameWithoutExtension(iniFilePath);

                if (File.Exists(iniFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024);
                    GetPrivateProfileString(Section, Key, "", temp, 1024, iniFilePath);
                    return temp.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
            catch
            {
                return String.Empty;
            }
        }


        public static bool WriteIniData(string Key, string Value)
        {
            try
            {
                string versionFilePath = yoyoConst.KANGJIA_PATH + "kangjia.ini";
                if (File.Exists(versionFilePath))
                {
                    string Section = Path.GetFileNameWithoutExtension(versionFilePath);

                    long OpStation = WritePrivateProfileString(Section, Key, Value, versionFilePath);
                    if (OpStation == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 写kangjia_version Ini文件
   
       
        public static string ReadVersionIniData(string Key)
        {
            try
            {
                string versionFilePath = yoyoConst.KANGJIA_MAIN_PATH + "kangjiamain.ini";

                string Section = Path.GetFileNameWithoutExtension(versionFilePath);

                if (File.Exists(versionFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024);
                    GetPrivateProfileString(Section, Key, "", temp, 1024, versionFilePath);
                    return temp.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
            catch {
                return String.Empty;
            }
        }

        public static bool WriteVersionIniData(string Key, string Value)
        {
            try
            {
                string versionFilePath = yoyoConst.KANGJIA_MAIN_PATH + "kangjiamain.ini";
                if (File.Exists(versionFilePath))
                {
                    string Section = Path.GetFileNameWithoutExtension(versionFilePath);

                    long OpStation = WritePrivateProfileString(Section, Key, Value, versionFilePath);
                    if (OpStation == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch {
                return false;
            }
        }

        #endregion

        #region 读SNIni文件

        public static string ReadSNIniData(string Key)
        {
            try
            {
                string iniFilePath = yoyoConst.KANGJIA_PATH + "\\SN.ini";
                string Section = Path.GetFileNameWithoutExtension(iniFilePath);

                if (File.Exists(iniFilePath))
                {
                    StringBuilder temp = new StringBuilder(1024);
                    GetPrivateProfileString(Section, Key, "", temp, 1024, iniFilePath);
                    return temp.ToString();
                }
                else
                {
                    return String.Empty;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
        public static bool WriteSNIniData(string Key, string Value)
        {
            try
            {
                string versionFilePath = yoyoConst.KANGJIA_PATH + "SN.ini";
               
                if (File.Exists(versionFilePath))
                {
                    string Section = Path.GetFileNameWithoutExtension(versionFilePath);

                    long OpStation = WritePrivateProfileString(Section, Key, Value, versionFilePath);
                    if (OpStation == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
