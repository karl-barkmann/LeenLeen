using Leen.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.Tree
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
        Task<IEnumerable<INamedCascadeDataEntity>> GetNodes();

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
        Task<INamedCascadeDataEntity> GetNode(string id);

        /// <summary>
        /// 获取指定标识的节点的父节点数据实体。
        /// </summary>
        /// <param name="id">节点标识。</param>
        /// <returns></returns>
        Task<INamedCascadeDataEntity> GetParentNode(string id);

        /// <summary>
        /// 获取指定标识节点下的直属节点实体集合。
        /// </summary>
        /// <param name="parentId">父节点标识。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedCascadeDataEntity>> GetNodes(string parentId);

        /// <summary>
        /// 获取指定节点下的直属节点实体集合。
        /// </summary>
        /// <param name="parent">父节点。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedCascadeDataEntity>> GetNodes(INamedCascadeDataEntity parent);

        /// <summary>
        /// 递归获取指定标识节点下的所有节点实体集合。
        /// </summary>
        /// <param name="parentId">父节点标识。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedCascadeDataEntity>> GetNodesRecursive(string parentId);

        /// <summary>
        /// 递归获取指定节点下的所有节点实体集合。
        /// </summary>
        /// <param name="parent">父节点。</param>
        /// <returns></returns>
        Task<IEnumerable<INamedCascadeDataEntity>> GetNodesRecursive(INamedCascadeDataEntity parent);
    }
}
