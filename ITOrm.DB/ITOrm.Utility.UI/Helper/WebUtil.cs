using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ITOrm.Core.Utility.Helper
{
    public static class WebUtil
    {

        /// <summary>
        /// 返回试图名称，电脑端 还是 移动端
        /// </summary>
        /// <returns></returns>
        public static string ReturnPcOrMobile()
        {
            string u = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"android.+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od|ad)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!(b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                //"对不起,请用手机访问！";
                return "Briefing";
            }
            return "";
        }

        /// <summary>
        /// 把接收到得参数复制到实体中，包含文件的上传
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_request"></param>
        /// <param name="_NewModel"></param>
        /// <param name="_PostModel"></param>
        /// <returns></returns>
        public static T ConvertRequestToModel<T>(this HttpRequestBase _request, T _NewModel, T _PostModel)
        {
            Type myType = _PostModel.GetType();

            Type saveType = _NewModel.GetType();
            //复制Post的参数
            for (int i = 0; i < _request.Form.Count; i++)
            {
                PropertyInfo pinfo = myType.GetProperty(_request.Form.Keys[i]);
                PropertyInfo saveInfo = saveType.GetProperty(_request.Form.Keys[i]);
                if (saveInfo != null)
                {
                    object v = pinfo.GetValue(_PostModel, null);
                    saveInfo.SetValue(_NewModel, v, null);
                }

            }

            //复制Get的参数
            for (int i = 0; i < _request.QueryString.Count; i++)
            {
                if (_request.QueryString.Keys[i] != null)
                {
                    PropertyInfo pinfo = myType.GetProperty(_request.QueryString.Keys[i]);
                    PropertyInfo saveInfo = saveType.GetProperty(_request.QueryString.Keys[i]);
                    if (saveInfo != null)
                    {
                        object v = pinfo.GetValue(_PostModel, null);
                        if (v != null)
                        {
                            saveInfo.SetValue(_NewModel, v, null);
                        }
                    }
                }
            }

            //批量上传文件
            //System.Collections.Generic.List<FileLoadCallBack>
            //        _FileLoadCallBackList = ImageUpload.FileLoad(_request);

            //给对应的实体赋值
            //for (int i = 0; i < _FileLoadCallBackList.Count; i++)
            //{
            //    if (_FileLoadCallBackList[i].BigName != null)
            //    {
            //        PropertyInfo bigPinfo = saveType.GetProperty(_FileLoadCallBackList[i].BigName);
            //        if (bigPinfo != null)
            //        {
            //            if (_FileLoadCallBackList[i].IsError == false)
            //            {
            //                bigPinfo.SetValue(_NewModel, _FileLoadCallBackList[i].BigPic, null);
            //            }
            //        }
            //    }
            //    if (_FileLoadCallBackList[i].SmallName != null)
            //    {
            //        PropertyInfo smallPinfo = saveType.GetProperty(_FileLoadCallBackList[i].SmallName);
            //        if (smallPinfo != null)
            //        {
            //            if (_FileLoadCallBackList[i].IsError == false)
            //            {
            //                smallPinfo.SetValue(_NewModel, _FileLoadCallBackList[i].SmallPic, null);
            //            }
            //        }
            //    }
            //}
            return _NewModel;
        }


        #region 获取post和get提交值

        /// <summary>
        /// 获取post和get提交值
        /// 若转换失败,则返回默认值 string默认值为string.Empty|int默认值为0|bool默认值为False
        /// </summary>
        /// <typeparam name="T">支持string|int|bool三种类型</typeparam>
        /// <param name="valueName">参数名称</param>
        /// <returns></returns>
        public static T QueryString<T>(string valueName)
        {
            HttpContext rq = HttpContext.Current;
            T TempValue = default(T);

            if (rq.Request.QueryString[valueName] != null)
            {
                if (default(T) == null)//因为string的默认值为null
                {
                    TempValue = (T)Convert.ChangeType(rq.Request.QueryString[valueName], typeof(T));
                }
                else if (default(T).ToString() == "0")//int的默认值为0 仅仅为int类型时需要特殊处理
                {
                    //转换成功
                    int intValue = 0;
                    if (Int32.TryParse(rq.Request.QueryString[valueName], out intValue))
                    {
                        TempValue = (T)Convert.ChangeType(intValue, typeof(T));
                    }
                }
                else if (default(T).ToString() == "False")//如bool类型等
                {
                    bool boolValue = false;
                    if (bool.TryParse(rq.Request.QueryString[valueName], out boolValue))
                    {
                        TempValue = (T)Convert.ChangeType(boolValue, typeof(T));
                    }
                }
            }
            if (TempValue == null)//若TempValue==null时,则值为String.Empty 这样做是为了容错处理
            {
                TempValue = (T)Convert.ChangeType(string.Empty, typeof(T));
            }
            return TempValue;
        }

        #endregion

        /// <summary>
        /// 返回 URL 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        #region 获取客户端IP
        /// <summary>
        /// 获取客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string result = String.Empty;
            result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }

            if (null == result || result == String.Empty || !IsIP(result))
            {
                return "0.0.0.0";
            }
            return result;
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        /// <param name="Code">图片中验证码的内容</param>
        /// <param name="CodeLength">验证码位数</param>
        /// <param name="Width">图片宽度</param>
        /// <param name="Height">图片高度</param>
        /// <param name="FontSize">字体大小</param>
        /// <returns></returns>
        public static byte[] CreateValidateGraphic(out String Code, int CodeLength, int Width, int Height, int FontSize)
        {
            String sCode = String.Empty;
            //顏色列表，用于验证码、噪线、噪点
            Color[] oColors ={
             System.Drawing.Color.Black,
             System.Drawing.Color.Red,
             System.Drawing.Color.Blue,
             System.Drawing.Color.Green,
             System.Drawing.Color.Orange,
             System.Drawing.Color.Brown,
             System.Drawing.Color.Brown,
             System.Drawing.Color.DarkBlue
            };
            //字体列表，用于验证码
            string[] oFontNames = { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" };
            //验证码的字元集，去掉了一些容易混淆的字元
            char[] oCharacter = { '2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random oRnd = new Random();
            Bitmap oBmp = null;
            Graphics oGraphics = null;
            int N1 = 0;
            System.Drawing.Point oPoint1 = default(System.Drawing.Point);
            System.Drawing.Point oPoint2 = default(System.Drawing.Point);
            string sFontName = null;
            Font oFont = null;
            Color oColor = default(Color);

            //生成验证码字符串
            for (N1 = 0; N1 <= CodeLength - 1; N1++)
            {
                sCode += oCharacter[oRnd.Next(oCharacter.Length)];
            }

            oBmp = new Bitmap(Width, Height);
            oGraphics = Graphics.FromImage(oBmp);
            oGraphics.Clear(System.Drawing.Color.White);
            try
            {
                for (N1 = 0; N1 <= 4; N1++)
                {
                    //画噪线
                    oPoint1.X = oRnd.Next(Width);
                    oPoint1.Y = oRnd.Next(Height);
                    oPoint2.X = oRnd.Next(Width);
                    oPoint2.Y = oRnd.Next(Height);
                    oColor = oColors[oRnd.Next(oColors.Length)];
                    oGraphics.DrawLine(new Pen(oColor), oPoint1, oPoint2);
                }

                float spaceWith = 0, dotX = 0, dotY = 0;
                if (CodeLength != 0)
                {
                    spaceWith = (Width - FontSize * CodeLength - 10) / CodeLength;
                }

                for (N1 = 0; N1 <= sCode.Length - 1; N1++)
                {
                    //验证码
                    sFontName = oFontNames[oRnd.Next(oFontNames.Length)];
                    oFont = new Font(sFontName, FontSize, FontStyle.Italic);
                    oColor = oColors[oRnd.Next(oColors.Length)];

                    dotY = (Height - oFont.Height) / 2 + 2;//中心下移2像素
                    dotX = Convert.ToSingle(N1) * FontSize + (N1 + 1) * spaceWith;

                    oGraphics.DrawString(sCode[N1].ToString(), oFont, new SolidBrush(oColor), dotX, dotY);
                }

                for (int i = 0; i <= 30; i++)
                {
                    //画面噪点
                    int x = oRnd.Next(oBmp.Width);
                    int y = oRnd.Next(oBmp.Height);
                    Color clr = oColors[oRnd.Next(oColors.Length)];
                    oBmp.SetPixel(x, y, clr);
                }

                Code = sCode;
                //保存图片数据
                MemoryStream stream = new MemoryStream();
                oBmp.Save(stream, ImageFormat.Jpeg);
                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                oGraphics.Dispose();
            }
        }

    }
}

