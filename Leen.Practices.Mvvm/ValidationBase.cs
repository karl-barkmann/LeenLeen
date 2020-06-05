using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// Implementation of <see cref="INotifyDataErrorInfo"/> to simplify models.
    /// </summary>
    public class ValidationBase : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        private readonly object _lock = new object();

        /// <summary>
        /// 当验证错误针对属性或整个实体更改时发生。
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 获取一个值，该值指示实体是否包含验证错误。
        /// </summary>
        public bool HasErrors { get { return _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0); } }

        /// <summary>
        /// 获取一个值，该值指示实体是否已通过验证。
        /// </summary>
        public bool IsValid { get { return this.HasErrors; } }

        /// <summary>
        /// 获取针对指定属性或整个实体的验证错误。
        /// </summary>
        /// <param name="propertyName">要检索验证错误的属性的名称；若检索实体级别错误，则为 null 或 <see cref="string.Empty"/>。</param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) && _errors[propertyName].Count > 0)
                    return _errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return _errors.SelectMany(err => err.Value.ToList());
        }

        /// <summary>
        /// 使用特定值，验证指定属性名称的属性。
        /// </summary>
        /// <param name="value">特定属性值。</param>
        /// <param name="propertyName">属性名称。</param>
        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (_errors.ContainsKey(propertyName))
                    _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        /// <summary>
        /// 验证整个实体。
        /// </summary>
        public void Validate()
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                var propNames = _errors.Keys.ToList();
                _errors.Clear();
                propNames.ForEach(pn => OnErrorsChanged(pn));
                HandleValidationResults(validationResults);
            }
        }

        /// <summary>
        /// 当实体或属性验证错误变化时调用。
        /// </summary>
        /// <param name="propertyName">属性名称；若检索实体级别错误，则为 null 或 <see cref="string.Empty"/>。</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            //Group validation results by property names  
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname into g
                                     select g;
            //add _errors to dictionary and inform binding engine about _errors  
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }
        }
    }
}
