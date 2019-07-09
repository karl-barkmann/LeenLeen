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
        public OrganizationTreeNode(string organizationId) : base(organizationId, true)
        {
            NodeType = TreeNodeType.Organization;
        }

        /// <summary>
        /// 构造<see cref="OrganizationTreeNode"/>的实例。
        /// </summary>
        /// <param name="entity">此组织结构节点对应的组织机构实体。</param>
        public OrganizationTreeNode(INamedDataEntity entity) : this(entity.Id)
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
            var organizationDataService = ServiceLocator.Current.GetInstance<IOrganizationDataProvider>();
            var organizations = await organizationDataService.GetOrganizations(NodeId);
            if (organizations != null)
            {
                foreach (var organization in organizations)
                {
                    var node = new OrganizationTreeNode(organization)
                    {
                        Checkable = Checkable,
                        Selectable = Selectable,
                        NodeName = organization.Name,
                        IsChecked = IsChecked
                    };
                    children.Add(node);
                }
            }

            var deviceDataService = ServiceLocator.Current.GetInstance<IDeviceDataProvider>();
            var devices = await deviceDataService.GetDevices(NodeId);
            if (devices != null)
            {
                foreach (var device in devices)
                {
                    var node = new DeviceTreeNode(device)
                    {
                        Checkable = Checkable,
                        Selectable = Selectable,
                        NodeName = device.Name,
                        IsChecked = IsChecked
                    };
                    children.Add(node);
                }
            }

            return children;
        }
    }
}
