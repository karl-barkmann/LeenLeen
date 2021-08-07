using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 
    /// </summary>
    public class TreeNodeBehaviorDescriptor : ITreeNodeBehaviorDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canCheckedBeInherited"></param>
        /// <param name="canBehaviorBeInherited"></param>
        /// <param name="canCheckedBePropagated"></param>
        /// <param name="selectFirstChildOnExpanded"></param>
        public TreeNodeBehaviorDescriptor(bool canCheckedBeInherited, bool canBehaviorBeInherited, bool canCheckedBePropagated, bool selectFirstChildOnExpanded)
        {
            CanBehaviorBeInherited = canBehaviorBeInherited;
            SelectFirstChildOnExpanded = selectFirstChildOnExpanded;
            CanCheckedBeInherited = canCheckedBeInherited;
            CanCheckedBePropagated = canCheckedBePropagated;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CanBehaviorBeInherited { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool SelectFirstChildOnExpanded { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanCheckedBeInherited { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanCheckedBePropagated { get; }

        /// <summary>
        /// 
        /// </summary>
        public Func<BaseTreeNode,bool> NodeCheckableFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<BaseTreeNode, bool> NodeExpandableFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<BaseTreeNode, bool> NodeSelectableFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<BaseTreeNode, bool> NodeEnabledFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeCheckable(BaseTreeNode node)
        {
            if (NodeCheckableFilter != null)
                return NodeCheckableFilter(node);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeExpandable(BaseTreeNode node)
        {
            if (NodeExpandableFilter != null)
                return NodeExpandableFilter(node);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeSelectable(BaseTreeNode node)
        {
            if (NodeSelectableFilter != null)
                return NodeSelectableFilter(node);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsNodeEnabled(BaseTreeNode node)
        {
            if (NodeEnabledFilter != null)
                return NodeEnabledFilter(node);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canCheckedBeInherited"></param>
        /// <param name="canBehaviorBeInherited"></param>
        /// <param name="canCheckedBePropagated"></param>
        /// <param name="selectFirstChildOnExpanded"></param>
        /// <param name="nodeSelectableFilter"></param>
        /// <param name="nodeCheckableFilter"></param>
        /// <param name="nodeExpandableFilter"></param>
        /// <param name="nodeEnabledFilter"></param>
        /// <returns></returns>
        public static ITreeNodeBehaviorDescriptor CreateBehavior(
            bool canCheckedBeInherited,
            bool canBehaviorBeInherited,
            bool canCheckedBePropagated,
            bool selectFirstChildOnExpanded,
            Func<BaseTreeNode, bool> nodeEnabledFilter,
            Func<BaseTreeNode, bool> nodeCheckableFilter,
            Func<BaseTreeNode, bool> nodeSelectableFilter,
            Func<BaseTreeNode, bool> nodeExpandableFilter)
        {
            return new TreeNodeBehaviorDescriptor(canCheckedBeInherited, canBehaviorBeInherited, canCheckedBePropagated, selectFirstChildOnExpanded)
            {
                NodeEnabledFilter = nodeEnabledFilter,
                NodeCheckableFilter = nodeCheckableFilter,
                NodeSelectableFilter = nodeSelectableFilter,
                NodeExpandableFilter = nodeExpandableFilter,
            };
        }
    }
}
