using System.ComponentModel;

namespace  Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 组织机构和设备树节点的类型。
    /// </summary>
    public enum TreeNodeType
    {
        /// <summary>
        /// 默认值。
        /// </summary>
        [Description("未知节点")]
        None,

        /// <summary>
        /// 设备节点包含但不限于摄像头通道、探头、报警输出及报警输入等。
        /// </summary>
        [Description("设备节点")]
        Device,

        /// <summary>
        /// 设备分类节点。
        /// </summary>
        [Description("设备分类节点")]
        DeviceCategory,

        /// <summary>
        /// 组织机构节点
        /// </summary>
        [Description("组织机构节点")]
        Organization,

        /// <summary>
        /// 域节点
        /// </summary>
        [Description("域节点")]
        Domain,

        /// <summary>
        /// 任意自定义节点。
        /// </summary>
        [Description("自定义节点")]
        Custom = 0x1010,
    }
}
