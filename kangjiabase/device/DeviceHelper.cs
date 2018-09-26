using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kangjiabase
{
   public class DeviceHelper
    {

       public static byte[] Com_getSubData(byte[] CommandData)
       {
           byte[] ret = new byte[CommandData.Length - 7];

           for (int i = 4; i < CommandData.Length - 3; i++)
           {
               ret[i - 4] = CommandData[i];
           }
           return ret;

       }

       public static bool Com_checkSN()
       {
          // LogisTrac.WriteLog(yoyoConst.EQU_SN);
           if (string.IsNullOrEmpty(yoyoConst.EQU_SN))
           {
               return false;
           }
           System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
           if (!reg.IsMatch(yoyoConst.EQU_SN))
           {
               return false;
           }
           //if (yoyoConst.EQU_SN.Length < 15)
           //{
           //    return false;
           //}
           return true;
       }
    }
}
