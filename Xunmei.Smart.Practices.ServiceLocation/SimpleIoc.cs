//Copyright \x00a9 GalaSoft Laurent Bugnion 2009-2012
namespace GalaSoft.MvvmLight.Ioc
{
    using Microsoft.Practices.ServiceLocation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class SimpleIoc : ISimpleIoc, IServiceLocator, IServiceProvider
    {
        private readonly Dictionary<Type, ConstructorInfo> _constructorInfos = new Dictionary<Type, ConstructorInfo>();
        private static SimpleIoc _default;
        private readonly string _defaultKey = Guid.NewGuid().ToString();
        private readonly object[] _emptyArguments = new object[0];
        private readonly Dictionary<Type, Dictionary<string, Delegate>> _factories = new Dictionary<Type, Dictionary<string, Delegate>>();
        private readonly Dictionary<Type, Dictionary<string, object>> _instancesRegistry = new Dictionary<Type, Dictionary<string, object>>();
        private readonly Dictionary<Type, Type> _interfaceToClassMap = new Dictionary<Type, Type>();
        private readonly object _syncLock = new object();

        public bool ContainsCreated<TClass>()
        {
            return this.ContainsCreated<TClass>(null);
        }

        public bool ContainsCreated<TClass>(string key)
        {
            Type type = typeof(TClass);
            if (!this._instancesRegistry.ContainsKey(type))
            {
                return false;
            }
            if (string.IsNullOrEmpty(key))
            {
                return (this._instancesRegistry[type].Count > 0);
            }
            return this._instancesRegistry[type].ContainsKey(key);
        }

        private object DoGetService(Type serviceType, string key)
        {
            lock (this._syncLock)
            {
                Dictionary<string, object> dictionary;
                if (string.IsNullOrEmpty(key))
                {
                    key = this._defaultKey;
                }
                if (!this._instancesRegistry.ContainsKey(serviceType))
                {
                    if (!this._interfaceToClassMap.ContainsKey(serviceType))
                    {
                        throw new ActivationException(string.Format("Type not found in cache: {0}.", serviceType.FullName));
                    }
                    dictionary = new Dictionary<string, object>();
                    this._instancesRegistry.Add(serviceType, dictionary);
                }
                else
                {
                    dictionary = this._instancesRegistry[serviceType];
                }
                if (dictionary.ContainsKey(key))
                {
                    return dictionary[key];
                }
                object obj2 = null;
                if (this._factories.ContainsKey(serviceType))
                {
                    if (!this._factories[serviceType].ContainsKey(key))
                    {
                        if (!this._factories[serviceType].ContainsKey(this._defaultKey))
                        {
                            throw new ActivationException(string.Format("Type not found in cache without a key: {0}", serviceType.FullName));
                        }
                        obj2 = this._factories[serviceType][this._defaultKey].DynamicInvoke(null);
                    }
                    else
                    {
                        obj2 = this._factories[serviceType][key].DynamicInvoke(null);
                    }
                }
                dictionary.Add(key, obj2);
                return obj2;
            }
        }

        private void DoRegister<TClass>(Type classType, Func<TClass> factory, string key)
        {
            if (this._factories.ContainsKey(classType))
            {
                if (this._factories[classType].ContainsKey(key))
                {
                    if (key == this._defaultKey)
                    {
                        throw new InvalidOperationException(string.Format("Class {0} is already registered.", classType.FullName));
                    }
                    throw new InvalidOperationException(string.Format("Class {0} is already registered with key {1}.", classType.FullName, key));
                }
                this._factories[classType].Add(key, factory);
            }
            else
            {
                Dictionary<string, Delegate> dictionary2 = new Dictionary<string, Delegate>();
                dictionary2.Add(key, factory);
                Dictionary<string, Delegate> dictionary = dictionary2;
                this._factories.Add(classType, dictionary);
            }
        }

        public IEnumerable<TService> GetAllCreatedInstances<TService>()
        {
            Type serviceType = typeof(TService);
            return (from instance in this.GetAllCreatedInstances(serviceType) select (TService) instance);
        }

        public IEnumerable<object> GetAllCreatedInstances(Type serviceType)
        {
            if (this._instancesRegistry.ContainsKey(serviceType))
            {
                return this._instancesRegistry[serviceType].Values;
            }
            return new List<object>();
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            Type serviceType = typeof(TService);
            return (from instance in this.GetAllInstances(serviceType) select (TService) instance);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            lock (this._factories)
            {
                if (this._factories.ContainsKey(serviceType))
                {
                    foreach (KeyValuePair<string, Delegate> pair in this._factories[serviceType])
                    {
                        this.GetInstance(serviceType, pair.Key);
                    }
                }
            }
            if (this._instancesRegistry.ContainsKey(serviceType))
            {
                return this._instancesRegistry[serviceType].Values;
            }
            return new List<object>();
        }

        private ConstructorInfo GetConstructorInfo(Type serviceType)
        {
            Type type;
            if (this._interfaceToClassMap.ContainsKey(serviceType))
            {
                type = this._interfaceToClassMap[serviceType] ?? serviceType;
            }
            else
            {
                type = serviceType;
            }
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length > 1)
            {
                if (constructors.Length > 2)
                {
                    return this.GetPreferredConstructorInfo(constructors, type);
                }
                if (constructors.FirstOrDefault<ConstructorInfo>(i => (i.Name == ".cctor")) == null)
                {
                    return this.GetPreferredConstructorInfo(constructors, type);
                }
                ConstructorInfo info = constructors.FirstOrDefault<ConstructorInfo>(i => i.Name != ".cctor");
                if ((info == null) || !info.IsPublic)
                {
                    throw new ActivationException(string.Format("Cannot register: No public constructor found in {0}.", type.Name));
                }
                return info;
            }
            if ((constructors.Length == 0) || ((constructors.Length == 1) && !constructors[0].IsPublic))
            {
                throw new ActivationException(string.Format("Cannot register: No public constructor found in {0}.", type.Name));
            }
            return constructors[0];
        }

        public TService GetInstance<TService>()
        {
            return (TService) this.DoGetService(typeof(TService), this._defaultKey);
        }

        public TService GetInstance<TService>(string key)
        {
            return (TService) this.DoGetService(typeof(TService), key);
        }

        public object GetInstance(Type serviceType)
        {
            return this.DoGetService(serviceType, this._defaultKey);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return this.DoGetService(serviceType, key);
        }

        private ConstructorInfo GetPreferredConstructorInfo(IEnumerable<ConstructorInfo> constructorInfos, Type resolveTo)
        {
            ConstructorInfo info = (from t in constructorInfos
                let attribute = Attribute.GetCustomAttribute(t, typeof(PreferredConstructorAttribute))
                where attribute != null
                select t).FirstOrDefault<ConstructorInfo>();
            if (info == null)
            {
                throw new ActivationException(string.Format("Cannot register: Multiple constructors found in {0} but none marked with PreferredConstructor.", resolveTo.Name));
            }
            return info;
        }

        public object GetService(Type serviceType)
        {
            return this.DoGetService(serviceType, this._defaultKey);
        }

        public bool IsRegistered<T>()
        {
            Type key = typeof(T);
            return this._interfaceToClassMap.ContainsKey(key);
        }

        public bool IsRegistered<T>(string key)
        {
            Type type = typeof(T);
            return ((this._interfaceToClassMap.ContainsKey(type) && this._factories.ContainsKey(type)) && this._factories[type].ContainsKey(key));
        }

        private TClass MakeInstance<TClass>()
        {
            Type key = typeof(TClass);
            ConstructorInfo info = this._constructorInfos.ContainsKey(key) ? this._constructorInfos[key] : this.GetConstructorInfo(key);
            ParameterInfo[] parameters = info.GetParameters();
            if (parameters.Length == 0)
            {
                return (TClass) info.Invoke(this._emptyArguments);
            }
            object[] objArray = new object[parameters.Length];
            foreach (ParameterInfo info2 in parameters)
            {
                objArray[info2.Position] = this.GetService(info2.ParameterType);
            }
            return (TClass) info.Invoke(objArray);
        }

        public void Register<TClass>() where TClass: class
        {
            this.Register<TClass>(false);
        }

        public void Register<TInterface, TClass>() where TInterface: class where TClass: class
        {
            this.Register<TInterface, TClass>(false);
        }

        public void Register<TClass>(bool createInstanceImmediately) where TClass: class
        {
            Type key = typeof(TClass);
            if (key.IsInterface)
            {
                throw new ArgumentException("An interface cannot be registered alone.");
            }
            lock (this._syncLock)
            {
                if (this._factories.ContainsKey(key) && this._factories[key].ContainsKey(this._defaultKey))
                {
                    if (!this._constructorInfos.ContainsKey(key))
                    {
                        throw new InvalidOperationException(string.Format("Class {0} is already registered.", key));
                    }
                }
                else
                {
                    if (!this._interfaceToClassMap.ContainsKey(key))
                    {
                        this._interfaceToClassMap.Add(key, null);
                    }
                    this._constructorInfos.Add(key, this.GetConstructorInfo(key));
                    Func<TClass> factory = new Func<TClass>(this.MakeInstance<TClass>);
                    this.DoRegister<TClass>(key, factory, this._defaultKey);
                    if (createInstanceImmediately)
                    {
                        this.GetInstance<TClass>();
                    }
                }
            }
        }

        public void Register<TInterface, TClass>(bool createInstanceImmediately) where TInterface: class where TClass: class
        {
            lock (this._syncLock)
            {
                Type key = typeof(TInterface);
                Type type2 = typeof(TClass);
                if (this._interfaceToClassMap.ContainsKey(key))
                {
                    if (this._interfaceToClassMap[key] != type2)
                    {
                        throw new InvalidOperationException(string.Format("There is already a class registered for {0}.", key.FullName));
                    }
                }
                else
                {
                    this._interfaceToClassMap.Add(key, type2);
                    this._constructorInfos.Add(type2, this.GetConstructorInfo(type2));
                }
                Func<TInterface> factory = new Func<TInterface>(this.MakeInstance<TInterface>);
                this.DoRegister<TInterface>(key, factory, this._defaultKey);
                if (createInstanceImmediately)
                {
                    this.GetInstance<TInterface>();
                }
            }
        }

        public void Register<TClass>(Func<TClass> factory) where TClass: class
        {
            this.Register<TClass>(factory, false);
        }

        public void Register<TClass>(Func<TClass> factory, bool createInstanceImmediately) where TClass: class
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            lock (this._syncLock)
            {
                Type key = typeof(TClass);
                if (this._factories.ContainsKey(key) && this._factories[key].ContainsKey(this._defaultKey))
                {
                    throw new InvalidOperationException(string.Format("There is already a factory registered for {0}.", key.FullName));
                }
                if (!this._interfaceToClassMap.ContainsKey(key))
                {
                    this._interfaceToClassMap.Add(key, null);
                }
                this.DoRegister<TClass>(key, factory, this._defaultKey);
                if (createInstanceImmediately)
                {
                    this.GetInstance<TClass>();
                }
            }
        }

        public void Register<TClass>(Func<TClass> factory, string key) where TClass: class
        {
            this.Register<TClass>(factory, key, false);
        }

        public void Register<TClass>(Func<TClass> factory, string key, bool createInstanceImmediately) where TClass: class
        {
            lock (this._syncLock)
            {
                Type type = typeof(TClass);
                if (this._factories.ContainsKey(type) && this._factories[type].ContainsKey(key))
                {
                    throw new InvalidOperationException(string.Format("There is already a factory registered for {0} with key {1}.", type.FullName, key));
                }
                if (!this._interfaceToClassMap.ContainsKey(type))
                {
                    this._interfaceToClassMap.Add(type, null);
                }
                this.DoRegister<TClass>(type, factory, key);
                if (createInstanceImmediately)
                {
                    this.GetInstance<TClass>(key);
                }
            }
        }

        public void Reset()
        {
            this._interfaceToClassMap.Clear();
            this._instancesRegistry.Clear();
            this._constructorInfos.Clear();
            this._factories.Clear();
        }

        public void Unregister<TClass>() where TClass: class
        {
            lock (this._syncLock)
            {
                Type type2;
                Type key = typeof(TClass);
                if (this._interfaceToClassMap.ContainsKey(key))
                {
                    type2 = this._interfaceToClassMap[key] ?? key;
                }
                else
                {
                    type2 = key;
                }
                if (this._instancesRegistry.ContainsKey(key))
                {
                    this._instancesRegistry.Remove(key);
                }
                if (this._interfaceToClassMap.ContainsKey(key))
                {
                    this._interfaceToClassMap.Remove(key);
                }
                if (this._factories.ContainsKey(key))
                {
                    this._factories.Remove(key);
                }
                if (this._constructorInfos.ContainsKey(type2))
                {
                    this._constructorInfos.Remove(type2);
                }
            }
        }

        public void Unregister<TClass>(TClass instance) where TClass: class
        {
            Func<KeyValuePair<string, object>, bool> predicate = null;
            lock (this._syncLock)
            {
                Type key = typeof(TClass);
                if (this._instancesRegistry.ContainsKey(key))
                {
                    Dictionary<string, object> source = this._instancesRegistry[key];
                    if (predicate == null)
                    {
                        predicate = pair => pair.Value == instance;
                    }
                    List<KeyValuePair<string, object>> list = source.Where<KeyValuePair<string, object>>(predicate).ToList<KeyValuePair<string, object>>();
                    for (int i = 0; i < list.Count<KeyValuePair<string, object>>(); i++)
                    {
                        KeyValuePair<string, object> pair = list[i];
                        string str = pair.Key;
                        source.Remove(str);
                        if (this._factories.ContainsKey(key) && this._factories[key].ContainsKey(str))
                        {
                            this._factories[key].Remove(str);
                        }
                    }
                }
            }
        }

        public void Unregister<TClass>(string key) where TClass: class
        {
            Func<KeyValuePair<string, object>, bool> predicate = null;
            lock (this._syncLock)
            {
                Type type = typeof(TClass);
                if (this._instancesRegistry.ContainsKey(type))
                {
                    Dictionary<string, object> source = this._instancesRegistry[type];
                    if (predicate == null)
                    {
                        predicate = pair => pair.Key == key;
                    }
                    List<KeyValuePair<string, object>> list = source.Where<KeyValuePair<string, object>>(predicate).ToList<KeyValuePair<string, object>>();
                    for (int i = 0; i < list.Count<KeyValuePair<string, object>>(); i++)
                    {
                        KeyValuePair<string, object> pair = list[i];
                        source.Remove(pair.Key);
                    }
                }
                if (this._factories.ContainsKey(type) && this._factories[type].ContainsKey(key))
                {
                    this._factories[type].Remove(key);
                }
            }
        }

        public static SimpleIoc Default
        {
            get
            {
                return (_default ?? (_default = new SimpleIoc()));
            }
        }
    }
}

