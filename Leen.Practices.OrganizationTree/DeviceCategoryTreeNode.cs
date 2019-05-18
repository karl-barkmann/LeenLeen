
namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的设备分类节点。
    /// </summary>
    public class DeviceCategoryTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造 <see cref="DeviceCategoryTreeNode"/> 的实例。
        /// </summary>
        /// <param name="deviceType"></param>
        public DeviceCategoryTreeNode(int deviceType) : base(deviceType.ToString(), true)
        {
            Value = deviceType;
            NodeType = TreeNodeType.DeviceCategory;
        }

        /// <summary>
        /// 获取设备分类代表的值。
        /// </summary>
        public int Value { get; private set; }
    }
}
