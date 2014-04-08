using System;

namespace Xunmei.Smart.Common
{
    /// <summary>
    /// 简单切面编程辅助类型。
    /// </summary>
    public class Aspect
    {
        private Action<Action> chain = null;

        /// <summary>
        /// 增加切面定义。
        /// </summary>
        /// <param name="aspect">切面操作。</param>
        /// <returns></returns>
        public Aspect Combine(Action<Action> aspect)
        {
            if (chain == null)
            {
                chain = aspect;
            }
            else
            {
                Action<Action> previous = chain;
                Action<Action> combined = (work) =>
                    previous(() => aspect(work));
                chain = combined;
            }

            return this;
        }

        /// <summary>
        /// 做实际工作。
        /// </summary>
        /// <param name="work">工作内容。</param>
        public void Do(Action work)
        {
            if (chain == null)
            {
                work();
            }
            else
            {
                chain(work);
            }
        }

        /// <summary>
        /// 定义切面。
        /// </summary>
        public static Aspect Define
        {
            get 
            {
                return new Aspect();
            }
        }
    }
}
