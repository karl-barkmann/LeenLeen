using Leen.Practices.Mvvm;
using Leen.Practices.Tree;
using System.Collections.Generic;
using System.Windows;

namespace Demo.Windows.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var nodes = new List<BaseTreeNode>();
            var grandchildren = new List<BaseTreeNode>()
            {
                new OrganizationNode("5"){ NodeName="Node5" },
                new OrganizationNode("6"){ NodeName="Node6" },
                new OrganizationNode("7"){ NodeName="Node7" },
            };
            var children = new List<BaseTreeNode>()
            {
                new OrganizationNode("2", grandchildren){ NodeName="Node2" },
                new OrganizationNode("3"){ NodeName="Node3" },
                new OrganizationNode("4"){ NodeName="Node4" },
            };
            var behavior = new TreetopNodeBehavior();
            grandchildren.ForEach(x => x.SetBehavior(behavior));
            children[1].SetBehavior(behavior);
            children[2].SetBehavior(behavior);
            nodes.Add(new OrganizationNode("1", children) { NodeName = "Node1" });
            treeSelect.ItemsSource = nodes;
        }

        public FrameworkElement ActualView => this;
    }
}
