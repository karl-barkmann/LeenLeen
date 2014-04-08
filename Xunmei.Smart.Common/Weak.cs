using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Xunmei.Smart.Common
{
    /// <summary>
    /// Implements a weak event listener that allows the owner to be garbage
    /// collected if its only remaining link is an event handler.
    /// </summary>
    /// <typeparam name="TInstance">Type of instance listening for the event.</typeparam>
    /// <typeparam name="TSource">Type of source for the event.</typeparam>
    /// <typeparam name="TEventArgs">Type of event arguments for the event.</typeparam>
    public class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        /// <summary>
        /// WeakReference to the instance listening for the event.
        /// </summary>
        private WeakReference _weakInstance;

        /// <summary>
        /// Gets or sets the method to call when the event fires.
        /// </summary>
        public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

        /// <summary>
        /// Gets or sets the method to call when detaching from the event.
        /// </summary>
        public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

        /// <summary>
        /// Initializes a new instances of the WeakEventListener class.
        /// </summary>
        /// <param name="instance">Instance subscribing to the event.</param>
        public WeakEventListener(TInstance instance)
        {
            if (null == instance)
            {
                throw new ArgumentNullException("instance");
            }
            _weakInstance = new WeakReference(instance);
        }

        /// <summary>
        /// Handler for the subscribed event calls OnEventAction to handle it.
        /// </summary>
        /// <param name="source">Event source.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public void OnEvent(TSource source, TEventArgs eventArgs)
        {
            TInstance target = (TInstance)_weakInstance.Target;
            if (null != target)
            {
                // Call registered action
                if (null != OnEventAction)
                {
                    OnEventAction(target, source, eventArgs);
                }
            }
            else
            {
                // Detach from event
                Detach();
            }
        }

        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        public void Detach()
        {
            if (null != OnDetachAction)
            {
                OnDetachAction(this);
                OnDetachAction = null;
            }
        }
    }

    /// <summary>
    /// Implements a weak event listener that allows the owner to be garbage
    /// collected if its only remaining link is an event handler.
    /// </summary>
    /// <typeparam name="TSource">Type of source for the event.</typeparam>
    /// <typeparam name="TEventArgs">Type of event arguments for the event.</typeparam>
    public abstract class WeakEventListener<TSource, TEventArgs> : IDisposable where TEventArgs : EventArgs
    {
        private readonly WeakReference weakSource;
        private readonly WeakReference weakTarget;
        private readonly MethodInfo method;

        /// <summary>
        /// Initializes a new instances of the WeakEventListener class.
        /// </summary>
        /// <param name="source">Source of the event.</param>
        /// <param name="handler">Event arguments for the event.</param>
        protected WeakEventListener(TSource source, Delegate handler)
        {
            this.weakSource = new WeakReference(source);
            if (handler.Target != null)
            {
                this.weakTarget = new WeakReference(handler.Target);
            }

            this.method = handler.Method;
        }

        /// <summary>
        /// DeConstructor.
        /// </summary>
        ~WeakEventListener()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose resources.
        /// </summary>
        /// <param name="disposing">A value indicate if is disposing by caller.</param>
        protected virtual void Dispose(bool disposing)
        {
            object source = this.weakSource.Target;
            if (source != null)
            {
                this.StopListening((TSource)source);
            }
        }

        /// <summary>
        /// Stop listening the event.
        /// </summary>
        /// <param name="source">Source of the event.</param>
        protected abstract void StopListening(TSource source);

        /// <summary>
        /// Fire when event occurs.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="args">Event arguments for the event.</param>
        protected void OnEvent(object sender, TEventArgs args)
        {
            object target = null;
            if (this.weakTarget != null)
            {
                target = this.weakTarget.Target;
                if (target == null)
                {
                    this.Dispose();
                    return;
                }
            }
            this.method.Invoke(target, new object[] { sender, args });
        }
    }

    /// <summary>
    /// Represents a weak reference, which references an object while still allowing
    /// that object to be reclaimed by garbage collection.
    /// </summary>
    /// <typeparam name="T">The type of the object that is referenced.</typeparam>
    [Serializable]
    public class WeakReference<T> : WeakReference where T : class
    {
        /// <summary>
        /// Initializes a new instance of the WeakReference{T} class, referencing
        /// the specified object.
        /// </summary>
        /// <param name="target">The object to reference.</param>
        public WeakReference(T target)
            : base(target)
        {

        }

        /// <summary>
        /// Initializes a new instance of the WeakReference{T} class, referencing
        /// the specified object and using the specified resurrection tracking.
        /// </summary>
        /// <param name="target">An object to track.</param>
        /// <param name="trackResurrection">Indicates when to stop tracking the object. 
        /// If true, the object is tracked
        /// after finalization; if false, the object is only tracked 
        /// until finalization.</param>
        public WeakReference(T target, bool trackResurrection)
            : base(target, trackResurrection)
        {

        }

        protected WeakReference(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>
        /// Gets or sets the object (the target) referenced by the 
        /// current WeakReference{T} object.
        /// </summary>
        public new T Target
        {
            get
            {
                return (T)base.Target;
            }
            set
            {
                base.Target = value;
            }
        }
    }
}