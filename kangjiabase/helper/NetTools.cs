using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Management;
using System.IO;
using System.Net;
namespace kangjiabase
{
    public class NetTools
    {
        public static string GetMAC2()
        {
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        string mac = mo["MacAddress"].ToString().Replace(":", "-");
                        return mac;
                    }
                }
            }

            return "00-00-00-00-00-00";

        }
        public static string GetMAC()
        {
            ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select * FROM Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection objMOC = objMOS.Get();
            string macAddress = String.Empty;
            foreach (ManagementObject objMO in objMOC)
            {
                object tempMacAddrObj = objMO["MacAddress"];

                if (tempMacAddrObj == null) //Skip objects without a MACAddress
                {
                    continue;
                }
                //macAddress = tempMacAddrObj.ToString();
                if (macAddress == String.Empty) // only return MAC Address from first card that has a MAC Address
                {
                    macAddress = tempMacAddrObj.ToString();
                    LogisTrac.WriteLog("mac地址----" + macAddress);
                }
                objMO.Dispose();
            }
            LogisTrac.WriteLog("mac地址----" + macAddress);
            macAddress = macAddress.Replace(":", "-");
            return macAddress;

           // return "00-00-00-00-00-00";
        }
        

    }

}
