using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class DynamicExtensions
{

    public static string ToSting(dynamic value)
    {
        if (value == null)
        {
            return "";
        }
        else
        {
            string str = value;
            return str;
        }
    }

}

