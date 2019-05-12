using CommonServiceLocator;
using System;

namespace Leen.Common
{
    /// <summary>
    /// <see cref="ServiceLocator"/> 扩展类, 提供一组帮助方法。
    /// </summary>
    public static class ServiceLocatorExtensions
    {
        /// <summary>
        /// 获取一个值指示 <typeparamref name="T"/> 类型是否已注册到服务容器中。
        /// </summary>
        /// <typeparam name="T">需要判断的注册类型。</typeparam>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static bool IsRegisterd<T>(this IServiceLocator locator)
        {
            try
            {
                if (locator.GetInstance<T>() != null)
                {
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ActivationException)
            {
                return false;
            }

            return false;
        }
    }
}
