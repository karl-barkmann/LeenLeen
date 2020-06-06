using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/> to simplify models.
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
            if (!(propertyExpression.Body is MemberExpression expression))
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
        /// <param name="propertyName">属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            OnRaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 通知属性值正在更改。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanging([CallerMemberName]string propertyName = null)
        {
            OnRaisePropertyChanged(propertyName);
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
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (!Object.Equals(storage, value))
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

        #region private helpers

        /// <summary>
        /// 引发属性已变更事件。
        /// </summary>
        /// <param name="propertyName">变更的属性名称。</param>
        private void OnRaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("'propertyName' can not be null or empty", "propertyName");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 引发属性正在变更事件。
        /// </summary>
        /// <param name="propertyName">正在变更的属性名称。</param>
        private void OnRaisePropertyChanging(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("'propertyName' can not be null or empty", "propertyName");
            }

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion
    }
}
