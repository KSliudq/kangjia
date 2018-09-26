using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace kangjiabase
{
    //面部识别处理程序
    public class AFDFunction{
        public static bool IS32 = true;
        public struct AFD_FSDK_FACERES
        {
            public int nFace;
            public IntPtr rcFace;
            public IntPtr lfaceOrient;
        }

        public struct MRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        //定义FD的版本号
        public struct AFD_FSDK_Version
        {
            public int lCodebase;
            public int lMajor;
            public int lMinor;
            public int lBuild;
            public IntPtr Version;
            public IntPtr BuildDate;
            public IntPtr CopyRight;
        }

        //定义人脸检查结果中人脸的角度
        public enum AFD_FSDK_OrientCode
        {
            AFD_FSDK_FOC_0 = 1,
            AFD_FSDK_FOC_90 = 2,
            AFD_FSDK_FOC_270 = 3,
            AFD_FSDK_FOC_180 = 4,
            AFD_FSDK_FOC_30 = 5,
            AFD_FSDK_FOC_60 = 6,
            AFD_FSDK_FOC_120 = 7,
            AFD_FSDK_FOC_150 = 8,
            AFD_FSDK_FOC_210 = 9,
            AFD_FSDK_FOC_240 = 10,
            AFD_FSDK_FOC_300 = 11,
            AFD_FSDK_FOC_330 = 12
        }

        public enum AFD_FSDK_OrientPriority
        {
            AFD_FSDK_OPF_0_ONLY = 1,
            AFD_FSDK_OPF_90_ONLY = 2,
            AFD_FSDK_OPF_270_ONLY = 3,
            AFD_FSDK_OPF_180_ONLY = 4,
            AFD_FSDK_OPF_0_HIGHER_EXT = 5
        }

        public struct ASVLOFFSCREEN
        {
            public int u32PixelArrayFormat;

            public int i32Width;

            public int i32Height;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.SysUInt)]
            public System.IntPtr[] ppu8Plane;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I4)]
            public int[] pi32Pitch;
        }

        static IntPtr detectEngine = IntPtr.Zero;

        public static string Detect3264()
        {
             ConnectionOptions oConn = new ConnectionOptions();
             System.Management.ManagementScope oMs = new System.Management.ManagementScope("\\\\localhost", oConn);
             System.Management.ObjectQuery oQuery = new System.Management.ObjectQuery("select AddressWidth from Win32_Processor");
             ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery);
             ManagementObjectCollection oReturnCollection = oSearcher.Get();
            string addressWidth = null;

            foreach (ManagementObject oReturn in oReturnCollection)
             {
                 addressWidth = oReturn["AddressWidth"].ToString();
             }

            return addressWidth;
        }

        public static Boolean initEng()
        {
            string temp = Detect3264();
            if (temp != "32")
            {
                IS32 = false;
            }
            else {
                IS32 = true;
            }

            int detectSize = 40 * 1024 * 1024;
            int nScale = yoyoConst.FACE_LENGTH;//默认值是4
            if (nScale >= 7 || nScale < 4) {
                nScale = 4;
            }
            int nMaxFaceNum = 1;
            string appId;
            string sdkFDKey;
            if (!IS32)
            {
                appId = "9rRYbWqYR529HRszk2GjwLmcxr1CnRoTX3umKw8Gfx3x";
                sdkFDKey = "J4EqeYzPySFUb6JEwXfKZQxEx8FThRmCpuXamHBengwE";
            }
            else {
                appId = "9rRYbWqYR529HRszk2GjwLmk8FGNjLtSqvNh2EgPoxD9";
                sdkFDKey = "131rsV3VbU1ycoQZ2vt1hokVJcQiK1Vxq7p7WwxLVoNZ";
            }
            
            IntPtr pMem = Marshal.AllocHGlobal(detectSize);

            int retCode;
            if (!IS32)
            {
                retCode = AFD_FSDK_InitialFaceEngine(appId, sdkFDKey, pMem, detectSize, ref detectEngine, (int)AFD_FSDK_OrientPriority.AFD_FSDK_OPF_0_HIGHER_EXT, nScale, nMaxFaceNum);
            }
            else {
                retCode = AFD_FSDK_InitialFaceEngine32(appId, sdkFDKey, pMem, detectSize, ref detectEngine, (int)AFD_FSDK_OrientPriority.AFD_FSDK_OPF_0_HIGHER_EXT, nScale, nMaxFaceNum);
            }
            if (retCode != 0)
            {
                //MessageBox.Show("引擎初始化失败:错误码为:" + retCode);
                return false;
            }
            return true;
        }

        public static Boolean unInitEng()
        {

            int retCode;
            if (!IS32)
            {
                retCode = AFD_FSDK_UninitialFaceEngine(detectEngine);
            }
            else
            {
                retCode = AFD_FSDK_UninitialFaceEngine32(detectEngine);
            }
            if (retCode != 0)
            {
                //MessageBox.Show("引擎注销失败:错误码为:" + retCode);
                return false;
            }
            return true;
        }

        [DllImport("libarcsoft_fsdk_face_detection.dll", EntryPoint = "AFD_FSDK_InitialFaceEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_InitialFaceEngine(string appId, string sdkKey, IntPtr pMem, int lMemSize, ref IntPtr pEngine, int iOrientPriority, int nScale, int nMaxFaceNum);

        [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_StillImageFaceDetection(IntPtr pEngine, IntPtr pImgData, ref IntPtr pFaceRes);

        [DllImport("libarcsoft_fsdk_face_detection.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_UninitialFaceEngine(IntPtr pEngine);

        [DllImport("libarcsoft_fsdk_face_detection32.dll", EntryPoint = "AFD_FSDK_InitialFaceEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_InitialFaceEngine32(string appId, string sdkKey, IntPtr pMem, int lMemSize, ref IntPtr pEngine, int iOrientPriority, int nScale, int nMaxFaceNum);

        [DllImport("libarcsoft_fsdk_face_detection32.dll", EntryPoint = "AFD_FSDK_StillImageFaceDetection", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_StillImageFaceDetection32(IntPtr pEngine, IntPtr pImgData, ref IntPtr pFaceRes);

        [DllImport("libarcsoft_fsdk_face_detection32.dll", EntryPoint = "AFD_FSDK_UninitialFaceEngine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AFD_FSDK_UninitialFaceEngine32(IntPtr pEngine);

        private static byte[] readBmp(Bitmap image, ref int width, ref int height, ref int pitch)
        {//将Bitmap锁定到系统内存中,获得BitmapData
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            IntPtr ptr = data.Scan0;
            //定义数组长度
            int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
            byte[] sourceBitArray = new byte[soureBitArrayLength];
            //将bitmap中的内容拷贝到ptr_bgr数组中
            Marshal.Copy(ptr, sourceBitArray, 0, soureBitArrayLength);
            width = data.Width;
            height = data.Height;
            pitch = Math.Abs(data.Stride);


            int line = width * 3;
            int bgr_len = line * height;
            byte[] destBitArray = new byte[bgr_len];
            for (int i = 0; i < height; ++i)
            {
                Array.Copy(sourceBitArray, i * pitch, destBitArray, i * line, line);
            }
            pitch = line;
            image.UnlockBits(data);
            return destBitArray;
        }
        public static Boolean checkAndMarkFace(Bitmap image)
        {
           // Bitmap image = a;
            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            //位图中第一个像素数据的地址。它也可以看成是位图中的第一个扫描行
            IntPtr ptr = data.Scan0;
            //定义数组长度
            int soureBitArrayLength = data.Height * Math.Abs(data.Stride);
            byte[] sourceBitArray = new byte[soureBitArrayLength];
            //将bitmap中的内容拷贝到ptr_bgr数组中
            Marshal.Copy(ptr, sourceBitArray, 0, soureBitArrayLength);
            int width = data.Width;
            int height = data.Height;
            int pitch = Math.Abs(data.Stride);
            image.UnlockBits(data);
            data = null;

            int line = width * 3;
            int bgr_len = line * height;
            byte[] destBitArray = new byte[bgr_len];
            for (int i = 0; i < height; ++i)
            {
                Array.Copy(sourceBitArray, i * pitch, destBitArray, i * line, line);
            }
            pitch = line;
           // image.UnlockBits(data);

            byte[] imageData = destBitArray;//readBmp(image, ref width, ref height, ref pitch);
            IntPtr imageDataPtr = Marshal.AllocHGlobal(imageData.Length);
            Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);
            ASVLOFFSCREEN offInput = new ASVLOFFSCREEN();
            offInput.u32PixelArrayFormat = 513;
            offInput.ppu8Plane = new IntPtr[4];
            offInput.ppu8Plane[0] = imageDataPtr;
            offInput.i32Width = width;
            offInput.i32Height = height;
            offInput.pi32Pitch = new int[4];
            offInput.pi32Pitch[0] = pitch;
            IntPtr offInputPtr = Marshal.AllocHGlobal(Marshal.SizeOf(offInput));
            Marshal.StructureToPtr(offInput, offInputPtr, false);
            AFD_FSDK_FACERES faceRes = new AFD_FSDK_FACERES();
            IntPtr faceResPtr = Marshal.AllocHGlobal(Marshal.SizeOf(faceRes));

            int detectResult;
            if (!IS32)
            {
                detectResult = AFDFunction.AFD_FSDK_StillImageFaceDetection(detectEngine, offInputPtr, ref faceResPtr);
            }
            else {
                detectResult = AFDFunction.AFD_FSDK_StillImageFaceDetection32(detectEngine, offInputPtr, ref faceResPtr);
            }
            if (detectResult!=0)
            {
                //return false;
            }
            faceRes = (AFD_FSDK_FACERES)Marshal.PtrToStructure(faceResPtr, typeof(AFD_FSDK_FACERES));
            bool flag = (faceRes.nFace > 0 && faceRes.nFace<10);
            Marshal.FreeHGlobal(imageDataPtr);
            Marshal.FreeHGlobal(offInputPtr);

            
            return (flag);           
        }
    }
}
