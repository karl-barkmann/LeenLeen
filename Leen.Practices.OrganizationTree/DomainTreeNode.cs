namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的域节点。
    /// </summary>
    public class DomainTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造 <see cref="DomainTreeNode"/> 的实例。
        /// </summary>
        public DomainTreeNode() : base(string.Empty, true)
        {
            NodeType = TreeNodeType.Domain;
        }
    }
}
