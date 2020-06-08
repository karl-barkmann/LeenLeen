namespace Leen.Practices.OrganizationTree
{
    internal class PlaceholderNode : BaseTreeNode
    {
        public PlaceholderNode() : base("-1", TreeNodeType.PlaceHolder, true)
        {
            NodeName = "加载中...";
        }

        public PlaceholderNode(string nodeId, bool initializeWithPlaceholder) : base(nodeId, TreeNodeType.PlaceHolder, initializeWithPlaceholder)
        {
            NodeName = "加载中...";
        }
    }
}
