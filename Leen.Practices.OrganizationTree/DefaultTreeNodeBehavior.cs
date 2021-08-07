namespace Leen.Practices.Tree
{
    /// <summary>
    /// 默认树节点行为描述。
    /// </summary>
    public class DefaultTreeNodeBehavior : ITreeNodeBehaviorDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public bool CanBehaviorBeInherited => true;

        /// <summary>
        /// 
        /// </summary>
        public bool SelectFirstChildOnExpanded => true;

        /// <summary>
        /// 
        /// </summary>
        public bool CanCheckedBeInherited => true;

        /// <summary>
        /// 
        /// </summary>
        public bool CanCheckedBePropagated => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeCheckable(BaseTreeNode node)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeExpandable(BaseTreeNode node)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool CanNodeSelectable(BaseTreeNode node)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsNodeEnabled(BaseTreeNode node)
        {
            return true;
        }
    }
}
