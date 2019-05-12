using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Leen.Windows
{
    // Lots of specialized registration methods to avoid new'ing up more common stuff (like InputGesture's) at the callsite, as that's frequently
    // repeated and increases code size.  Do it once, here.  
    /// <summary>
    /// Used by custome control library for command registering.
    /// </summary>
    public static class CommandHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="command"></param>
        /// <param name="executedRoutedEventHandler"></param>
        /// <param name="key"></param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            Key key)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, new KeyGesture(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="command"></param>
        /// <param name="executedRoutedEventHandler"></param>
        /// <param name="inputGesture"></param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="command"></param>
        /// <param name="executedRoutedEventHandler"></param>
        /// <param name="inputGesture"></param>
        /// <param name="inputGesture2"></param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            InputGesture inputGesture,
            InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture, inputGesture2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="command"></param>
        /// <param name="executedRoutedEventHandler"></param>
        /// <param name="canExecuteRoutedEventHandler"></param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="command"></param>
        /// <param name="executedRoutedEventHandler"></param>
        /// <param name="canExecuteRoutedEventHandler"></param>
        /// <param name="inputGesture"></param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="command"></param>
        /// <param name="executedRoutedEventHandler"></param>
        /// <param name="canExecuteRoutedEventHandler"></param>
        /// <param name="inputGesture"></param>
        /// <param name="inputGesture2"></param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            InputGesture inputGesture,
            InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2);
        }

        private static void PrivateRegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            params InputGesture[] inputGestures)
        {

            // Validate parameters
            Debug.Assert(controlType != null);
            Debug.Assert(command != null);
            Debug.Assert(executedRoutedEventHandler != null);
            // All other parameters may be null

            // Create command link for this command
            CommandManager.RegisterClassCommandBinding(controlType, new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));

            // Create additional input binding for this command
            if (inputGestures != null)
            {
                for (int i = 0; i < inputGestures.Length; i++)
                {
                    CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, inputGestures[i]));
                }
            }
        }
    }
}
