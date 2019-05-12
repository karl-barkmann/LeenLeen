namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 定义一个通用协定表示此对象可用于在进行选择或取消选择。
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// 获取或设置一个值只是该对象是否已选择。
        /// </summary>
        bool IsSelected { get; set; }
    }
}
