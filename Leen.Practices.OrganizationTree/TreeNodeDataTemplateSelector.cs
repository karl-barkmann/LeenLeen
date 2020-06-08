using System.Windows;
using System.Windows.Controls;

namespace Leen.Practices.OrganizationTree
{
    /// <summary>
    /// 提供一种方式来根据树节点对象的具体类型和数据绑定元素选择树节点的数据模板。
    /// </summary>
    public class TreeNodeDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// 获取或设置自定义节点数据模板。
        /// </summary>
        public DataTemplate DefaultTemplate { get; set; }

        /// <summary>
        /// 获取或设置组织结构节点数据模板。
        /// </summary>
        public DataTemplate OrganizationTemplate { get; set; }

        /// <summary>
        /// 获取或设置设备节点数据模板。
        /// </summary>
        public DataTemplate DeviceTemplate { get; set; }

        /// <summary>
        /// 获取或设置自定义节点数据模板。
        /// </summary>
        public DataTemplate PlaceHolderTemplate { get; set; }

        /// <summary>
        /// 在派生类中重写时，基于自定义逻辑返回 System.Windows.DataTemplate。
        /// </summary>
        /// <param name="item"> 要为其选择模板的数据对象。</param>
        /// <param name="container">数据绑定对象。</param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            BaseTreeNode treeNode = item as BaseTreeNode;
            if (treeNode != null)
            {
                switch (treeNode.NodeType)
                {
                    case TreeNodeType.Device:
                        return DeviceTemplate;
                    case TreeNodeType.Organization:
                        return OrganizationTemplate;
                    case TreeNodeType.PlaceHolder:
                        return PlaceHolderTemplate;
                    default:
                        return DefaultTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
