using System;
using System.Collections.Generic;
using System.Xml;
using System.Configuration;
using System.Runtime.Remoting;
using System.Reflection;
using ITOrm.Core.Helper;

namespace ITOrm.Core.Remoting
{
    /// <summary>
    /// Remoting Config
    /// </summary>
    public class RemotingConfig
    {
        public static readonly RemotingConfig Instance = new RemotingConfig();

        private RemotingConfig()
        {
            XmlNode root = (XmlNode)ConfigurationManager.GetSection("ITOrmConfig/remoting");
            if (root == null) return;

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                switch (node.Name)
                {
                    case "client":
                        ParseClientSection(node);
                        break;
                    case "server":
                        ParseServerSection(node);
                        break;
                    default:
                        break;
                }
            }
        }

        Dictionary<string, ClientConfig> _ClientInfoCollection = new Dictionary<string, ClientConfig>();
        Dictionary<string, ServerConfig> _ServerInfoCollection = new Dictionary<string, ServerConfig>();

        public Dictionary<string, ClientConfig> ClientInfoCollection
        {
            get { return _ClientInfoCollection; }
        }

        #region Client Config Parse

        ClientConfig ci;

        //解析客户端配置节
        void ParseClientSection(XmlNode root)
        {
            ci = new ClientConfig();

            ci.Name = ConfigHelper.GetStringAttribute(root, "name", "default");
            ci.LBEnabled = ConfigHelper.GetBoolAttribute(root, "lbEnabled", false);
            ci.PollingInterval = ConfigHelper.GetIntAttribute(root, "pollingInterval", 1);

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                switch (node.Name)
                {
                    case "host":
                        ParseHostSection(node);
                        break;
                    case "wellknown":
                        ParseWellKnownClientObjectSection(node);
                        break;
                    //case "activated":
                    //    ParseActivatedClientObjectSection(node);
                    //    break;
                    default:
                        break;
                }
            }

            if (!_ClientInfoCollection.ContainsKey(ci.Name))
                _ClientInfoCollection.Add(ci.Name, ci);
        }

        //解析Remoting宿主
        void ParseHostSection(XmlNode root)
        {
            string defaultHostName = ConfigHelper.GetStringAttribute(root, "default");

            Host firstHost = null;

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                if (node.Name != "add") continue;

                if (node.Attributes["name"] == null ||
                    node.Attributes["url"] == null ||
                    string.IsNullOrEmpty(node.Attributes["name"].Value) ||
                    string.IsNullOrEmpty(node.Attributes["url"].Value)) continue;

                Host host = new Host();
                host.Name = node.Attributes["name"].Value;
                host.Url = node.Attributes["url"].Value;

                ci.AddHost(host);

                if (firstHost == null) firstHost = host;
            }

            if (ci.HostCollection.Count == 0)
            {
                throw new Exception("没有配置Remoting宿主");
            }
            else
            {
                if (ci.HostCollection.ContainsKey(defaultHostName))
                {
                    ci.DefaultHost = ci.HostCollection[defaultHostName];
                }
                else
                {
                    ci.DefaultHost = firstHost;
                }
            }
        }

        //解析并注册WellKnown对象
        void ParseWellKnownClientObjectSection(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                if (node.Name != "add") continue;

                string objectUri = ConfigHelper.GetStringAttribute(node, "objectUri");
                if (string.IsNullOrEmpty(objectUri)) continue;

                string fullTypeName = ConfigHelper.GetStringAttribute(node, "type");
                if (string.IsNullOrEmpty(fullTypeName)) continue;

                string typeName = fullTypeName.Split(',')[0];
                string assemblyName = string.Empty;

                if (fullTypeName.IndexOf(',') != -1)
                {
                    assemblyName = fullTypeName.Split(',')[1];
                }
                else
                {
                    Assembly assembly = Util.FindAssemblyFromAppDirectory(typeName);
                    if (assembly != null)
                        assemblyName = assembly.FullName;
                }

                WellKnownClientTypeEntry wce = new WellKnownClientTypeEntry(typeName, assemblyName, objectUri);
                RemotingConfiguration.RegisterWellKnownClientType(wce);
            }
        }

        //解析并注册Activated对象
        //void ParseActivatedClientObjectSection(XmlNode root)
        //{
        //    //
        //}

        #endregion

        #region Server Config Parse

        ServerConfig si = new ServerConfig();

        /// <summary>
        /// 服务端配置信息
        /// </summary>
        public ServerConfig ServerConfig
        {
            get { return si; }
        }

        //解析服务端配置节
        void ParseServerSection(XmlNode root)
        {
            si.Address = ConfigHelper.GetStringAttribute(root, "address");
            string channelType = ConfigHelper.GetStringAttribute(root, "channelType").ToUpper();
            switch (channelType)
            {
                case "TCP":
                    si.ChannelType = ChannelType.TCP;
                    break;
                case "HTTP":
                    si.ChannelType = ChannelType.HTTP;
                    break;
                case "IPC":
                    si.ChannelType = ChannelType.IPC;
                    break;
                default:
                    si.ChannelType = ChannelType.TCP;
                    break;
            }

            si.Port = ConfigHelper.GetIntAttribute(root, "port", 9999);

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                switch (node.Name)
                {
                    case "wellknown":
                        ParseWellKnownServerObjectSection(node);
                        break;
                    //case "activated":
                    //    ParseActivatedServerObjectSection(node);
                    //    break;
                    default:
                        break;
                }
            }
        }

        //解析WellKnown对象
        void ParseWellKnownServerObjectSection(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment) continue;
                if (node.Name != "add") continue;

                string objectUri = ConfigHelper.GetStringAttribute(node, "objectUri");
                if (string.IsNullOrEmpty(objectUri)) continue;

                string fullTypeName = ConfigHelper.GetStringAttribute(node, "type");
                if (string.IsNullOrEmpty(fullTypeName)) continue;

                string mode = ConfigHelper.GetStringAttribute(node, "mode").ToLower();
                

                string typeName = fullTypeName.Split(',')[0];
                string assemblyName = string.Empty;

                if (fullTypeName.IndexOf(',') != -1)
                {
                    assemblyName = fullTypeName.Split(',')[1];
                }
                else
                {
                    Assembly assembly = Util.FindAssemblyFromAppDirectory(typeName);
                    if (assembly != null)
                        assemblyName = assembly.FullName;
                }

                WellKnownObjectMode objectMode = WellKnownObjectMode.Singleton;
                if (mode == "singlecall")
                    objectMode = WellKnownObjectMode.SingleCall;

                WellKnownServiceTypeEntry wse = new WellKnownServiceTypeEntry(typeName, assemblyName, objectUri, objectMode);
                si.AddWellKnownObject(wse);
            }
        }

        #endregion
    }
}
