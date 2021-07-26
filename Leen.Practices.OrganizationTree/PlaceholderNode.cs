namespace Leen.Practices.Tree
{
    internal class PlaceholderNode : BaseTreeNode
    {
        public PlaceholderNode() : this("-1")
        {
        }

        public PlaceholderNode(string nodeId) : base(nodeId, TreeNodeType.PlaceHolder)
        {
            Selectable = false;
            Checkable = false;
            Expandable = false;
            NodeName = "加载中...";
        }
    }
}
