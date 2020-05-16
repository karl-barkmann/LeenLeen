namespace Leen.Practices.OrganizationTree
{
    internal class PlaceholderNode : BaseTreeNode
    {
        public PlaceholderNode() : base("-1", true)
        {
            NodeType = TreeNodeType.PlaceHolder;
            NodeName = "加载中...";
        }

        public PlaceholderNode(string nodeId, bool initializeWithPlaceholder) : base(nodeId, initializeWithPlaceholder)
        {
            NodeType = TreeNodeType.PlaceHolder;
            NodeName = "加载中...";
        }
    }
}
