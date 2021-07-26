namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义树形关联视图模型接口。
    /// </summary>
    public interface ITreeAssociatedViewModel
    {
        /// <summary>
        /// 通知树节点正在被选中。
        /// </summary>
        /// <param name="selectingNode">正在选中的节点。</param>
        /// <returns></returns>
        bool NotifySelecting(BaseTreeNode selectingNode);

        /// <summary>
        /// 通知树节点已被选中。
        /// </summary>
        /// <param name="selectedNode">被选中的节点。</param>
        void NotifySelected(BaseTreeNode selectedNode);
    }
}
