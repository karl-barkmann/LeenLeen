namespace Leen.Windows.Interaction
{
    /// <summary>
    /// 定义交互接口相关组件。
    /// <para>使用此接口为需要使用交互接口的相关组件注入交互接口。</para>
    /// </summary>
    public interface IUIInteractionAssociate
    {
        /// <summary>
        /// 获取或设置当前交互接口。
        /// </summary>
        IUIInteractionService UIInteraction { get; set; }
    }
}
