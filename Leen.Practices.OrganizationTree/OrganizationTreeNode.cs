using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DCCS.Client.Infrastructure.OrganizationTree
{
    /// <summary>
    /// 定义组织机构和设备树上的组织机构节点。
    /// </summary>
    public class OrganizationTreeNode : BaseTreeNode
    {
        /// <summary>
        /// 构造<see cref="OrganizationTreeNode"/>的实例。
        /// </summary>
        public OrganizationTreeNode(string organizationId):base(true)
        {
            NodeType = TreeNodeType.Organization;
            OrganizationId = organizationId;
        }

        /// <summary>
        /// 获取组织结构节点Id。
        /// </summary>
        public string OrganizationId { get; private set; }

        /// <summary>
        /// 加载子节点。
        /// </summary>
        /// <returns></returns>
        protected override async Task<IEnumerable<BaseTreeNode>> LoadChildrenAsync()
        {
            return await Task.Run(() =>
            {
                List<BaseTreeNode> children = new List<BaseTreeNode>();

                var organizationDataService = ServiceLocator.Current.GetInstance<IOrganizationDataService>();
                var organizations = organizationDataService.GetOrganizations(OrganizationId);
                if (organizations != null)
                {
                    foreach (var organization in organizations)
                    {
                        var node = new OrganizationTreeNode(organization.Id) { Checkable=true, NodeName = organization.Name, IsChecked = IsChecked };
                        children.Add(node);
                    }
                }

                var deviceDataService = ServiceLocator.Current.GetInstance<IDeviceDataService>();
                var devices = deviceDataService.GetDevices(OrganizationId);
                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        var node = new DeviceTreeNode(device.Id) { Checkable = true, NodeName = device.Name, IsChecked = IsChecked };
                        children.Add(node);
                    }
                }

                return children;
            });
        }
    }
}
