using Leen.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义提供组织机构树形数据结构实体的数据接口。
    /// </summary>
    public interface ITreeNodeDataProvider
    {
        /// <summary>
        /// 获取树形数据结构的所有节点数据实体。
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetNodes();

        /// <summary>
        /// 获取树形数据结构的所有根节点数据实体。
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetRootNodes();

        /// <summary>
        /// 获取指定标识的节点数据实体。
        /// </summary>
        /// <param name="id">节点标识。</param>
        /// <returns></returns>
        Task<INamedDataEntity> GetNode(string id);

        /// <summary>
        /// 获取指定节点下的直属节点实体集合。
        /// </summary>
        /// <param name="parent">父节点。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetNodes(INamedDataEntity parent);

        /// <summary>
        /// 递归获取指定节点下的所有节点实体集合。
        /// </summary>
        /// <param name="parent">父节点。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedDataEntity>> GetNodesRecursive(INamedDataEntity parent);
    }
}
