using Leen.Common.Utils;
using System;
using System.Collections.Generic;
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
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var properties = new List<string>();
            Expression expression = propertyExpression.Body;
            do
            {
                if (expression is MemberExpression memberExpression && memberExpression.Member is PropertyInfo property)
                {
                    expression = memberExpression.Expression;
                    var method = property.GetGetMethod(true);
                    if (method.IsStatic)
                    {
                        throw new ArgumentException("The referenced property is a static property.", nameof(propertyExpression));
                    }
                    properties.Add(memberExpression.Member.Name);
                }
                else
                {
                    break;
                }
            } while (expression != null);

            if(properties.Count<1)
            {
                throw new ArgumentException("The expression does not access a property.", nameof(propertyExpression));
            }

            properties.Reverse();
            return string.Join(".", properties);
        }

        /// <summary>
        /// Watch on a property changed.
        /// </summary>
        /// <typeparam name="T">Property type.</typeparam>
        /// <param name="propertyExpression">Property name expression.</param>
        /// <param name="callback">Callback Lambda when the property changed. </param>
        /// <param name="deepWatch">Where Watch property deeply if a property is a refernce value.</param>
        /// <returns></returns>
        public IDisposable Watch<T>(Expression<Func<T>> propertyExpression, Action<T, T> callback, bool deepWatch)
        {
            if (propertyExpression is null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            var watcher = new PropertyWatcher<T>(this, propertyExpression, callback, deepWatch);
            return watcher;
        }

        /// <summary>
        /// Watch on a property changed.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="callback">Callback Lambda when the property changed. </param>
        /// <param name="deepWatch">Where Watch property deeply if a property is a refernce value.</param>
        /// <returns></returns>
        private protected IDisposable Watch(Type propertyType, string propertyName, Action callback, bool deepWatch)
        {
            var watcher = new PropertyWatcher(this, propertyType, propertyName, callback, deepWatch);
            return watcher;
        }

        /// <summary>
        /// Watch on a property changed.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="callback">Callback Lambda when the property changed. </param>
        private protected IDisposable Watch<T>(string propertyName, Action<T, T> callback)
        {
            var watcher = new PropertyWatcher<T>(this, propertyName, callback);
            return watcher;
        }

        /// <summary>
        /// Watch on a property changed.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="callback">Callback Lambda when the property changed. </param>
        private protected IDisposable Watch(Type propertyType, string propertyName, Action<object, object> callback)
        {
            var watcher = new PropertyWatcher(this, propertyType, propertyName, callback);
            return watcher;
        }

        /// <summary>
        /// 通知属性值已更改。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnRaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// 通知属性值正在更改。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
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
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
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
        /// 引发属性已变更事件。
        /// </summary>
        /// <param name="propertyName">变更的属性名称。</param>
        protected void OnRaisePropertyChanged(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"{nameof(propertyName)} can not be null or empty", nameof(propertyName));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 引发属性正在变更事件。
        /// </summary>
        /// <param name="propertyName">正在变更的属性名称。</param>
        protected void OnRaisePropertyChanging(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"{nameof(propertyName)}  can not be null or empty", nameof(propertyName));
            }

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        private class PropertyWatcher<T> : IDisposable
        {
            private T oldVal;
            private T newVal;
            private readonly List<IDisposable> _deepWatchers;

            public PropertyWatcher(BindableBase target, Expression<Func<T>> propertyExpression, Action<T, T> callback, bool deepWatch)
            {
                Target = target;
                PropertyName = ExtractPropertyName(propertyExpression);
                Callback = callback;
                DeepWatch = deepWatch;
                DirectlyWatch = PropertyName.IndexOf('.') > 0;

                if (deepWatch && !DirectlyWatch)
                {
                    var propertyValue = ReflectionHelper.GetPropValue<T>(target, PropertyName);
                    if (propertyValue is BindableBase nestTarget)
                    {
                        if (_deepWatchers == null)
                            _deepWatchers = new List<IDisposable>();
                        var properties = nestTarget.GetType().GetProperties(BindingFlags.Public
                                                                            | BindingFlags.NonPublic
                                                                            | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            var watcher = nestTarget.Watch(property.PropertyType, property.Name, () =>
                            {
                                callback((T)propertyValue, (T)propertyValue);
                            }, deepWatch);
                            _deepWatchers.Add(watcher);
                        }
                    }
                }

                if (DirectlyWatch)
                {
                    var lastPropValue = ReflectionHelper.GetPropValue<T>(target, PropertyName);
                    if (lastPropValue is BindableBase lastTarget)
                    {
                        if (_deepWatchers == null)
                            _deepWatchers = new List<IDisposable>();
                        var properties = lastTarget.GetType().GetProperties(BindingFlags.Public
                                                                       | BindingFlags.NonPublic
                                                                       | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            var watcher = lastTarget.Watch(property.PropertyType, property.Name, () =>
                            {
                                callback((T)lastPropValue, (T)lastPropValue);
                            }, deepWatch);
                            _deepWatchers.Add(watcher);
                        }
                    }
                    else
                    {
                        var preLastPropValue = ReflectionHelper.GetPropValue(target, PropertyName.Substring(0, PropertyName.LastIndexOf('.')));
                        if (preLastPropValue is BindableBase preTarget)
                        {
                            if (_deepWatchers == null)
                                _deepWatchers = new List<IDisposable>();

                            var propertyName = PropertyName.Substring(PropertyName.LastIndexOf('.') + 1);
                            var watcher = preTarget.Watch(propertyName, callback);
                            _deepWatchers.Add(watcher);
                        }
                    }
                }
                Target.PropertyChanged += OnTargetPropertyChanged;
                Target.PropertyChanging += OnTargetPropertyChanging;
            }

            public PropertyWatcher(BindableBase target, string propertyName, Action<T, T> callback)
            {
                Target = target;
                PropertyName = propertyName;
                Callback = callback;
                Target.PropertyChanged += OnTargetPropertyChanged;
                Target.PropertyChanging += OnTargetPropertyChanging;
            }

            public bool IsDisposed { get; private set; }

            private protected BindableBase Target { get; }

            private protected string PropertyName { get; }

            private protected Action<T, T> Callback { get; }

            private protected bool DeepWatch { get; }

            public bool DirectlyWatch { get; }

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    if (_deepWatchers != null)
                    {
                        _deepWatchers.ForEach(x => x.Dispose());
                    }
                    _deepWatchers.Clear();
                    Target.PropertyChanged -= OnTargetPropertyChanged;
                    Target.PropertyChanging -= OnTargetPropertyChanging;
                    IsDisposed = true;
                    oldVal = default;
                    newVal = default;
                }
            }

            private void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == PropertyName)
                {
                    newVal = ReflectionHelper.GetPropValue<T>(Target, PropertyName);
                    try
                    {
                        Callback(oldVal, newVal);
                    }
                    finally
                    {
                        oldVal = default;
                        newVal = default;
                    }
                }
            }

            private void OnTargetPropertyChanging(object sender, PropertyChangingEventArgs e)
            {
                if (e.PropertyName == PropertyName)
                {
                    oldVal = ReflectionHelper.GetPropValue<T>(Target, PropertyName);
                }
            }
        }

        private class PropertyWatcher : IDisposable
        {
            private readonly List<IDisposable> _deepWatchers;
            private object oldVal;
            private object newVal;

            public PropertyWatcher(BindableBase target, Type propertyType, string propertyName, Action callback, bool deepWatch)
            {
                Target = target;
                PropertyType = propertyType;
                PropertyName = propertyName;
                Callback = callback;
                DeepWatch = deepWatch;
                if (deepWatch)
                {
                    var propertyValue = ReflectionHelper.GetPropValue(target, propertyName, propertyType);
                    if (propertyValue is BindableBase nestTarget)
                    {
                        _deepWatchers = new List<IDisposable>();
                        var properties = nestTarget.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        foreach (var property in properties)
                        {
                            var watcher = nestTarget.Watch(property.PropertyType, property.Name, callback, deepWatch);
                            _deepWatchers.Add(watcher);
                        }
                    }
                }
                Target.PropertyChanged += OnTargetPropertyChanged;
            }

            public PropertyWatcher(BindableBase target, Type propertyType, string propertyName, Action<object,object> callback)
            {
                Target = target;
                PropertyType = propertyType;
                PropertyName = propertyName;
                ValCallback = callback;
                Target.PropertyChanging += OnTargetPropertyChanging;
                Target.PropertyChanged += OnTargetPropertyChanged;
            }

            public bool IsDisposed { get; private set; }

            private protected BindableBase Target { get; }
            public Type PropertyType { get; }
            private protected string PropertyName { get; }

            public Action Callback { get; }

            public Action<object, object> ValCallback { get; }

            public bool DeepWatch { get; }

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    if (_deepWatchers != null)
                    {
                        _deepWatchers.ForEach(x => x.Dispose());
                    }
                    _deepWatchers.Clear();
                    Target.PropertyChanged -= OnTargetPropertyChanged;
                    Target.PropertyChanging -= OnTargetPropertyChanging;
                    IsDisposed = true;
                }
            }

            private void OnTargetPropertyChanging(object sender, PropertyChangingEventArgs e)
            {
                if (e.PropertyName == PropertyName)
                    oldVal = ReflectionHelper.GetPropValue(Target, PropertyName, PropertyType);
            }

            private void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == PropertyName)
                {
                    newVal = ReflectionHelper.GetPropValue(Target, PropertyName, PropertyType);
                    Callback?.Invoke();
                    try
                    {
                        ValCallback?.Invoke(oldVal, newVal);
                    }
                    finally
                    {
                        oldVal = default;
                        newVal = default;
                    }
                }
            }
        }
    }
}
