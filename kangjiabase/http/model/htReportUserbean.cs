using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kangjiabase
{
    public class htReportUserbean
    {
        //性别。0：女；1：男
        public string sex;
        //刷卡机必填 卡号、用户主键（20171102update）
        public string userNo;
        //实时头像地址（20171102add）
        public string FaceImageUrl;
        //身高
        public string height;
        //身高单位。例：cm
        public string heightUnit;
        //年龄
        public int age;
        //美丑打分 范围0-100 越大表示越美。face_fields包含beauty时返回
        public string beauty;
        //表情 0 不笑；1 微笑；2 大笑。face_fields包含expression时返回
        public string expression;
        //体重
        public string weight;
        //体重单位。例：kg
        public string weightUnit;
        //心率
        public string heartRate;

        public string glasses;


        public string faceId;
    }
}
