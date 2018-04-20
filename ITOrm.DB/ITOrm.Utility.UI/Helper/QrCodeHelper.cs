using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ITOrm.Core.Utility.Helper
{
    /// <summary>
    /// 生成二维码
    /// </summary>
    public class QrCodeHelper
    {
        public static readonly QrCodeHelper Instance = new QrCodeHelper();

        private QrCodeHelper()
        {
        }

        /// <summary>
        /// 二维码二进制流
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] QrCodeCreate(string str)
        {
            QrCode qrcode = new QrEncoder().Encode(str);
            GraphicsRenderer gRenderer = new GraphicsRenderer(new FixedModuleSize(6, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            MemoryStream ms = new MemoryStream();
            gRenderer.WriteToStream(qrcode.Matrix, ImageFormat.Png, ms);
            Image image = Image.FromStream(ms);
            MemoryStream ms1 = new MemoryStream();
            image.Save(ms1, ImageFormat.Png);
            byte[] arr = new byte[ms1.Length];
            ms1.Position = 0L;
            ms1.Read(arr, 0, (int)ms1.Length);
            ms1.Close();
            return arr;
        }

        /// <summary>
        /// 直接输出二维码图片
        /// </summary>
        /// <param name="str"></param>
        public void Api(string str)
        {
            QrCode qrcode = new QrEncoder().Encode(str);
            GraphicsRenderer gRenderer = new GraphicsRenderer(new FixedModuleSize(6, QuietZoneModules.Two), Brushes.Black, Brushes.White);
            MemoryStream ms = new MemoryStream();
            gRenderer.WriteToStream(qrcode.Matrix, ImageFormat.Png, ms);
            Image image = Image.FromStream(ms);
            image.Save(ms, ImageFormat.Png);
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ContentType = "image/x-png";
            System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            image.Dispose();
        }
    }
}