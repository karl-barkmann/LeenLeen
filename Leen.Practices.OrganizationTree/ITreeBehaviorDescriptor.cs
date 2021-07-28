using System;

namespace Leen.Practices.Tree
{
    /// <summary>
    /// 定义描述树行为的接口。
    /// </summary>
    public interface ITreeBehaviorDescriptor
    {
        /// <summary>
        /// 获取一个值指示结构树是否支持勾选。
        /// </summary>
        bool Checkable { get; }

        /// <summary>
        /// 获取一个值指示结构树是否支持选中。
        /// </summary>
        bool Selectable { get; }

        /// <summary>
        /// 获取一个值指示结构树默认展开树节点。
        /// </summary>
        bool IsExpanded { get; }

        /// <summary>
        /// 获取结构树对树节点是否支持选中的过滤回调方法。
        /// </summary>
        bool CanNodeSelectable(BaseTreeNode node);
    }
}
