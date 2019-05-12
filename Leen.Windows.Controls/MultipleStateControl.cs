﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Leen.Windows.Controls
{
    /// <summary>
    /// 根据绑定的状态值显示不同状态界面。
    /// </summary>
    [TemplatePart(Name = "PART_StateIndicator", Type = typeof(Shape))]
    public class MultipleStateControl : Control
    {
        private Shape stateIndicator;

        public override void OnApplyTemplate()
        {
            stateIndicator = GetTemplateChild("PART_StateIndicator") as Shape;
            if (stateIndicator != null)
            {
                stateIndicator.SetBinding(Shape.FillProperty, new Binding("State") { Converter = StateConverter, Source = this });
            }
        }

        static MultipleStateControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (MultipleStateControl),
                new FrameworkPropertyMetadata(typeof (MultipleStateControl)));
        }

        /// <summary>
        /// 状态值。
        /// </summary>
        public int State
        {
            get { return (int) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof (int), typeof (MultipleStateControl), new PropertyMetadata(0));

        /// <summary>
        /// 状态值转换器。
        /// </summary>
        public IValueConverter StateConverter
        {
            get { return (IValueConverter) GetValue(StateConverterProperty); }
            set { SetValue(StateConverterProperty, value); }
        }

        public static readonly DependencyProperty StateConverterProperty =
            DependencyProperty.Register("StateConverter", typeof(IValueConverter), typeof(MultipleStateControl),
                new PropertyMetadata(null));
    }
}