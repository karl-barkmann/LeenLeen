using System;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 
    /// </summary>
    public class TreeBehaviorDescriptor : ITreeBehaviorDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        private TreeBehaviorDescriptor(bool checkable, bool selectable, bool isExpanded)
        {
            Checkable = checkable;
            Selectable = selectable;
            IsExpanded = isExpanded;
        }

        /// <summary>
        /// 
        /// </summary>
        public Func<BaseTreeNode, bool> NodeSelectableFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Checkable { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool Selectable { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsExpanded { get; }

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
        /// <param name="nodeSelectableFilter"></param>
        /// <param name="checkable"></param>
        /// <param name="selectable"></param>
        /// <param name="isExpanded"></param>
        /// <returns></returns>
        public static ITreeBehaviorDescriptor CreateBehavior(bool checkable, bool selectable, bool isExpanded, Func<BaseTreeNode, bool> nodeSelectableFilter = null)
        {
            return new TreeBehaviorDescriptor(checkable, selectable, isExpanded) 
            { 
                NodeSelectableFilter = nodeSelectableFilter 
            };
        }
    }
}
