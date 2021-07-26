using System;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义描述树节点行为的接口。
    /// </summary>
    public interface ITreeNodeBehaviorDescriptor
    {
        /// <summary>
        /// 获取一个值指示改行为是否可以被继承。
        /// </summary>
        bool CanBehaviorBeInherited { get; }

        /// <summary>
        /// 获取一个值指示节点展开时是否选中第一个子节点。
        /// </summary>
        bool SelectFirstChildOnExpanded { get; }

        /// <summary>
        /// 获取一个值指示节点勾选时是否同时勾选其子节点。
        /// </summary>
        bool CanCheckedBeInherited { get; }

        /// <summary>
        /// 获取一个值指示节点是否支持选中。
        /// </summary>
        /// <param name="node">指定的节点。</param>
        /// <returns></returns>
        bool CanNodeSelectable(BaseTreeNode node);

        /// <summary>
        /// 获取一个值指示节点是否支持单选。
        /// </summary>
        /// <param name="node">指定的节点。</param>
        /// <returns></returns>
        bool CanNodeCheckable(BaseTreeNode node);

        /// <summary>
        /// 获取一个值指示节点是否支持启用。
        /// </summary>
        /// <param name="node">指定的节点。</param>
        /// <returns></returns>
        bool IsNodeEnabled(BaseTreeNode node);

        /// <summary>
        /// 获取一个值指示节点是否支持展开。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool CanNodeExpandable(BaseTreeNode node);
    }
}
