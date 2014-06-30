using System;

namespace Smart.Practices.Mvvm
{
    /// <summary>
    /// 窗口及窗口视图模型映射接口。
    /// </summary>
    public interface IWindowViewModelMapping
    {
        /// <summary>
        /// 获取窗口视图模型类型对应的窗口类型。
        /// </summary>
        /// <param name="vidwModelType">窗口的视图模型类型。</param>
        /// <returns>窗口类型。</returns>
        Type GetWindowTypeFromViewModelType(Type vidwModelType);
    }
}
