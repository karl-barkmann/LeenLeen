using System;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 树末梢节点行为。
    /// </summary>
    public class TreetopNodeBehavior : ITreeNodeBehaviorDescriptor
    {
        /// <summary>
        /// 获取一个值指示改行为是否可以被继承。
        /// </summary>
        public bool CanBehaviorBeInherited => false;

        /// <summary>
        /// 获取一个值指示节点展开时是否选中第一个子节点。
        /// </summary>
        public bool SelectFirstChildOnExpanded => false;

        /// <summary>
        /// 获取一个值指示节点勾选时是否同时勾选其子节点。
        /// </summary>
        public bool CanCheckedBeInherited => false;

        /// <summary>
        /// 获取一个值指示节点是否支持单选。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeCheckable(BaseTreeNode node)
        {
            return true;
        }

        /// <summary>
        /// 获取一个值指示节点是否支持展开。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeExpandable(BaseTreeNode node)
        {
            return false;
        }

        /// <summary>
        /// 获取一个值指示节点是否支持选中。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeSelectable(BaseTreeNode node)
        {
            return true;
        }

        /// <summary>
        /// 获取一个值指示节点是否支持启用。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsNodeEnabled(BaseTreeNode node)
        {
            return true;
        }
    }
}
