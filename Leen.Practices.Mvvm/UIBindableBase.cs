using System.Runtime.CompilerServices;

namespace Leen.Practices.Mvvm
{
    /// <summary>
    /// 定义支持界面视图项绑定基类。
    /// </summary>
    public class UIBindableBase : BindableBase
    {
        private byte _internalStateFlags = 0x00;
        private const byte IsCheckedMask = 0x01;
        private const byte IsSelectedMask = 0x02;
        private const byte EnabledMask = 0x08;

        private byte _internalBehaviorFlags = 0x00;
        private const byte SelectableMask = 0x01;
        private const byte CheckableMask = 0x02;

        /// <summary>
        /// 获取或设置一个值，指示该视图项是否支持选中。
        /// </summary>
        public bool Selectable
        {
            get { return GetInternalBehaviorFlag(SelectableMask); }
            set
            {
                SetInternalBehaviorFlag(value, SelectableMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该视图项是否支持勾选。
        /// </summary>
        public bool Checkable
        {
            get { return GetInternalBehaviorFlag(CheckableMask); }
            set
            {
                SetInternalBehaviorFlag(value, CheckableMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该视图项是否启用。
        /// </summary>
        public bool IsEnabled
        {
            get { return GetInternalStateFlag(EnabledMask); }
            set
            {
                SetInternalStateFlag(value, EnabledMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该视图项是否被选中。
        /// </summary>
        public bool IsSelected
        {
            get { return GetInternalStateFlag(IsSelectedMask); }
            set
            {
                if (!IsEnabled || !Selectable)
                    return;
                SetInternalStateFlag(value, IsSelectedMask);
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示该视图项是否被勾选。
        /// </summary>
        public bool? IsChecked
        {
            get { return GetInternalStateFlag(IsCheckedMask); }
            set
            {
                if (!IsEnabled || !Checkable)
                    return;
                SetInternalStateFlag(value, IsCheckedMask);
            }
        }

        private bool GetInternalBehaviorFlag(byte mask)
        {
            return (_internalBehaviorFlags & mask) == mask;
        }

        private bool SetInternalBehaviorFlag(bool? value, byte mask, [CallerMemberName] string propertyName = null)
        {
            if (GetInternalBehaviorFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _internalBehaviorFlags |= mask;
            }
            else
            {
                _internalBehaviorFlags ^= mask;
            }

            if (propertyName != null)
                RaisePropertyChanged(propertyName);

            return true;
        }

        private bool GetInternalStateFlag(byte mask)
        {
            return (_internalStateFlags & mask) == mask;
        }

        private bool SetInternalStateFlag(bool? value, byte mask, [CallerMemberName] string propertyName = null)
        {
            if (GetInternalStateFlag(mask) == value)
            {
                return false;
            }

            if (value.HasValue && value.Value)
            {
                _internalStateFlags |= mask;
            }
            else
            {
                _internalStateFlags ^= mask;
            }

            if (propertyName != null)
                RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
