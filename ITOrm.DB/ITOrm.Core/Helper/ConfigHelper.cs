using System;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ITOrm.Core.Helper
{
    /// <summary>
    /// 读取web.config文件内容帮助类
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// ConnectionStrings方式 ，根据名称读取web.config文件内容
        /// </summary>
        /// <param name="num">集合位数</param>
        /// <returns>返回web.config中配置文件名称对应的值</returns>
        public static string GetConnectionStrings(int num)
        {
            string connectionStrings = String.Empty;
            if (num > 0)
            {
                try
                {
                    connectionStrings = ConfigurationManager.ConnectionStrings[num].ConnectionString;
                }
                catch
                {
                    connectionStrings = "";
                }
            }
            return connectionStrings;
        }


        /// <summary>
        /// ConnectionStrings方式 ，根据名称读取web.config文件内容
        /// </summary>
        /// <param name="name">配置文件名称</param>
        /// <returns>返回web.config中配置文件名称对应的值</returns>
        public static string GetConnectionStrings(string name)
        {
            string connectionStrings = String.Empty;
            if (name != null && !"".Equals(name))
            {
                try
                {
                    connectionStrings = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                }
                catch
                {
                    connectionStrings = "";
                }
            }
            return connectionStrings;
        }

        /// <summary>
        ///获取或者设置提供程序名称属性ConfigurationManager.ConnectionStrings[name].ProviderName 
        /// </summary>
        /// <param name="name">配置文件名称</param>
        /// <returns>返回web.config中配置文件名称对应的值</returns>
        public static string GetConnectionStringsProviderName(string name)
        {
            string connectionStrings = String.Empty;
            if (name != null && !"".Equals(name))
            {
                try
                {
                    connectionStrings = ConfigurationManager.ConnectionStrings[name].ProviderName;
                }
                catch
                {
                    connectionStrings = "";
                }
            }
            return connectionStrings;
        }

        /// <summary>
        ///获取或者设置提供程序名称属性ConfigurationManager.ConnectionStrings[name].ProviderName 
        /// </summary>
        /// <param name="num">集合位数</param>
        /// <returns>返回web.config中配置文件名称对应的值</returns>
        public static string GetConnectionStringsProviderName(int num)
        {
            string connectionStrings = String.Empty;
            if (num > 0)
            {
                try
                {
                    connectionStrings = ConfigurationManager.ConnectionStrings[num].ProviderName;
                }
                catch
                {
                    connectionStrings = "";
                }
            }
            return connectionStrings;
        }

        /// <summary>
        ///获取集合中的元素数 ConfigurationManager.ConnectionStrings.Count
        /// </summary>
        /// <returns></returns>
        public static int GetConnectionStringsCount()
        {
            int num = 0;
            try
            {
                num = ConfigurationManager.ConnectionStrings.Count;
            }
            catch
            {
                num = 0;
            }
            return num;
        }

        /// <summary>
        /// AppSettings方式 ，根据名称读取web.config文件内容
        /// </summary>
        /// <param name="name">配置文件名称</param>
        /// <returns>返回web.config中配置文件名称对应的值</returns>
        public static string GetAppSettings(string name)
        {
            string appSettingss = String.Empty;
            if (name != null && !"".Equals(name))
            {
                try
                {
                    appSettingss = ConfigurationManager.AppSettings[name].ToString();
                }
                catch
                {
                    appSettingss = "";
                }
            }
            return appSettingss;
        }

        /// <summary>
        /// 索引当前应用程序默认配置的指定配置节
        /// </summary>
        /// <param name="name">配置节名称</param>
        /// <returns></returns>
        public static object GetSection(string name)
        {
            object section = String.Empty;
            if (name != null && !"".Equals(name))
            {
                try
                {
                    section = ConfigurationManager.GetSection(name);
                }
                catch
                {
                    section = "";
                }
            }
            return section;
        }

        /// <summary>
        /// 获取属性（string类型）
        /// </summary>
        public static string GetStringAttribute(XmlNode node, string key, string defaultValue)
        {
            XmlAttributeCollection attributes = node.Attributes;
            if (attributes[key] != null && !string.IsNullOrEmpty(attributes[key].Value))
                return attributes[key].Value;
            return defaultValue;
        }

        /// <summary>
        /// 获取属性（string类型），默认值string.Empty
        /// </summary>
        public static string GetStringAttribute(XmlNode node, string key)
        {
            return GetStringAttribute(node, key, string.Empty);
        }

        /// <summary>
        /// 获取属性（int类型）
        /// </summary>
        public static int GetIntAttribute(XmlNode node, string key, int defaultValue)
        {
            int val = defaultValue;
            XmlAttributeCollection attributes = node.Attributes;

            if (attributes[key] != null && !string.IsNullOrEmpty(attributes[key].Value))
            {
                int.TryParse(attributes[key].Value, out val);
            }
            return val;
        }

        /// <summary>
        /// 获取属性（bool类型）
        /// </summary>
        public static bool GetBoolAttribute(XmlNode node, string key, bool defaultValue)
        {
            bool val = defaultValue;
            XmlAttributeCollection attributes = node.Attributes;

            if (attributes[key] != null && !string.IsNullOrEmpty(attributes[key].Value))
            {
                bool.TryParse(attributes[key].Value, out val);
            }
            return val;
        }

        /// <summary>
        /// 从配置节点获取属性值
        /// </summary>
        /// <param name="isMandatory">是否为强制属性</param>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public static string GetAttribute(XmlNode node, string key, bool isMandatory)
        {
            string errMsg = string.Format("配置节点[{0}]必须设置属性[{1}]", node.Name, key);

            if (node.Attributes == null)
                throw new ConfigurationErrorsException(errMsg);

            XmlAttribute attribute = node.Attributes[key];
            if (attribute == null && isMandatory)
                throw new ConfigurationErrorsException(errMsg);

            return attribute == null ? null : attribute.InnerText;
        }

        /// <summary>
        /// 从配置节点获取属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">若配置节点键值为key的属性没有配置，则返回一个默认值</param>
        /// <returns>配置节点键值为key的属性值</returns>
        public static string GetAttribute(XmlNode node, string key, string defaultValue)
        {
            if (node.Attributes.Count == 0) return defaultValue;

            XmlAttribute attribute = node.Attributes[key];
            if (attribute == null) return defaultValue;

            return attribute.InnerText;
        }

        public static Dictionary<string, T> LoadModules<T>(XmlNode node)
        {
            Dictionary<string, T> modules = new Dictionary<string, T>();

            if (node != null)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Comment)
                    {
                        switch (n.Name)
                        {
                            case "clear":
                                modules.Clear();
                                break;
                            case "remove":
                                XmlAttribute removeNameAtt = n.Attributes["name"];
                                string removeName = removeNameAtt == null ? null : removeNameAtt.Value;

                                if (!string.IsNullOrEmpty(removeName) && modules.ContainsKey(removeName))
                                {
                                    modules.Remove(removeName);
                                }

                                break;
                            case "add":

                                XmlAttribute en = n.Attributes["enabled"];
                                if (en != null && en.Value == "false")
                                    continue;

                                XmlAttribute nameAtt = n.Attributes["name"];
                                XmlAttribute typeAtt = n.Attributes["type"];
                                string name = nameAtt == null ? null : nameAtt.Value;
                                string itype = typeAtt == null ? null : typeAtt.Value;

                                if (string.IsNullOrEmpty(name))
                                {
                                    continue;
                                }

                                if (string.IsNullOrEmpty(itype))
                                {
                                    continue;
                                }

                                Type type = Type.GetType(itype);

                                if (type == null)
                                {
                                    continue;
                                }

                                T mod = default(T);

                                try
                                {
                                    mod = (T)Activator.CreateInstance(type);
                                }
                                catch
                                {
                                    //todo: log
                                }

                                if (mod == null)
                                {
                                    continue;
                                }

                                modules.Add(name, mod);
                                break;

                        }
                    }
                }
            }
            return modules;
        }

        /// <summary>
        /// 将配置节点转换成NameValueCollection集合
        /// </summary>
        /// <param name="node">配置节点</param>
        /// <returns></returns>
        public static NameValueCollection XmlNode2NameValueCollection(XmlNode node)
        {
            NameValueCollection nvc = new NameValueCollection();

            foreach (XmlNode item in node)
            {
                nvc.Add(item.Name, item.InnerXml);
            }

            return nvc;
        }
    }
}
