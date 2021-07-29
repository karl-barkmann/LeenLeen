using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义组织结构及设备树的视图模型。
    /// </summary>
    public interface ITreeViewModel
    {
        /// <summary>
        /// 获取组织机构的根节点集合。
        /// </summary>
        IList<BaseTreeNode> Nodes { get; }

        /// <summary>
        /// 获取已勾选的节点集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGrouping<TreeNodeType, BaseTreeNode>> GetCheckedNodes();

        /// <summary>
        /// 获取已选中的节点集合。
        /// </summary>
        /// <returns></returns>
        IEnumerable<BaseTreeNode> GetSelectedNodes();

        /// <summary>
        /// 获取指定Id的树节点。
        /// </summary>
        /// <param name="nodeId">树节点Id。</param>
        /// <returns></returns>
        Task<BaseTreeNode> GetNodeAggressivelyAsync(string nodeId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="association"></param>
        void AddAssociation(ITreeAssociatedViewModel association);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assoication"></param>
        void RemoveAssociation(ITreeAssociatedViewModel assoication);
    }
}
