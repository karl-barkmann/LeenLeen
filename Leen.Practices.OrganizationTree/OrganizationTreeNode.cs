using CommonServiceLocator;
using Leen.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的组织机构节点。
    /// </summary>
    public class OrganizationTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造<see cref="OrganizationTreeNode"/>的实例。
        /// </summary>
        /// <param name="organizationId">机构节点标识。</param>
        /// <param name="withChildren">节点是否会包含子节点。</param>
        public OrganizationTreeNode(string organizationId, bool withChildren) : base(organizationId, TreeNodeType.Organization, withChildren)
        {
        }

        /// <summary>
        /// 构造<see cref="OrganizationTreeNode"/>的实例。
        /// </summary>
        /// <param name="entity">此组织结构节点对应的组织机构实体。</param>
        /// <param name="withChildren">节点是否会包含子节点。</param>
        public OrganizationTreeNode(INamedDataEntity entity, bool withChildren) : this(entity.Id, withChildren)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            NodeName = entity.Name;
        }

        /// <summary>
        /// 获取节点实体对象。
        /// </summary>
        public INamedDataEntity Entity { get; }

        /// <summary>
        /// 加载子节点。
        /// </summary>
        /// <returns></returns>
        protected override async Task<IEnumerable<BaseTreeNode>> LoadChildrenAsync()
        {
            var children = new List<BaseTreeNode>();
            var nodeDataProvier = ServiceLocator.Current.GetInstance<ITreeNodeDataProvider>();
            var nodeDatas = await nodeDataProvier.GetNodes(Entity);
            if (nodeDatas != null)
            {
                foreach (var nodeData in nodeDatas)
                {
                    var node = new OrganizationTreeNode(nodeData, true)
                    {
                        Checkable = Checkable,
                        Selectable = Selectable,
                        IsChecked = IsChecked
                    };
                    children.Add(node);
                }
            }

            return children;
        }
    }
}
