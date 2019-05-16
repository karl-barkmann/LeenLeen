
namespace  Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的设备节点。
    /// </summary>
    public class DeviceTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造 <see cref="DeviceTreeNode"/> 的实例。
        /// </summary>
        public DeviceTreeNode(string deviceId) : base(false)
        {
            DeviceId = deviceId;
            NodeType = TreeNodeType.Device;
        }

        /// <summary>
        /// 获取设备Id。
        /// </summary>
        public string DeviceId { get; private set; }
    }
}
