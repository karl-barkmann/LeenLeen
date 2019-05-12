using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using Leen.Windows.Utils;

namespace Leen.Windows.Interactivity
{
    public class DragBehavior : Behavior<UIElement>
    {
        private Point? _lastMouseDown;

        public object Bomb
        {
            get { return (object)GetValue(BombProperty); }
            set { SetValue(BombProperty, value); }
        }

        public static readonly DependencyProperty BombProperty =
            DependencyProperty.Register("Bomb", typeof(object), typeof(DragBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DragBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            AssociatedObject.GiveFeedback += AssociatedObject_GiveFeedback;
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            base.OnAttached();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        DragAdorner adorner;

        void AssociatedObject_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (adorner != null)
            {
                UIElement element = sender as UIElement;
                var pos = element.PointFromScreen(GetMousePosition());
                adorner.UpdatePosition(pos);
            }
        }

        void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed && Bomb != null)
                {
                    var draggingElement = FindProperDragingElement();
                    if (draggingElement != null)
                    {
                        Point currentPosition = e.GetPosition(draggingElement);

                        if (_lastMouseDown.HasValue && ((Math.Abs(currentPosition.X - _lastMouseDown.Value.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                            (Math.Abs(currentPosition.Y - _lastMouseDown.Value.Y) > SystemParameters.MinimumVerticalDragDistance)))
                        {
                            var decorator = Application.Current.MainWindow.FindChild<AdornerDecorator>();
                            AdornerLayer adornerLayer = null;
                            if (decorator != null)
                            {
                                adornerLayer = decorator.AdornerLayer;
                                if (adornerLayer != null)
                                {
                                    adorner = new DragAdorner(draggingElement, e.GetPosition(draggingElement));
                                    adornerLayer.Add(adorner);
                                }
                            }
                            var effects = DragDrop.DoDragDrop(draggingElement, Bomb, DragDropEffects.Move);
                            if (effects == DragDropEffects.Move)
                            {
                                if (Command != null && Command.CanExecute(Bomb))
                                {
                                    Command.Execute(Bomb);
                                }
                            }
                            //Indicate dragging is over.
                            _lastMouseDown = null;
                            if (adornerLayer != null)
                            {
                                adornerLayer.Remove(adorner);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        protected virtual UIElement FindProperDragingElement()
        {
            return AssociatedObject;
        }

        void AssociatedObject_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(AssociatedObject);
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.GiveFeedback -= AssociatedObject_GiveFeedback;
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            base.OnDetaching();
        }
    }

    public class DataGridDragBehavior : DragBehavior
    {
        protected override UIElement FindProperDragingElement()
        {
            var dataGrid = AssociatedObject as DataGrid;
            if (dataGrid == null)
            {
                throw new ArgumentException("AssociatedObject is not type of 'DataGrid'!");
            }
            if (dataGrid.SelectedIndex < 0 || dataGrid.SelectedIndex < dataGrid.Items.Count)
            {
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);

                return row;
            }
            return null;
        }
    }

    public class DragAdorner : Adorner
    {
        public DragAdorner(UIElement adornedElement, Point offset)

            : base(adornedElement)
        {
            this.offset = offset;
            vbrush = new VisualBrush(AdornedElement);
            vbrush.Opacity = .7;

            IsHitTestVisible = false;
        }

        public void UpdatePosition(Point location)
        {
            this.location = location;
            var adornerLayer = Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            var p = location;
            p.Offset(-offset.X, -offset.Y);
            dc.DrawRectangle(vbrush, null, new Rect(p, this.RenderSize));
        }

        private Brush vbrush;

        private Point location;

        private Point offset;
    }
}
