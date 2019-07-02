using System.Collections.Generic;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织结构及设备树的视图模型。
    /// </summary>
    public interface IOrganizationTreeViewModel
    {
        /// <summary>
        /// 获取组织机构的根节点集合。
        /// </summary>
        IList<BaseTreeNode> Nodes { get; }

        /// <summary>
        /// 获取已勾选的设备Id集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCheckedDeviceIds();

        /// <summary>
        /// 获取已勾选的组织机构Id集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCheckedOrganizationIds();

        /// <summary>
        /// 获取指定Id的树节点。
        /// </summary>
        /// <param name="nodeId">树节点Id。</param>
        /// <returns></returns>
        BaseTreeNode FindNode(string nodeId);

        /// <summary>
        /// 获取指定Id的组织机构树节点。
        /// </summary>
        /// <param name="organizationId">组织机构Id。</param>
        /// <returns></returns>
        OrganizationTreeNode FindOrganizationNode(string organizationId);

        /// <summary>
        /// 获取指定Id的设备树节点。
        /// </summary>
        /// <param name="deviceId">设备Id。</param>
        /// <returns></returns>
        DeviceTreeNode FindDeviceNode(string deviceId);
    }
}
