using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace ITOrm.Core.Utility.Files
{
    public class FileHelper
    {
        /// <summary>
        /// 封装了文件常用操作方法
        /// </summary>
        public FileHelper()
        {
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <returns>是否存在</returns>
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <param name="overwrite">当目标文件存在时是否覆盖</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName, bool overwrite)
        {
            if (!System.IO.File.Exists(sourceFileName))
            {
                throw new FileNotFoundException(sourceFileName + "文件不存在！");
            }
            if (!overwrite && System.IO.File.Exists(destFileName))
            {
                return false;
            }
            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 备份文件,当目标文件存在时覆盖
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作是否成功</returns>
        public static bool BackupFile(string sourceFileName, string destFileName)
        {
            return BackupFile(sourceFileName, destFileName, true);
        }

        /// <summary>
        /// 获得指定目录下的文件列表
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static string[] GetDirectoryFileList(string path, string searchPattern)
        {
            if (!Directory.Exists(path))
                return new string[0];

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = dirInfo.GetFiles(searchPattern);
            string[] result = new string[fileInfos.Length];
            for (int i = 0; i < fileInfos.Length; i++)
            {
                result[i] = fileInfos[i].Name;
            }

            return result;
        }

        /// <summary>
        /// 获取指定目录的所有文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetDirectoryFileList(string path)
        {
            return GetDirectoryFileList(path, "*.*");
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static string GetFileContent(string filename)
        {
            string result = string.Empty;

            if (!File.Exists(filename))
                return string.Format("文件 {0} 不存在", filename);
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        ///   根据指定宽度，高度，裁剪
        /// </summary>
        /// <param name="fromFileStream">图片流</param>
        /// <param name="fileSaveUrl">保存地址</param>
        /// <param name="templateWidth">指定要裁剪后的宽度</param>
        /// <param name="templateHeight">指定要裁剪后的高度</param>
        /// <param name="mode">裁剪方式</param>
        public static void MakeSmallImg(System.IO.Stream fromFileStream, string fileSaveUrl, System.Double templateWidth, System.Double templateHeight, string mode)
        {
            //从文件取得图片对象，并使用流中嵌入的颜色管理信息  
            Image myImage = Image.FromStream(fromFileStream, true);
            int x = 0;
            int y = 0;
            int newWidth = Convert.ToInt32(templateWidth);
            int newHeight = Convert.ToInt32(templateHeight);
            int ow = myImage.Width;
            int oh = myImage.Height;

            switch (mode)
            {
                //仿QQ空间切图，永不超出界定
                case "WH":
                    //如果上传图片的宽度[小于]指定宽度
                    if (myImage.Width < Convert.ToInt32(templateWidth))
                    {
                        //上传图片高度[大于]指定高度
                        if (myImage.Height > Convert.ToInt32(templateHeight))
                        {
                            //指定高，宽按比例
                            newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                        }
                        else//上传的图片的高度[小于]指定高度
                        {
                            //什么都有不做，但是，原样存一个副本到缩略图路径
                            //保存缩略图  
                        }
                    }
                    else//上传图片的宽度[大于]指定宽度
                    {
                        //上传图片高度[大于]指定高度
                        if (myImage.Height > Convert.ToInt32(templateHeight))//上传图片的宽度和高度同时[大于]指定宽高
                        {
                            if (myImage.Width > myImage.Height)//上传图片宽[大于]高
                            {
                                double whBL = Convert.ToInt32(templateWidth) / Convert.ToInt32(templateHeight);//指定图片的宽高比例
                                double scwhBL = myImage.Width / myImage.Height;//上传图片的宽高比例

                                //指定高，宽按比例
                                newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                                //指定宽，高按比例              
                                newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;

                                if (whBL > scwhBL)//推测，同比例缩放后 ：上传的图片的高度[小于]指定高度
                                {
                                    newHeight = Convert.ToInt32(templateHeight);
                                }
                                else
                                {
                                    newWidth = Convert.ToInt32(templateWidth);
                                }

                            }
                            else//上传图片宽[小于]高
                            {
                                double hwBL = Convert.ToInt32(templateHeight) / Convert.ToInt32(templateWidth);//指定图片的高宽比例
                                double schwBL = myImage.Width / myImage.Height;//上传图片的高宽比例

                                //指定高，宽按比例
                                newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                                //指定宽，高按比例              
                                newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;

                                if (hwBL < schwBL)//推测，同比例缩放后 ：上传的图片的高度[小于]指定高度
                                {
                                    newHeight = Convert.ToInt32(templateHeight);
                                }
                                else
                                {
                                    newWidth = Convert.ToInt32(templateWidth);
                                }
                            }
                        }
                        else//上传的图片的高度[小于]指定高度
                        {
                            //指定宽，高按比例                    
                            newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;
                        }
                    }
                    break;
                case "HW"://指定高宽缩放（可能变形）                
                    break;
                case "None"://保持原始大小 
                    newHeight = oh;
                    newWidth = ow;
                    break;
                case "W"://指定宽，高按比例                    
                    newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;
                    break;
                case "H"://指定高，宽按比例
                    newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                
                    if ((double)myImage.Width / (double)myImage.Height > (double)newWidth / (double)newHeight)
                    {
                        oh = myImage.Height;
                        ow = myImage.Height * newWidth / newHeight;
                        y = 0;
                        x = (myImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = myImage.Width;
                        oh = myImage.Width * Convert.ToInt32(templateHeight) / newWidth;
                        x = 0;
                        y = (myImage.Height - oh) / 2;
                    }
                    break;

                default:
                    break;

            }
            //取得图片大小  
            System.Drawing.Size mySize = new Size((int)newWidth, (int)newHeight);
            //新建一个bmp图片  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(myImage, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
            System.Drawing.GraphicsUnit.Pixel);

            //保存缩略图  
            if (File.Exists(fileSaveUrl))
            {
                File.SetAttributes(fileSaveUrl, FileAttributes.Normal);
                File.Delete(fileSaveUrl);
            }

            bitmap.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            myImage.Dispose();
            bitmap.Dispose();
        }

        /// <summary>
        /// 判断 远程文件是否存在，200:存在，500：，401：出错，404：不存在
        /// </summary>
        /// <param name="curl"></param>
        /// <returns></returns>
        public static int CheckUrlErrorCode(string curl)
        {
            int num = 200;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(curl));
            ServicePointManager.Expect100Continue = false;
            try
            {
                ((HttpWebResponse)request.GetResponse()).Close();
            }
            catch (WebException exception)
            {
                if (exception.Status != WebExceptionStatus.ProtocolError)
                {
                    return num;
                }
                if (exception.Message.IndexOf("500 ") > 0)
                {
                    return 500;
                }
                if (exception.Message.IndexOf("401 ") > 0)
                {
                    return 401;
                }
                if (exception.Message.IndexOf("404") > 0)
                {
                    num = 404;
                }
            }
            return num;
        }

        /// <summary>
        ///   根据指定宽度，高度，裁剪
        /// </summary>
        /// <param name="fromFileStream">图片流</param>
        /// <param name="fileSaveUrl">保存地址</param>
        /// <param name="templateWidth">指定要裁剪后的宽度</param>
        /// <param name="templateHeight">指定要裁剪后的高度</param>
        /// <param name="mode">裁剪方式</param>
        public static void CacheFilesMakeSmallImg(System.IO.Stream fromFileStream, string fileSaveUrl, System.Double templateWidth, System.Double templateHeight, string mode)
        {

            //从文件取得图片对象，并使用流中嵌入的颜色管理信息  
            System.Drawing.Image myImage = System.Drawing.Image.FromStream(fromFileStream, true);

            int x = 0;
            int y = 0;
            int newWidth = Convert.ToInt32(templateWidth);
            int newHeight = Convert.ToInt32(templateHeight);

            int ow = myImage.Width;
            int oh = myImage.Height;
            //当文件大于模板的宽度时，才生成缩略图
            //if (ow > newWidth)
            //if (File.Exists(fileSaveUrl))
            if (1 == 1)
            {
                switch (mode)
                {
                    case "WH":
                        //如果上传图片的宽度[小于]指定宽度
                        if (myImage.Width < Convert.ToInt32(templateWidth))
                        {
                            //上传图片高度[大于]指定高度
                            if (myImage.Height > Convert.ToInt32(templateHeight))
                            {
                                //指定高，宽按比例
                                newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                            }
                            else//上传的图片的高度[小于]指定高度
                            {
                                //什么都有不做，但是，原样存一个副本到缩略图路径
                                //保存缩略图  
                                newWidth = myImage.Width;
                                newHeight = myImage.Height;
                            }
                        }
                        else//上传图片的宽度[大于]指定宽度
                        {
                            //上传图片高度[大于]指定高度
                            if (myImage.Height > Convert.ToInt32(templateHeight))//上传图片的宽度和高度同时[大于]指定宽高
                            {

                                if (myImage.Width > myImage.Height)//上传图片宽[大于]高
                                {
                                    double whBL = Convert.ToInt32(templateWidth) / Convert.ToInt32(templateHeight);//指定图片的宽高比例
                                    double scwhBL = myImage.Width / myImage.Height;//上传图片的宽高比例

                                    //指定高，宽按比例
                                    newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                                    //指定宽，高按比例              
                                    newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;

                                    if (whBL > scwhBL)//推测，同比例缩放后 ：上传的图片的高度[小于]指定高度
                                    {
                                        newWidth = Convert.ToInt32(templateWidth);
                                    }
                                    else
                                    {
                                        newHeight = Convert.ToInt32(templateHeight);
                                    }

                                }
                                else//上传图片宽[小于]高
                                {
                                    double hwBL = Convert.ToInt32(templateHeight) / Convert.ToInt32(templateWidth);//指定图片的高宽比例
                                    double schwBL = myImage.Width / myImage.Height;//上传图片的高宽比例

                                    //指定高，宽按比例
                                    newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                                    //指定宽，高按比例              
                                    newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;

                                    if (hwBL < schwBL)//推测，同比例缩放后 ：上传的图片的高度[小于]指定高度
                                    {
                                        newWidth = Convert.ToInt32(templateWidth);
                                    }
                                    else
                                    {
                                        newHeight = Convert.ToInt32(templateHeight);
                                    }
                                }
                            }
                            else//上传的图片的高度[小于]指定高度
                            {
                                //指定宽，高按比例                    
                                newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;
                            }
                        }
                        break;


                    case "HW"://指定高宽缩放（可能变形）                
                        break;
                    case "W"://指定宽，高按比例                    
                        newHeight = myImage.Height * Convert.ToInt32(templateWidth) / myImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        newWidth = myImage.Width * Convert.ToInt32(templateHeight) / myImage.Height;
                        break;
                    case "Default"://指定高，宽按比例
                        newWidth = myImage.Width;
                        newHeight = myImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                
                        if ((double)myImage.Width / (double)myImage.Height > (double)newWidth / (double)newHeight)
                        {
                            oh = myImage.Height;
                            ow = myImage.Height * newWidth / newHeight;
                            y = 0;
                            x = (myImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = myImage.Width;
                            oh = myImage.Width * Convert.ToInt32(templateHeight) / newWidth;
                            x = 0;
                            y = (myImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;

                }

                //取得图片大小  
                System.Drawing.Size mySize = new Size((int)newWidth, (int)newHeight);
                //新建一个bmp图片  
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
                //新建一个画板  
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                //设置高质量插值法  
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度  
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空一下画布  
                g.Clear(Color.White);
                //在指定位置画图  
                g.DrawImage(myImage, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
                System.Drawing.GraphicsUnit.Pixel);

                //保存缩略图  
                if (File.Exists(fileSaveUrl))
                {
                    File.SetAttributes(fileSaveUrl, FileAttributes.Normal);
                    File.Delete(fileSaveUrl);
                }

                bitmap.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);
                g.Dispose();
                myImage.Dispose();
                bitmap.Dispose();

            }

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        public static void DeleteFile(string filename)
        {
            string s = HttpContext.Current.Server.MapPath(filename);
            if (File.Exists(s))
            {
                try
                {
                    File.Delete(s);
                }
                catch { }
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filename"></param>
        public static void DeleteFileMapPath(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }


        /// <summary>
        ///   根据Url图片网址，裁剪
        /// </summary>
        /// <param name="picUrl">图片Url</param>
        /// <param name="x">裁剪后，距离左侧像素</param>
        /// <param name="y">裁剪后，距离头部像素</param>
        /// <param name="w">裁剪后，图片宽度</param>
        /// <param name="h">裁剪后，图片高度</param>
        public static string Cutting(string picUrl, int x, int y, int w, int h, string saveUrl = "/upfile/")
        {
            if (!System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~" + saveUrl)))
            {
                System.IO.Directory.CreateDirectory(@System.Web.HttpContext.Current.Server.MapPath("~" + saveUrl));
            }
            WebRequest webrequest = WebRequest.Create(picUrl);//图片完整地址 包含http
            WebResponse webresponse = webrequest.GetResponse();
            //文件流获取图片操作
            Stream reader = webresponse.GetResponseStream();
            string webFilePath = System.Web.HttpContext.Current.Server.MapPath("~/" + saveUrl);
            string[] temp = picUrl.Split('.');
            string imgname = DateTime.Now.ToString("ddHHmmssssffff") + "." + temp[temp.Length - 1].ToLower();
            string path = webFilePath + imgname;//图片路径命名 
            saveUrl = saveUrl + imgname;
            //从文件取得图片对象，并使用流中嵌入的颜色管理信息  
            System.Drawing.Image myImage = System.Drawing.Image.FromStream(reader, true);
            //取得图片大小  
            System.Drawing.Size mySize = new Size(w, h);//截取的图片的宽和高
            //新建一个bmp图片  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(mySize.Width, mySize.Height);
            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空一下画布  
            g.Clear(Color.White);
            //在指定位置画图  
            g.DrawImage(myImage,
                new System.Drawing.Rectangle(0, 0, myImage.Width, myImage.Height),
                new System.Drawing.Rectangle(x, y, myImage.Width, myImage.Height),
                System.Drawing.GraphicsUnit.Pixel);
            
            //保存缩略图  
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }

            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            g.Dispose();
            myImage.Dispose();
            bitmap.Dispose();


            //FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            //try
            //{
            //    byte[] buff = new byte[1024];
            //    int c = 0;//实际读取的字节数
            //    while ((c = reader.Read(buff, 0, buff.Length)) > 0)
            //    {
            //        writer.Write(buff, 0, c);
            //    }
            //}
            //catch (Exception)
            //{
            //    //释放资源
            //    writer.Close();
            //    writer.Dispose();
            //    reader.Close();
            //    reader.Dispose();
            //    webresponse.Close();
            //}
            ////释放资源
            //writer.Close();
            //writer.Dispose();
            //reader.Close();
            //reader.Dispose();
            //webresponse.Close();

            //上传成功
            return saveUrl;
        }


        public string UploadPicUrl(string picUrl, string saveUrl = "/themes/upfile/")
        {
            if (!System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath("~" + saveUrl)))
            {
                System.IO.Directory.CreateDirectory(@System.Web.HttpContext.Current.Server.MapPath("~" + saveUrl));
            }
            WebRequest webrequest = WebRequest.Create(picUrl);//图片src内容
            WebResponse webresponse = webrequest.GetResponse();
            //文件流获取图片操作
            Stream reader = webresponse.GetResponseStream();
            string webFilePath = System.Web.HttpContext.Current.Server.MapPath("~/" + saveUrl);
            string[] temp = webFilePath.Split('.');
            string imgname = DateTime.Now.ToString("ddHHmmssssffff") + "." + temp[temp.Length - 1].ToLower();
            string path = webFilePath + imgname;        //图片路径命名 
            saveUrl = saveUrl + imgname;
            FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                byte[] buff = new byte[1024];
                int c = 0;//实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
            }
            catch (Exception)
            {
                //释放资源
                writer.Close();
                writer.Dispose();
                reader.Close();
                reader.Dispose();
                webresponse.Close();
            }
            //释放资源
            writer.Close();
            writer.Dispose();
            reader.Close();
            reader.Dispose();
            webresponse.Close();
            //上传成功
            return saveUrl;
        }


        /// <summary>
        /// 旋转图片，根据angle角度值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string RotateBitmap(string src, float angle)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                src = System.Web.HttpContext.Current.Server.MapPath(src);
                Image oldImage = Bitmap.FromFile(src);
                Bitmap newBmp = RotateImage(oldImage, angle);
                oldImage.Dispose();
                int nw = newBmp.Width;
                int nh = newBmp.Height;
                newBmp.Save(src);
                newBmp.Dispose();
                //非标准Json格式，在Jquery1.4.x目前测试1.4.2&1.4.4中是不正确的  
                //return "{msg:'success',size:{width:" + nw.ToString() + ",height:" + nh.ToString() + "}}";
                //标准Json格式,记得是双引号，经测试单引号也不能被识别。  
                return "{\"msg\":\"success\",\"size\":{\"width\":\"" + nw.ToString() + "\",\"height\":\"" + nh.ToString() + "\"}}";
            }
            catch (Exception ex)
            {
                return "{msg:'" + ex.Message + "'}";
            }
        }

        /// <summary>
        /// 图像旋转
        /// </summary>
        /// <param name="oldImage">Image类型的对象</param>
        /// <param name="angle">Rotate angle(only -180, -90, 90, 180,)</param>
        /// <returns></returns>
        public static Bitmap RotateImage(Image oldImage, float angle)
        {
            float na = Math.Abs(angle);
            if (na != 90 && na != 180)
                throw new ArgumentException("angle could only be -180, -90, 90, 180 degrees, but now it is " + angle.ToString() + " degrees!");

            #region -Unused(for any degrees)-
            //double radius = angle * Math.PI / 180.0;
            //double cos = Math.Cos(radius);
            //double sin = Math.Sin(radius);

            //int ow = oldImage.Width;
            //int oh = oldImage.Height;
            //int nw = (int)(Math.Abs(ow * cos) + Math.Abs(oh * sin));
            //int nh = (int)(Math.Abs(ow * sin) + Math.Abs(oh * cos));
            #endregion

            int ow = oldImage.Width;
            int oh = oldImage.Height;
            int nw = ow;
            int nh = oh;
            //90/-90
            if (na == 90)
            {
                nw = oh;
                nh = ow;
            }

            Bitmap bmp = new Bitmap(nw, nh);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.SmoothingMode = SmoothingMode.HighQuality;

                Point offset = new Point((nw - ow) / 2, (nh - oh) / 2);
                Rectangle rect = new Rectangle(offset.X, offset.Y, ow, oh);
                Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(angle);
                g.TranslateTransform(-center.X, -center.Y);
                g.DrawImage(oldImage, rect);
                g.ResetTransform();
                g.Save();
            }
            return bmp;
        }

        /// <summary>
        /// Generate the new bitmap
        /// </summary>
        /// <param name="oldImage">A <see cref="System.Drawing.Image"/> source</param>
        /// <param name="cut">The <see cref="BitmapCutter.Core.API.Cutter"/></param>
        /// <returns></returns>
        public static Bitmap GenerateBitmap(Image oldImage, Cutter cut)
        {
            if (oldImage == null)
                throw new ArgumentNullException("oldImage");

            Image newBitmap = new Bitmap(cut.SaveWidth, cut.SaveHeight);
            //Re-paint the oldImage
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(oldImage, new Rectangle(0, 0, cut.SaveWidth, cut.SaveHeight), new Rectangle(0, 0, cut.Width, cut.Height), GraphicsUnit.Pixel);
                g.Save();
            }

            Bitmap bmp = new Bitmap(cut.CutterWidth, cut.CutterHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(newBitmap, 0, 0, new Rectangle(cut.X, cut.Y, cut.CutterWidth, cut.CutterHeight), GraphicsUnit.Pixel);
                g.Save();
                newBitmap.Dispose();
            }
            return bmp;
        }
    }


    /// <summary>
    /// Cutter Model
    /// </summary>
    public class Cutter
    {
        #region -Constructor-
        /// <summary>
        /// Create a new BitmapCutter.Core.API.Cutter instance
        /// </summary>
        public Cutter()
            : this(0, 0, 0, 0, 0, 0, 0, 0, 0)
        { }
        /// <summary>
        /// Create a new BitmapCutter.Core.API.Cutter instance
        /// </summary>
        public Cutter(double _Zoom, int _X, int _Y, int _CutterWidth, int _CutterHeight, int _Width, int _Height)
            : this(_Zoom, _X, _Y, _CutterWidth, _CutterHeight, _Width, _Height, (int)(_Zoom * _Width), (int)(_Zoom * _Height))
        { }
        /// <summary>
        /// Create a new BitmapCutter.Core.API.Cutter instance
        /// </summary>
        public Cutter(double _Zoom, int _X, int _Y, int _CutterWidth, int _CutterHeight, int _Width, int _Height, int _SaveWidth, int _SaveHeight)
        {
            this._Zoom = _Zoom;
            this._X = _X;
            this._Y = _Y;
            this._CutterHeight = _CutterHeight;
            this._CutterWidth = _CutterWidth;
            this._Width = _Width;
            this._Height = _Height;
            this._SaveWidth = _SaveWidth;
            this._SaveHeight = _SaveHeight;
        }
        #endregion

        #region -Properties-
        private int _SaveWidth;
        /// <summary>
        /// Resize bitmap(Width)
        /// </summary>
        public int SaveWidth
        {
            get { return _SaveWidth; }
            set { _SaveWidth = value; }
        }
        private int _SaveHeight;
        /// <summary>
        /// Resize bitmap(Height)
        /// </summary>
        public int SaveHeight
        {
            get { return _SaveHeight; }
            set { _SaveHeight = value; }
        }
        private double _Zoom;
        /// <summary>
        /// Zoom rate
        /// </summary>
        public double Zoom
        {
            get { return _Zoom; }
            set { _Zoom = value; }
        }
        private int _X;
        /// <summary>
        /// X coordinates(from left-top corner)
        /// </summary>
        public int X
        {
            get { return _X; }
            set { _X = value; }
        }
        private int _Y;
        /// <summary>
        /// Y coordinates(from left-top corner)
        /// </summary>
        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        private int _CutterWidth;
        /// <summary>
        /// Width of cutter
        /// </summary>
        public int CutterWidth
        {
            get { return _CutterWidth; }
            set { _CutterWidth = value; }
        }
        private int _CutterHeight;
        /// <summary>
        /// Height of cutter
        /// </summary>
        public int CutterHeight
        {
            get { return _CutterHeight; }
            set { _CutterHeight = value; }
        }
        private int _Width;
        /// <summary>
        /// Width of original size
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        private int _Height;
        /// <summary>
        /// Height of original size
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        #endregion
    }
}


