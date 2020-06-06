namespace Leen.Practices.OrganizationTree
{
    class AllEnabledNodeBehavior : ITreeNodeBehavior
    {
        public bool CanBehaviorBeInherited => true;

        public bool SelectFirstChildOnExpanded => true;

        public bool CanCheckedBeInherited => true;

        public bool CanNodeCheckable(BaseTreeNode node)
        {
            return true;
        }

        public bool CanNodeExpandable(BaseTreeNode node)
        {
            return true;
        }

        public bool CanNodeSelectable(BaseTreeNode node)
        {
            return true;
        }

        public bool IsNodeEnabled(BaseTreeNode node)
        {
            return true;
        }
    }
}
