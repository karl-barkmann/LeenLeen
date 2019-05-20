using Leen.Common;
using System;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的设备节点。
    /// </summary>
    public class DeviceTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造 <see cref="DeviceTreeNode"/> 的实例。
        /// </summary>
        public DeviceTreeNode(string deviceId) : base(deviceId, false)
        {
            NodeType = TreeNodeType.Device;
        }

        /// <summary>
        /// 构造 <see cref="DeviceTreeNode"/> 的实例。
        /// </summary>
        /// <param name="entity">此设备节点对应的设备实体。</param>
        public DeviceTreeNode(INamedDataEntity entity) : this(entity.Id)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            NodeName = entity.Name;
        }

        /// <summary>
        /// 获取节点实体对象。
        /// </summary>
        public INamedDataEntity Entity { get; }
    }
}
