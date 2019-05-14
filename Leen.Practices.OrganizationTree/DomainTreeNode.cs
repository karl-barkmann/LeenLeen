using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCCS.Client.Infrastructure.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的域节点。
    /// </summary>
    public class DomainTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造 <see cref="DomainTreeNode"/> 的实例。
        /// </summary>
        public DomainTreeNode() : base(true)
        {
            NodeType = TreeNodeType.Domain;
        }
    }
}
