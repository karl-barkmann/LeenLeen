using CommonServiceLocator;
using Leen.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义组织机构和设备树上的组织机构节点。
    /// </summary>
    public class OrganizationNode : BaseTreeNode
    {
        /// <summary>
        /// 构造<see cref="OrganizationNode"/>的实例。
        /// </summary>
        /// <param name="organizationId">机构节点标识。</param>
        public OrganizationNode(string organizationId) : base(organizationId, TreeNodeType.Organization)
        {
        }

        /// <summary>
        /// 构造<see cref="OrganizationNode"/>的实例。
        /// </summary>
        /// <param name="organizationId">机构节点标识。</param>
        /// <param name="children">子节点集合。</param>
        public OrganizationNode(string organizationId, IEnumerable<BaseTreeNode> children) : base(organizationId, children, TreeNodeType.Organization)
        {
        }

        /// <summary>
        /// 构造<see cref="OrganizationNode"/>的实例。
        /// </summary>
        /// <param name="entity">此组织结构节点对应的组织机构实体。</param>
        public OrganizationNode(INamedCascadeDataEntity entity) : this(entity.Id)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            NodeName = entity.Name;
        }

        /// <summary>
        /// 构造<see cref="OrganizationNode"/>的实例。
        /// </summary>
        /// <param name="entity">此组织结构节点对应的组织机构实体。</param>
        /// <param name="children">子节点集合。</param>
        public OrganizationNode(INamedCascadeDataEntity entity, IEnumerable<BaseTreeNode> children) : this(entity.Id,  children)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
            NodeName = entity.Name;
        }

        /// <summary>
        /// 获取节点实体对象。
        /// </summary>
        public INamedCascadeDataEntity Entity { get; }

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
                    var node = new OrganizationNode(nodeData);
                    children.Add(node);
                }
            }
            return children;
        }
    }
}
