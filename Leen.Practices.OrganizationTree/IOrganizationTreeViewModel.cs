using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace DCCS.Client.Infrastructure.OrganizationTree
{
    /// <summary>
    /// 定义组织结构及设备树的视图模型。
    /// </summary>
    public interface IOrganizationTreeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        IList<BaseTreeNode> Nodes { get; }

        /// <summary>
        /// 获取已勾选的设备Id集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetCheckedDeviceIds();

        /// <summary>
        /// 获取已勾选的组织机构Id集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetCheckedOrganizationIds();

        /// <summary>
        /// 获取指定Id的组织机构树节点。
        /// </summary>
        /// <param name="organizationId">组织机构Id。</param>
        /// <returns></returns>
        OrganizationTreeNode FindOrganizationNode(int organizationId);

        /// <summary>
        /// 获取指定Id的设备树节点。
        /// </summary>
        /// <param name="deviceId">设备Id。</param>
        /// <returns></returns>
        DeviceTreeNode FindDeviceNode(int deviceId);
    }
}
