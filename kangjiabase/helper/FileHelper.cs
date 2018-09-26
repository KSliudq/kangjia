using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace kangjiabase
{
    public class FileHelper
    {
        /// <summary>
        /// 取得固件更新文件长度
        /// </summary>
        /// <returns></returns>
        public static int FileSize(string filePath)
        {

            int temp = 0;
            try
            {
                //判断当前路径所指向的是否为文件
                if (File.Exists(filePath) == false)
                {
                    string[] str1 = Directory.GetFileSystemEntries(filePath);
                    foreach (string s1 in str1)
                    {
                        temp += FileSize(s1);
                    }
                }
                else
                {
                    //定义一个FileInfo对象,使之与filePath所指向的文件向关联,
                    //以获取其大小
                    FileInfo fileInfo = new FileInfo(filePath);
                    return Convert.ToInt32(fileInfo.Length);
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
            return temp;
        }

        /// <summary>
        /// 将文件转换成byte[] 数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]</returns>
        public static byte[] GetFileData(string fileUrl)
        {
            FileStream fs = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);

                return buffur;
            }
            catch (Exception ex)
            {
                //MessageBoxHelper.ShowPrompt(ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    //关闭资源
                    fs.Close();
                }
            }
        }


        /// <summary>
        /// 删除文件夹及子文件内文件
        /// </summary>
        /// <param name="str"></param>
        public static void DeleteFolder(string str)
        {
            try
            {
                DirectoryInfo fatherFolder = new DirectoryInfo(str);
                //删除当前文件夹内文件
                FileInfo[] files = fatherFolder.GetFiles();
                foreach (FileInfo file in files)
                {
                    //string fileName = file.FullName.Substring((file.FullName.LastIndexOf("\\") + 1), file.FullName.Length - file.FullName.LastIndexOf("\\") - 1);
                    string fileName = file.Name;
                    try
                    {
                        if (!fileName.Equals("index.dat"))
                        {
                            File.Delete(file.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                //递归删除子文件夹内文件
                foreach (DirectoryInfo childFolder in fatherFolder.GetDirectories())
                {
                    DeleteFolder(childFolder.FullName);
                    Directory.Delete(childFolder.FullName);
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }

        }
        /// <summary>
        /// 取得指定文件夹下的指定文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string getFilename(string path, string type)
        {

            try
            {
                DirectoryInfo d = new DirectoryInfo(path);
                FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    if (fsinfo is DirectoryInfo)     //判断是否为文件夹  
                    {
                        getFilename(fsinfo.FullName, type);//递归调用  
                    }
                    else
                    {
                        String onlyFileName = fsinfo.Extension.ToLower();
                        if (onlyFileName.IndexOf(type.ToLower()) >= 0)
                        {
                            return fsinfo.FullName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogisTrac.WriteLog(ex.Message);
            }
            return "";
        }
    }
}
