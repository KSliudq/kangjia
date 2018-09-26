using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace kangjiabase
{
    /// <summary>
    /// 图片压缩
    /// </summary>
    public class ImgThumbnail
    {
        /// <summary>
        /// 指定缩放类型
        /// </summary>
        public enum ImgThumbnailType
        {
            /// <summary>
            /// 指定高宽缩放（可能变形）
            /// </summary>
            WH = 0,
            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            W = 1,
            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            H = 2,
            /// <summary>
            /// 指定高宽裁减（不变形）
            /// </summary>
            Cut = 3
        }
        #region Thumbnail
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="height">高度</param>
        /// <param name="width"></param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="type">压缩缩放类型</param>
        /// <returns></returns>
        public Bitmap Thumbnail(Bitmap iSource, int height, int width, int flag)
        {
            ImgThumbnailType type = ImgThumbnail.ImgThumbnailType.H;
            //System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;

            //缩放后的宽度和高度
            int towidth = width;
            int toheight = height;
            //
            int x = 0;
            int y = 0;
            int ow = iSource.Width;
            int oh = iSource.Height;

            switch (type)
            {
                case ImgThumbnailType.WH://指定高宽缩放（可能变形）           
                    {
                        break;
                    }
                case ImgThumbnailType.W://指定宽，高按比例     
                    {
                        toheight = iSource.Height * width / iSource.Width;
                        break;
                    }
                case ImgThumbnailType.H://指定高，宽按比例
                    {
                        towidth = iSource.Width * height / iSource.Height;
                        break;
                    }
                case ImgThumbnailType.Cut://指定高宽裁减（不变形）     
                    {
                        if ((double)iSource.Width / (double)iSource.Height > (double)towidth / (double)toheight)
                        {
                            oh = iSource.Height;
                            ow = iSource.Height * towidth / toheight;
                            y = 0;
                            x = (iSource.Width - ow) / 2;
                        }
                        else
                        {
                            ow = iSource.Width;
                            oh = iSource.Width * height / towidth;
                            x = 0;
                            y = (iSource.Height - oh) / 2;
                        }
                        break;
                    }
                default:
                    break;
            }

            Bitmap ob = new Bitmap(towidth, toheight);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(System.Drawing.Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource
              , new Rectangle(x, y, towidth, toheight)
              , new Rectangle(0, 0, iSource.Width, iSource.Height)
              , GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int i = 0; i < arrayICI.Length; i++)
                {
                    if (arrayICI[i].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[i];
                        break;
                    }
                }
                //if (jpegICIinfo != null)
                //{
                //    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                //}
                //else
                //{
                //    ob.Save(dFile, tFormat);
                //}
                return ob;
            }
            catch
            {
                return null;
            }
            finally
            {
                iSource.Dispose();
                iSource = null;
                //ob.Dispose();

            }
        }
        #endregion
    }
}
