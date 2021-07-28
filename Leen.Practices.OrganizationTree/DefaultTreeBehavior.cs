namespace Leen.Practices.Tree
{
    /// <summary>
    /// 默认树行为描述。
    /// </summary>
    public class DefaultTreeBehavior : ITreeBehaviorDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Checkable => false;

        /// <summary>
        /// 
        /// </summary>
        public bool Selectable => true;

        /// <summary>
        /// 
        /// </summary>
        public bool IsExpanded => true;

        /// <summary>
        /// 
        /// </summary>
        public bool CanNodeSelectable(BaseTreeNode node)
        {
            return true;
        }
    }
}
