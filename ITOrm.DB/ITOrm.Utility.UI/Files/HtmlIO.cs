using System.Web;
using System.Collections;

namespace ITOrm.Core.Utility.Files
{
    public class HtmlIO
    {
        public HtmlIO() { }

        public static string Reader(string Path)
        {
            return HtmlHelper.Reader(HttpContext.Current.Server.MapPath(Path), "utf-8");
        }

        public static void WriterMapPath(string Path, string Text)
        {
            HtmlHelper.Writer(Path, Text, "utf-8");
        }

        public static void Writer(string Path, string Text)
        {
            HtmlHelper.Writer(HttpContext.Current.Server.MapPath(Path), Text, "utf-8");
        }

        public static void Delete(string Path)
        {
            HtmlHelper.Delete(HttpContext.Current.Server.MapPath(Path));
        }

        public static void AllDelete(string Path)
        {
            HtmlHelper.AllDelete(HttpContext.Current.Server.MapPath(Path));
        }

        public static string Replace(string Source, Hashtable Ht)
        {
            foreach (DictionaryEntry De in Ht)
            {
                Source = Source.Replace(De.Key.ToString(), De.Value.ToString());
            }
            return Source;
        }

    }
}
