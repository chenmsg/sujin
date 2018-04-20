using System;

namespace ITOrm.Core.Remoting
{
    /// <summary>
    /// 通道类型枚举
    /// </summary>
    [Serializable]
    public enum ChannelType
    {
        /// <summary>
        /// Tcp Channel
        /// </summary>
        TCP,
        /// <summary>
        /// Http Channel
        /// </summary>
        HTTP,
        /// <summary>
        /// Ipc Channel
        /// </summary>
        IPC
    }
}
