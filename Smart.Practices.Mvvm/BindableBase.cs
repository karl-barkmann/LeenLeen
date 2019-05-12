using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// Implementation of System.ComponentModel.INotifyPropertyChanged and System.ComponentModel.INotifyPropertyChanging to simplify models.
    /// </summary>
    [Serializable]
    public class BindableBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// 构造BindableBase的实例。
        /// </summary>
        protected BindableBase()
        {

        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property value changes.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Provides support for extracting property information based on a property expression. 
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        /// <returns>The property name.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"),
        SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            MemberExpression expression = propertyExpression.Body as MemberExpression;
            if (expression == null)
            {
                throw new ArgumentException("expression is not a member access expression.", "propertyExpression");
            }
            PropertyInfo info = expression.Member as PropertyInfo;
            if (info == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            //For some reason,developers may delcare a property as private.
            var method = info.GetGetMethod(true);
            if (method.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }
            return expression.Member.Name;
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <param name="propertyName">默认为自动解析属性名称，亦可指定属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [Obsolete("不要再使用此方法，至少在我们将目标框架更改为4.5之前。此方法是依赖调用堆栈来获取属性名称，然而依赖调用堆栈是不可靠的，因为在代码优化后某些堆栈将不可见。")]
        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                propertyName = ExtractCallMemberName();
            }
            OnRaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <param name="propertyName">可指定属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChangedWith(string propertyName)
        {
            OnRaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 通知属性值正在更改。
        /// </summary>
        /// <param name="propertyName">默认为自动解析属性名称，亦可指定属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [Obsolete("不要再使用此方法，至少在我们将目标框架更改为4.5之前。此方法是依赖调用堆栈来获取属性名称，然而依赖调用堆栈是不可靠的，因为在代码优化后某些堆栈将不可见。")]
        protected virtual void RaisePropertyChanging(string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                propertyName = ExtractCallMemberName();
            }
            OnRaisePropertyChanging(propertyName);
        }

        /// <summary>
        /// 通知属性值正在更改。
        /// </summary>
        /// <param name="propertyName">指定属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChangingWith(string propertyName)
        {
            OnRaisePropertyChanging(propertyName);
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <typeparam name="T">描述属性的类型。</typeparam>
        /// <param name="propertyExpression">用户获取属性名称的表达式。</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"),
        SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = ExtractPropertyName(propertyExpression);
            OnRaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 通知属性值正在更改。
        /// </summary>
        /// <typeparam name="T">描述属性的类型。</typeparam>
        /// <param name="propertyExpression">用户获取属性名称的表达式。</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"),
        SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression)
        {
            string propertyName = ExtractPropertyName(propertyExpression);

            OnRaisePropertyChanging(propertyName);
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This value is optional and
        ///     can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        /// <returns> True if the value was changed, false if the existing value matched the desired value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        [Obsolete("不要再使用此方法，至少在我们将目标框架更改为4.5之前。此方法是依赖调用堆栈来获取属性名称，然而依赖调用堆栈是不可靠的，因为在代码优化后某些堆栈将不可见。")]
        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            if (!Object.Equals(storage, value))
            {
                propertyName = ExtractCallMemberName();

                OnRaisePropertyChanging(propertyName);
                storage = value;
                OnRaisePropertyChanged(propertyName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        /// <returns> True if the value was changed, false if the existing value matched the desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> propertyExpression)
        {
            if (!Equals(storage, value))
            {
                RaisePropertyChanging(propertyExpression);
                storage = value;
                RaisePropertyChanged(propertyExpression);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. </param>
        /// <returns> True if the value was changed, false if the existing value matched the desired value.</returns>
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
        protected virtual bool SetPropertyWith<T>(ref T storage, T value, string propertyName)
        {
            if (!Equals(storage, value))
            {
                OnRaisePropertyChanging(propertyName);
                storage = value;
                OnRaisePropertyChanged(propertyName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 提取属性名称。
        /// </summary>
        /// <returns></returns>
        [Obsolete("不要使用此方法，此方法是依赖调用堆栈来获取属性名称。然而依赖调用堆栈是不可靠的，因为在代码优化后某些堆栈是不可见的。")]
        protected string ExtractCallMemberName()
        {
            StackFrame frame = new StackFrame(2);
            string name = frame.GetMethod().Name;
            if (name.Contains("set_"))
            {
                name = name.Replace("set_", "");
            }
            return name;
        }

        #region private helpers

        /// <summary>
        /// 引发属性已变更事件。
        /// </summary>
        /// <param name="propertyName">变更的属性名称。</param>
        protected virtual void OnRaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("'propertyName' can not be null or empty", "propertyName");
            }
            var handler = PropertyChanged;
            
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 引发属性正在变更事件。
        /// </summary>
        /// <param name="propertyName">正在变更的属性名称。</param>
        protected virtual void OnRaisePropertyChanging(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("'propertyName' can not be null or empty", "propertyName");
            }

            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
