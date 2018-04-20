using System;
using System.Web;
using System.IO;
using ITOrm.Core.Helper;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ITOrm.Core.Utility.Files
{
    public class ImageUpload
    {
        /// <summary>
        /// 虚构函数
        /// </summary>
        public ImageUpload() { }

        /**/
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {

            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "W"://指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "CutDefault"://指定高，宽按比例
                    if (originalImage.Height > originalImage.Width)
                    {
                        towidth = originalImage.Width * height / originalImage.Height; 
                    }
                    else if (originalImage.Height == originalImage.Width)
                    {
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                    }
                    else
                    {
                        toheight = originalImage.Height * width / originalImage.Width;
                    }
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以jpg格式保存缩略图

                if (new FileInfo(originalImagePath).Extension.ToLower().IndexOf("png") != -1)
                {
                    bitmap.Save(thumbnailPath, ImageFormat.Png);
                }
                else if (new FileInfo(originalImagePath).Extension.ToLower().IndexOf("gif") != -1)
                {
                    bitmap.Save(thumbnailPath, ImageFormat.Gif);
                }
                else if (new FileInfo(originalImagePath).Extension.ToLower().IndexOf("bmp") != -1)
                {
                    bitmap.Save(thumbnailPath, ImageFormat.Bmp);
                }
                else
                {
                    bitmap.Save(thumbnailPath, ImageFormat.Jpeg);
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        #region 暂时没有用到的代码

        /**/
        /// <summary>
        /// 在图片上增加文字水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_sy">生成的带文字水印的图片路径</param>
        protected void AddWater(string Path, string Path_sy)
        {
            string addText = ConfigHelper.GetAppSettings("Domain");
            Image image = Image.FromFile(Path);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(image, 0, 0, image.Width, image.Height);
            Font f = new Font("Verdana", 60);
            Brush b = new SolidBrush(Color.Green);

            g.DrawString(addText, f, b, 35, 35);
            g.Dispose();

            image.Save(Path_sy);
            image.Dispose();
        }

        /**/
        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="Path">原服务器图片路径</param>
        /// <param name="Path_syp">生成的带图片水印的图片路径</param>
        /// <param name="Path_sypf">水印图片路径</param>
        protected void AddWaterPic(string Path, string Path_syp, string Path_sypf)
        {
            Image image = Image.FromFile(Path);
            Image copyImage = Image.FromFile(Path_sypf);
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(copyImage, new Rectangle(image.Width - copyImage.Width, image.Height - copyImage.Height, copyImage.Width, copyImage.Height), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
            g.Dispose();
            image.Save(Path_syp);
            image.Dispose();
        }

        /// <summary>
        /// 删除临时文件夹的文件
        /// </summary>
        /// <param name="pic"></param>
        public void delPic(string pic)
        {
            System.IO.File.Delete(HttpContext.Current.Server.MapPath("/Shopkeeping/ImgUpload/" + pic));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="pic"></param>
        public void delTruePic(string pic, string path)
        {
            //2008/01/200821212.jpg 
            //File.Delete(Server.MapPath("/Shopkeeping/UploadedFiles/" + path +"/"+ pic));


        }
        /// <summary>
        /// 把文件从临时文件夹移动到指定文件夹
        /// </summary>
        /// <param name="pic"></param>
        public void MovePic(string pic, string path)
        {

            // if (!System.IO.Directory.Exists(newPath))
            //    System.IO.Directory.CreateDirectory(newPath);
            // File.Move(oldPath, newPaths);




        }
        #endregion

        #region private
        private static string[] DateToFileName()
        {
            DateTime dateNow = DateTime.Now;
            string[] files = new string[2];
            files[0] = string.Format("{0}/{1}/{2}/", dateNow.Year, dateNow.Month, dateNow.Day);
            files[1] = string.Format("{0}{1}{2}{3}{4}{5}{6}", dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute, dateNow.Second, dateNow.Millisecond);
            return files;
        }
        #endregion

    }



}
