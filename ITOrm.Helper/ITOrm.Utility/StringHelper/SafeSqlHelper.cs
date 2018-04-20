using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ITOrm.Utility.StringHelper
{
  public  class SafeSqlHelper
    {
      public static string SafeSql(string str)
      {
          str = string.IsNullOrEmpty(str) ? "" : str.Replace("'", "''");
          str = new Regex("exec", RegexOptions.IgnoreCase).Replace(str, "&#101;xec");
          str = new Regex("xp_cmdshell", RegexOptions.IgnoreCase).Replace(str, "&#120;p_cmdshell");
          str = new Regex("select", RegexOptions.IgnoreCase).Replace(str, "&#115;elect");
          str = new Regex("insert", RegexOptions.IgnoreCase).Replace(str, "&#105;nsert");
          str = new Regex("update", RegexOptions.IgnoreCase).Replace(str, "&#117;pdate");
          str = new Regex("delete", RegexOptions.IgnoreCase).Replace(str, "&#100;elete");
          str = new Regex("drop", RegexOptions.IgnoreCase).Replace(str, "&#100;rop");
          str = new Regex("create", RegexOptions.IgnoreCase).Replace(str, "&#99;reate");
          str = new Regex("rename", RegexOptions.IgnoreCase).Replace(str, "&#114;ename");
          str = new Regex("truncate", RegexOptions.IgnoreCase).Replace(str, "&#116;runcate");
          str = new Regex("alter", RegexOptions.IgnoreCase).Replace(str, "&#97;lter");
          str = new Regex("exists", RegexOptions.IgnoreCase).Replace(str, "&#101;xists");
          str = new Regex("master.", RegexOptions.IgnoreCase).Replace(str, "&#109;aster.");
          str = new Regex("restore", RegexOptions.IgnoreCase).Replace(str, "&#114;estore");
          return str;
      }

    }
}
