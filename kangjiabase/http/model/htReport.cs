using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace kangjiabase
{
    public class htReport
    {
        //报告ID
        public string reportId;
        //产生报告的小悠的Mac地址
        public string macId;
        //时间格式
        public string reportDate;
        //测量值以K-V形式存储
        public Hashtable targetList;
        //类型，
        public string type;
        //设备SN（20171102add）
        public string deviceSN;
        //请求时间戳（2017/11/9）
        public string timeStamp;
        //用户基本情况
        public htReportUserbean userBean;
    }
}
