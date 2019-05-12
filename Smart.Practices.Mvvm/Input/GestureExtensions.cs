using System.Globalization;

namespace System.Windows.Input
{
    internal static class GestureExtensions
    {
        public static string GetDisplayString(this KeyGesture gesture)
        {
            string gestureText = gesture.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);

            if (gesture.Key == Key.OemPlus || gesture.Key == Key.OemMinus)
            {
                switch (gesture.Modifiers)
                {
                    case ModifierKeys.Alt:
                        gestureText = "Alt ";
                        break;
                    case ModifierKeys.Control:
                        gestureText = "Ctrl ";
                        break;
                    case ModifierKeys.Shift:
                        gestureText = "Shift ";
                        break;
                    case ModifierKeys.Windows:
                        gestureText = "Win ";
                        break;
                    case ModifierKeys.None:
                    default:
                        break;
                }

                if (!string.IsNullOrWhiteSpace(gestureText))
                {
                    gestureText += " + ";
                }

                switch (gesture.Key)
                {
                    case Key.OemPlus:
                        gestureText += "小键盘+";
                        break;
                    case Key.OemMinus:
                        gestureText += "小键盘-";
                        break;
                }
            }

            return gestureText;
        }

        public static string GetDisplayString(this MouseGesture gesture)
        {
            string gestureText = string.Empty;

            switch (gesture.Modifiers)
            {
                case ModifierKeys.Alt:
                    gestureText = "Alt ";
                    break;
                case ModifierKeys.Control:
                    gestureText = "Ctrl ";
                    break;
                case ModifierKeys.Shift:
                    gestureText = "Shift ";
                    break;
                case ModifierKeys.Windows:
                    gestureText = "Win ";
                    break;
                case ModifierKeys.None:
                default:
                    break;
            }

            if (!string.IsNullOrWhiteSpace(gestureText))
            {
                gestureText += " + ";
            }

            switch (gesture.MouseAction)
            {
                case MouseAction.LeftClick:
                    gestureText += "单击鼠标左键";
                    break;
                case MouseAction.LeftDoubleClick:
                    gestureText += "双击鼠标左键";
                    break;
                case MouseAction.MiddleClick:
                    gestureText += "单击鼠标中键";
                    break;
                case MouseAction.MiddleDoubleClick:
                    gestureText += "双击鼠标中键";
                    break;
                case MouseAction.RightClick:
                    gestureText += "单击鼠标右键";
                    break;
                case MouseAction.RightDoubleClick:
                    gestureText += "双击鼠标右键";
                    break;
                case MouseAction.WheelClick:
                    gestureText += "鼠标滚轮";
                    if(gesture is ImprovedMouseGesture)
                    {
                        switch((gesture as ImprovedMouseGesture).WheelDirection)
                        {
                            case MouseWheelDirection.Up:
                                gestureText += "向上";
                                break;
                            case MouseWheelDirection.Down:
                                gestureText += "向下";
                                break;
                        }
                    }
                    break;
                case MouseAction.None:
                default:
                    break;
            }
            return gestureText;
        }
    }
}
