﻿namespace ITOrm.Core.Security
{
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Common
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Common()
        {
        }

        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ITOrm.Utility.Common", typeof(Common).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   查找类似 参数[{0}]不能为空 的本地化字符串。
        /// </summary>
        internal static string ArgumentNotEmpty
        {
            get
            {
                return ResourceManager.GetString("ArgumentNotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   查找类似 参数[{0}]不能为Null 的本地化字符串。
        /// </summary>
        internal static string ArgumentNotNull
        {
            get
            {
                return ResourceManager.GetString("ArgumentNotNull", resourceCulture);
            }
        }

        /// <summary>
        ///   查找类似 参数[{0}]不能为空白字符 的本地化字符串。
        /// </summary>
        internal static string ArgumentNotWhitespace
        {
            get
            {
                return ResourceManager.GetString("ArgumentNotWhitespace", resourceCulture);
            }
        }

        /// <summary>
        ///   查找类似 类型转换失败，原始类型[{0}] -&gt; 目标类型[{1}] 的本地化字符串。
        /// </summary>
        internal static string TypeConvertFailed
        {
            get
            {
                return ResourceManager.GetString("TypeConvertFailed", resourceCulture);
            }
        }
    }
}
