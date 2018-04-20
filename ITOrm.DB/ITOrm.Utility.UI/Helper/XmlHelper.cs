using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Web;
using System.Text.RegularExpressions;

namespace ITOrm.Core.Utility.Helper
{
    public class XmlHelper
    {

        /// <summary>
        /// 通过xml和xslt数据得到html 代码
        /// </summary>
        /// <param name="xmlData">xml字符串</param>
        /// <param name="xsltData">xslt字符串</param>
        /// <returns></returns>
        public static string GetHTMLByXmlAndXsltData(string xmlData, string xsltData)
        {
            try
            {
                TextReader tr = new StringReader(xsltData);
                XmlTextReader xmlTextReader = new XmlTextReader(tr);
                StringReader strReader = new StringReader(xmlData);

                XslCompiledTransform tgXslt = new XslCompiledTransform();
                XmlDocument tgXml = new XmlDocument();

                tgXslt.Load(xmlTextReader);
                tgXml.Load(strReader);

                MemoryStream t = new MemoryStream();
                tgXslt.Transform(tgXml, null, t);
                string resultString = Encoding.Default.GetString(t.ToArray()).Trim();
                resultString = resultString.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                return resultString;
            }
            catch
            {
                return "";
            }
        }

        public static bool WriteHTMLByXmlAndXsltData(string xmlFileName, string xsltFileName, string htmlFilePath)
        {
            try
            {
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(xsltFileName);
                transform.Transform(xmlFileName, htmlFilePath);
                return true;

            }
            catch (Exception exp)
            {
                throw exp;
                //rtnReason = exp.ToString();
                //return false;
            }
        }

        public static bool GetHtmlFileByXmlStreamAndXsltFile(XmlDocument xdoc, string xsltFileName, string htmlFilePath)
        {
            try
            {
                /// Check Dir is exist ? if not create it .
                string fileDir = "";
                if (htmlFilePath.LastIndexOf("\\") != -1)
                    fileDir = htmlFilePath.Substring(0, htmlFilePath.LastIndexOf("\\"));
                if (!Directory.Exists(fileDir))
                    Directory.CreateDirectory(fileDir);
                FileInfo file = new FileInfo(htmlFilePath);
                XslCompiledTransform xslTrans = new XslCompiledTransform();
                xslTrans.Load(xsltFileName);
                //将转换的结果保存在内存流中，然后读出到字符串，返回
                StreamWriter writer = file.CreateText();
                xslTrans.Transform(xdoc, null, writer);
                writer.Close();
                return true;
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }

    public class XMLBuilder
    {

        private string xmlFilePath;
        private EnumXmlPathType xmlFilePathType;
        private XmlDocument xmlDoc = new XmlDocument();

        public string XmlFilePath
        {
            set
            {
                xmlFilePath = value;

            }
        }

        public EnumXmlPathType XmlFilePathTyp
        {
            set
            {
                xmlFilePathType = value;
            }
        }

        public XMLBuilder(string tempXmlFilePath)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.xmlFilePathType = EnumXmlPathType.VirtualPath;
            this.xmlFilePath = tempXmlFilePath;
            GetXmlDocument();
            //xmlDoc.Load( xmlFilePath );
        }

        public XMLBuilder(string tempXmlFilePath, EnumXmlPathType tempXmlFilePathType)
        {
            this.xmlFilePathType = tempXmlFilePathType;
            this.xmlFilePath = tempXmlFilePath;
            GetXmlDocument();
        }

        /// </summary>
        /// <param name="strEntityTypeName">实体类的名称</param>
        /// <returns>指定的XML描述文件的路径</returns>
        public XmlDocument GetXmlDocument()
        {
            XmlDocument doc = null;

            if (this.xmlFilePathType == EnumXmlPathType.AbsolutePath)
            {
                doc = GetXmlDocumentFromFile(xmlFilePath);
            }
            else if (this.xmlFilePathType == EnumXmlPathType.VirtualPath)
            {
                doc = GetXmlDocumentFromFile(HttpContext.Current.Server.MapPath(xmlFilePath));
            }
            return doc;
        }

        private XmlDocument GetXmlDocumentFromFile(string tempXmlFilePath)
        {
            string xmlFileFullPath = tempXmlFilePath;

            xmlDoc.Load(xmlFileFullPath);
            return xmlDoc;
        }

        #region 读取指定节点的指定属性值

        /// <summary>
        /// 功能:
        /// 读取指定节点的指定属性值
        /// 
        /// 参数:
        /// 参数一:节点名称
        /// 参数二:此节点的属性
        /// </summary>
        /// <param name="strNode"></param>
        /// <param name="strAttribute"></param>
        /// <returns></returns>
        public string GetXmlNodeValue(string strNode, string strAttribute)
        {
            string strReturn = "";
            try
            {
                //根据指定路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(strNode);
                //获取节点的属性，并循环取出需要的属性值
                XmlAttributeCollection xmlAttr = xmlNode.Attributes;

                for (int i = 0; i < xmlAttr.Count; i++)
                {
                    if (xmlAttr.Item(i).Name == strAttribute)
                        strReturn = xmlAttr.Item(i).Value;
                }
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
            return strReturn;
        }

        #endregion

        #region

        /// <summary>
        /// 功能:
        /// 读取指定节点的值
        /// 
        /// 参数:
        /// 参数:节点名称
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public string GetXmlNodeValue(string strNode)
        {
            string strReturn = String.Empty;
            try
            {
                //根据路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(strNode);
                if (xmlNode != null)
                    strReturn = xmlNode.InnerText;
            }
            catch (XmlException xmle)
            {
                System.Console.WriteLine(xmle.Message);
            }
            return strReturn;
        }

        #endregion

        public string GetXmlNodeOuterXml(string strNode)
        {
            string strReturn = String.Empty;
            try
            {
                //根据路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(strNode);
                if (xmlNode != null)
                    strReturn = xmlNode.OuterXml;
            }
            catch (XmlException xmle)
            {
                System.Console.WriteLine(xmle.Message);
            }
            return strReturn;
        }

        #region 设置节点值
        /// <summary>
        /// 功能:
        /// 设置节点值
        /// 
        /// 参数:
        ///    参数一:节点的名称
        ///    参数二:节点值
        ///    
        /// </summary>
        /// <param name="strNode"></param>
        /// <param name="newValue"></param>
        public void SetXmlNodeValue(string xmlNodePath, string xmlNodeValue)
        {
            try
            {
                //根据指定路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xmlNodePath);
                //设置节点值
                xmlNode.InnerText = xmlNodeValue;
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #endregion

        #region 设置节点的属性值
        /// <summary>
        /// 功能:
        /// 设置节点的属性值
        /// 
        /// 参数:
        /// 参数一:节点名称
        /// 参数二:属性名称
        /// 参数三:属性值
        /// 
        /// </summary>
        /// <param name="xmlNodePath"></param>
        /// <param name="xmlNodeAttribute"></param>
        /// <param name="xmlNodeAttributeValue"></param>
        public void SetXmlNodeValue(string xmlNodePath, string xmlNodeAttribute, string xmlNodeAttributeValue)
        {
            try
            {
                //根据指定路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xmlNodePath);

                //获取节点的属性，并循环取出需要的属性值
                XmlAttributeCollection xmlAttr = xmlNode.Attributes;
                for (int i = 0; i < xmlAttr.Count; i++)
                {
                    if (xmlAttr.Item(i).Name == xmlNodeAttribute)
                    {
                        xmlAttr.Item(i).Value = xmlNodeAttributeValue;
                        break;
                    }
                }
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #endregion

        /// <summary>
        /// 获取XML文件的根元素
        /// </summary>
        public XmlNode GetXmlRoot()
        {
            return xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 在根节点下添加父节点
        /// </summary>
        public void AddParentNode(string parentNode)
        {
            XmlNode root = GetXmlRoot();
            XmlNode parentXmlNode = xmlDoc.CreateElement(parentNode);
            root.AppendChild(parentXmlNode);
        }

        /// <summary>
        /// 向一个已经存在的父节点中插入一个子节点
        /// </summary>
        public void AddChildNode(string parentNodePath, string childNodePath)
        {
            XmlNode parentXmlNode = xmlDoc.SelectSingleNode(parentNodePath);
            XmlNode childXmlNode = xmlDoc.CreateElement(childNodePath);
            parentXmlNode.AppendChild(childXmlNode);
        }

        public void AddChildNode(string parentNodePath, string childNodePath, string innerXmlText)
        {
            XmlNode parentXmlNode = xmlDoc.SelectSingleNode(parentNodePath);
            XmlNode childXmlNode = xmlDoc.CreateElement(childNodePath);
            parentXmlNode.AppendChild(childXmlNode);
            XmlNode theNode = xmlDoc.SelectSingleNode(parentNodePath + "/" + childNodePath);
            theNode.InnerXml = innerXmlText;
        }

        /// <summary>
        /// 向一个节点添加属性
        /// </summary>
        public void AddAttribute(string NodePath, string NodeAttribute)
        {
            XmlAttribute nodeAttribute = xmlDoc.CreateAttribute(NodeAttribute);
            XmlNode nodePath = xmlDoc.SelectSingleNode(NodePath);
            nodePath.Attributes.Append(nodeAttribute);
        }

        /// <summary>
        /// 删除一个节点的属性
        /// </summary>
        public void DeleteAttribute(string NodePath, string NodeAttribute, string NodeAttributeValue)
        {
            XmlNodeList nodePath = xmlDoc.SelectSingleNode(NodePath).ChildNodes;

            foreach (XmlNode xn in nodePath)
            {
                XmlElement xe = (XmlElement)xn;



                if (xe.GetAttribute(NodeAttribute) == NodeAttributeValue)
                {
                    xe.RemoveAttribute(NodeAttribute);//删除属性
                }
            }

        }

        /// <summary>
        /// 删除一个节点
        /// </summary>
        public void DeleteXmlNode(string tempXmlNode)
        {
            //  XmlNodeList     xmlNodePath = xmlDoc.SelectSingleNode(tempXmlNode).ChildNodes;

            XmlNode xmlNodePath = xmlDoc.SelectSingleNode(tempXmlNode);
            if (xmlNodePath != null)
            {
                xmlNodePath.ParentNode.RemoveChild(xmlNodePath);
                foreach (XmlNode xn in xmlNodePath)
                {
                    XmlElement xe = (XmlElement)xn;
                    xe.RemoveAll();
                    //xe.RemoveChild(xn);

                    xn.RemoveAll();

                    if (xe.HasChildNodes)
                    {

                        foreach (XmlNode xnode in xe)
                        {

                            xnode.RemoveAll();//删除所有子节点和属性
                        }
                    }

                }
            }
        }

        #region 保存XML文件
        /// <summary>
        /// 功能: 
        /// 保存XML文件
        /// 
        /// </summary>
        public void SaveXmlDocument()
        {
            try
            {
                //保存设置的结果

                if (this.xmlFilePathType == EnumXmlPathType.AbsolutePath)
                {
                    xmlDoc.Save(xmlFilePath);
                }
                else if (this.xmlFilePathType == EnumXmlPathType.VirtualPath)
                {
                    xmlDoc.Save(HttpContext.Current.Server.MapPath(xmlFilePath));
                }


            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #endregion

        #region 保存XML文件

        /// <summary>
        /// 功能: 
        /// 保存XML文件
        /// 
        /// </summary>
        public void SaveXmlDocument(string tempXMLFilePath)
        {
            try
            {
                //保存设置的结果
                xmlDoc.Save(tempXMLFilePath);
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }

        #endregion

    }

    public enum EnumXmlPathType
    {
        AbsolutePath,
        VirtualPath
    }

    public class XmlUtility
    {

        /// <summary>
        /// 根据提供的xpath创建节点,并返回最终的页节点,
        /// 支持xpath的属性，支持是否创建属性节点
        /// </summary>
        /// <param name="xPath">相对于当前节点的xpath表达式，只支持简单的xpath，
        ///	如：a[@a='c']/b/c[@b='a' or @b='f']/d，
        ///		a[@a='c']/b/c[@b='a' and @a='f']/d,
        ///		a/b/c/d
        /// </param>
        /// <param name="rootNode">需要创建节点的根节点</param>
        /// <param name="IsCreateAttribute">是否创建属性节点</param>
        /// <returns>最后创建的页节点</returns>
        public static XmlNode GetOrCreateXmlTree(string xPath, XmlNode node)
        {
            return GetOrCreateXmlTree(xPath, node, true);
        }

        /// <summary>
        /// 根据提供的xpath创建节点,并返回最终的页节点,
        /// 支持xpath的属性，支持是否创建属性节点
        /// </summary>
        /// <param name="xPath">相对于当前节点的xpath表达式，只支持简单的xpath，
        ///	如：a[@a='c']/b/c[@b='a' or @b='f']/d，
        ///		a[@a='c']/b/c[@b='a' and @a='f']/d,
        ///		a/b/c/d
        /// </param>
        /// <param name="rootNode">需要创建节点的根节点</param>
        /// <param name="IsCreateAttribute">是否创建属性节点</param>
        /// <returns>最后创建的页节点</returns>
        public static XmlNode GetOrCreateXmlTree(string xPath, XmlNode node, bool IsCreateAttribute)
        {
            if (node.NodeType != XmlNodeType.Element)
            {
                throw new ArgumentException("传入的节点不是一个XmlNodeType.Element节点");
            }

            XmlDocument tempXdoc = node.OwnerDocument;
            XmlNode fatherNode = node;
            XmlNode sonNode = null;
            xPath = xPath.Trim('/');

            string regstr = @"(?<path>(?<name>\w+)[^/]*)";

            MatchCollection mc = Regex.Matches(xPath, regstr);

            foreach (Match match in mc)
            {
                string name = match.Groups["name"].Value;
                string path = match.Groups["path"].Value;
                sonNode = fatherNode.SelectSingleNode(path);
                if (sonNode == null)
                {
                    sonNode = tempXdoc.CreateElement(name);
                    fatherNode.AppendChild(sonNode);
                    if (IsCreateAttribute)
                    {
                        string attrRegx = "@(?<aName>\\w+)=['|\"]?(?<aValue>\\w+)['|\"]?";
                        MatchCollection amc = Regex.Matches(path, attrRegx);
                        foreach (Match amatch in amc)
                        {
                            string aName = amatch.Groups["aName"].Value;
                            string aValue = amatch.Groups["aValue"].Value;
                            sonNode.Attributes.Append(tempXdoc.CreateAttribute(aName));
                            sonNode.Attributes[aName].Value = aValue;
                        }
                    }
                }
                fatherNode = sonNode;
            }
            return sonNode;
        }

        /// <summary>
        /// 为xml节点的属性赋值，如果属性不存在则创建属性并赋值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        public static void SetAtrributeValue(XmlNode node, string attrName, string attrValue)
        {
            if (node.NodeType != XmlNodeType.Element)
            {
                throw new ArgumentException("传入的节点不是一个XmlNodeType.Element节点");
            }

            XmlDocument tempXdoc = node.OwnerDocument;

            if (node.Attributes[attrName] == null)
            {
                node.Attributes.Append(tempXdoc.CreateAttribute(attrName));
            }
            node.Attributes[attrName].Value = attrValue;
        }

        /// <summary>
        /// 添加一个新节点，如果有内容，同时为节点赋值，如果没有，则只创建
        /// </summary>
        /// <param name="fathernode"></param>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static XmlNode AddNewNode(XmlNode fathernode, string name, string content)
        {
            XmlDocument xdoc = fathernode.OwnerDocument;
            XmlNode snode = xdoc.CreateElement(name);
            if (!String.IsNullOrEmpty(content))
            {
                snode.InnerXml = ReplaceInvalidChar(content);
            }
            fathernode.AppendChild(snode);
            return snode;
        }

        public static string ReplaceInvalidChar(string instr)
        {
            string result = instr.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;");
            return result;
        }

        public static XmlNode AddCDataNode(XmlNode fathernode, string name, string content)
        {
            XmlDocument xdoc = fathernode.OwnerDocument;
            XmlNode snode = xdoc.CreateElement(name);
            XmlCDataSection CData = xdoc.CreateCDataSection(content);
            snode.AppendChild((XmlNode)CData);
            fathernode.AppendChild(snode);
            return snode;
        }
    }

    public class XsltHelper
    {
        public static string TransformToHtml(string XsltFile, string XmlFile, Encoding encode)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(XmlFile);

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(XsltFile);

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms, encode);

            xslt.Transform(xdoc, null, sw);
            sw.Flush();
            sw.Close();
            sw.Dispose();

            StreamReader sr = new StreamReader(ms, encode);
            string html = sr.ReadToEnd();
            ms.Close();
            sr.Close();

            return html;
        }


        public static string TransformToHtml(string XsltFile, XmlDocument xdoc, Encoding encode)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(XsltFile);

            //将转换的结果保存在内存流中，然后返回文件名
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms, encode);
            xslt.Transform(xdoc, null, writer);
            writer.Flush();

            byte[] bt = ms.ToArray();
            ms.Write(bt, 0, Convert.ToInt32(ms.Length));

            return encode.GetString(bt);
        }
    }

}
