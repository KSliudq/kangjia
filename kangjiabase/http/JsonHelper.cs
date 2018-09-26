using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;
/// <summary>
/// JSON序列化和反序列化辅助类
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 把对象转换为JSON字符串
    /// </summary>
    /// <param name="o">对象</param>
    /// <returns>JSON字符串</returns>
    public static string objectToJson<T>(T t)
    {
        if (t == null) {
            return "";
        }
        return JsonConvert.SerializeObject(t);
    }
    /// <summary>
    /// 把Json文本转为实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public static T FromJSON<T>(this string input)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
        catch (Exception ex)
        {
            return default(T);
        }
    }
   
}