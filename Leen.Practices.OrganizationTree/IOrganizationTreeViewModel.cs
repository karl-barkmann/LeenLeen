using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<BaseTreeNode> FindNodeAsync(string nodeId);

        /// <summary>
        /// 获取指定Id的组织机构树节点。
        /// </summary>
        /// <param name="organizationId">组织机构Id。</param>
        /// <returns></returns>
        Task<OrganizationTreeNode> FindOrganizationNodeAsync(string organizationId);
    }
}
