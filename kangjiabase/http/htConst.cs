using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Xml;
using kangjiabase;
namespace kangjiabase
{
    //测试地址
    //http://123.57.27.38:9090/swagger-ui.html#/
    public class htConst
    {
        //设备状态
        public enum YoyoEqu
        {
            lock_state = 1,//1：锁屏状态
            testing_state = 2,//2：(测试中解锁)
            start_state = 3,//3:开机 
            end_state = 4,//4：关机
            other_state = 5,//5：其他
            cant_state = 6,//6：设备正在使用中
            unlock_error = 21//6：解锁失败
        }
        //小悠报告数据接口IFI_001	
        public static string url_htreport = yoyoConst.SERVER_IP + "/receive/receiveReport";
        /// <summary>
        /// 当前设备状态上报接口IFI_02
        /// </summary>
        public static string url_IFI_02 = yoyoConst.SERVER_IP + "/IFI/V1/IFI_02";
        /// <summary>
        /// 设备锁屏二维码请求接口IFI_03
        /// </summary>

        public static string url_IFI_03 = yoyoConst.SERVER_IP + "/IFI/V1/IFI_03";
        /// <summary>
        /// 根据设备SN，productKey，deviceType信息请求，返回永久二维码地址，长连接相关信息。
        /// </summary>

        public static string url_IFI_2_03 = yoyoConst.QR_URL;//yoyoConst.SERVER_IP + "/IFI/V2/IFI_03";
        /// <summary>
        /// 网络时间同步接口IFI_05
        /// </summary>

        public static string url_IFI_05 = yoyoConst.SERVER_IP + "/IFI/V1/IFI_05";

        /// <summary>
        /// v2版接口增加了token校验，调用接口时需要带上token.
        /// </summary>

        public static string url_token = yoyoConst.SERVER_IP + "/api/V2/token?appId=7592007052483&appKey=hgox9fe3xedrm0qz";

        /// <summary>
        /// 设备注册激活
        /// </summary>
        public static string url_REGISTER = yoyoConst.SERVER_IP + "/base/V1/register/R_01";
        /// <summary>
        /// 设备升级
        /// </summary>
        public static string url_V1_UPLOAD = yoyoConst.SERVER_IP + "/base/V1/upload/U_01";

        /// <summary>
        /// 验证token是否过期
        /// </summary>
        /// <returns></returns>
        public static bool checkToken()
        {

            if (yoyoConst.TOKEN_EXP_TIME != null)
            {
                if (yoyoConst.TOKEN_EXP_TIME >= DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断token是否超时，否则取得最新token
        /// </summary>
        public static string getToken()
        {
            try
            {
                if (checkToken())
                {
                    return yoyoConst.TOKEN;
                }
                //设备锁屏二维码请求接口
                string parurl = htConst.url_token;
                string ret = htConst.Post(parurl, "");

                retIToken model = JsonHelper.FromJSON<retIToken>(ret);
                if (model.success)
                {
                    yoyoConst.TOKEN = model.data.token;
                    yoyoConst.TOKEN_EXP_TIME = DateTime.Now.AddSeconds(model.data.expire);
                    return yoyoConst.TOKEN;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex) {
                LogisTrac.WriteLog(ex.Message);
                return null;
            }
        }
        //发送请求，并取得返回结果
        public static string Post<T>(string Url, T t)
        {
            
            try
            {
                string jsonParas = "";
                if (t != null && t.ToString() != "")
                {
                    //将对象转换为json
                    jsonParas = JsonHelper.objectToJson(t);
                   // yoyoConst.WriteLog(jsonParas);
                }
                return PostJson(Url, jsonParas);
            }
            catch (Exception ex){
                yoyoConst.WriteLog(ex.ToString());
                return "";
            }
        }

        //发送请求，并取得返回结果
        public static string PostJson(string Url, string jsonParas)
        {
            try
            {
                string strURL = Url;

                //创建一个HTTP请求  
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
                //Post请求方式  
                request.Method = "POST";
                //内容类型
                request.ContentType = "application/json";

                //设置参数，并进行URL编码 

                string paraUrlCoded = jsonParas;//System.Web.HttpUtility.UrlEncode(jsonParas);   

                byte[] payload;
                //将Json字符串转化为字节  
                payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
                //设置请求的ContentLength   
                request.ContentLength = payload.Length;
                //发送请求，获得请求流 

                Stream writer;
                try
                {
                    writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
                }
                catch (Exception ex)
                {
                    writer = null;
                    yoyoConst.WriteLog(ex.ToString());
                }
                //将请求参数写入流
                writer.Write(payload, 0, payload.Length);
                writer.Close();//关闭请求流

                //String strValue = "";//strValue为http响应所返回的字符流
                HttpWebResponse response;
                try
                {
                    //获得响应流
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }
                string html = "";
                try
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(stream, Encoding.UTF8);
                    html = sr.ReadToEnd();
                    sr.Close();
                    stream.Close();
                }
                catch (Exception ex)
                {
                    yoyoConst.WriteLog(ex.ToString());
                }
                //Stream s = response.GetResponseStream();


                ////服务器端返回的是一个XML格式的字符串，XML的Content才是我们所需要的Json数据
                //XmlTextReader Reader = new XmlTextReader(s);
                //Reader.MoveToContent();
                //strValue = Reader.ReadInnerXml();//取出Content中的Json数据
                //Reader.Close();
                //s.Close();



                return html;//返回Json数据
            }catch{
                return "";
            }
        }
        /// <summary>
        /// 共通参数
        /// </summary>
        /// <returns></returns>
        public static string getUrlPara() {
            string time = DateTime.Now.ToString("yyyyMMddHHssmm");
            if (!htConst.checkEQUSN("生成url")) {
                return null;
            }
            string url = "?deviceSN=" + getEQUSN() + "&timeStamp=" + time;
            
            return url;
        }

        public static bool checkEQUSN(string stepname) {
            if (!string.IsNullOrEmpty(yoyoConst.EQU_SN))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(yoyoConst.EQU_SN_LOG))
            {
                return true;
            }
            LogisTrac.WriteLog("没有获得SN无法" + stepname);
            return false;
        }
        public static string getEQUSN() {
            if (!string.IsNullOrEmpty(yoyoConst.EQU_SN))
            {
                return yoyoConst.EQU_SN;
            }
            else if (!string.IsNullOrEmpty(yoyoConst.EQU_SN_LOG))
            {
                return yoyoConst.EQU_SN_LOG;
            }
            LogisTrac.WriteLog("没有获得SN无法生成url地址");
            return null;
        }
    }
}
