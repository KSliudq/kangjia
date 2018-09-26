using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kangjiabase
{
    /// <summary>
    /// 小悠日志接口
    /// </summary>
    public class htLog:htBaseModel
    {

        //格式：YYYY-MM-DD HH:mm设计报告ID日志内容日志级别服务名请求ID
        public string logDate;
        //设计报告ID
        public string reportId;
        //日志内容
        public string content;
        //日志级别
        public int logLive;
        //服务名
        public string serviceName;
        //请求ID
        public string timeStamp;
    }
}
