using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TFramework.Runtime
{
    public partial class BaseManager : MonoBehaviour
    {
        public Framework framework;

        protected Dictionary<Type, Delegate> _eventMap = new();

        #region System

        protected Dictionary<Type, BaseSystem> _systemMap = new();
        
        /// <summary>
        /// 添加系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected T AddSystem<T>() where T : BaseSystem,new()
        {
            var type = typeof(T);
            if (_systemMap.TryGetValue(type, out var value))
                return value as T;
            var system = new T
            {
                Manager = this
            };
            system.Init();
            _systemMap[type] = system;
            Debug.Log($"<color=#66ccff>[{GetType()}]</color> add system:{type}");
            return system;
        }

       

        /// <summary>
        /// 移除系统
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T RemoveSystem<T>() where T : BaseSystem
        {
            var type = typeof(T);
            if (_systemMap.Remove(type, out var value))
            {
                value.Destroy();
                return value as T;
            }
            return default;
        }
        /// <summary>
        /// 清空System
        /// </summary>
        protected void ClearSystem()
        {
            foreach (var system in _systemMap.Values)
            {
                system.Destroy();
            }
            _systemMap.Clear();
        }
        /// <summary>
        /// 获取子系统
        /// 不存在会报错
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetSystem<T>() where T : BaseSystem
        {
            var type = typeof(T);
            if (_systemMap.TryGetValue(type, out var value))
                return value as T;
            else
                Debug.LogError($"<color=red>[{GetType()}]</color> can`t find system:{type}");
            return default(T);
        }

        #endregion

        #region Module
        protected Dictionary<Type, BaseModule> _moduleMap = new();

        /// <summary>
        /// 添加系统模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected T AddModule<T>() where T : BaseModule,new()
        {
            var type = typeof(T);
            if (_moduleMap.TryGetValue(type, out var value))
                return value as T;
            var module = new T();
            module.Init();
            _moduleMap[type] = module;
            Debug.Log($"<color=#66ccff>[{GetType()}]</color> add module:{type}");
            return module;
        }

        /// <summary>
        /// 移除系统模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T RemoveModule<T>() where T : BaseModule
        {
            var type = typeof(T);
            if (_moduleMap.Remove(type, out var value))
            {
                value.Destroy();
                return value as T;
            }
            return default;
        }
        /// <summary>
        /// 清空模块
        /// </summary>
        protected void ClearModule()
        {
            foreach (var module in _moduleMap.Values)
            {
                module.Destroy();
            }
            _moduleMap.Clear();
        }
        
        /// <summary>
        /// 获取Module
        /// 如果目标Module不存在，会主动添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetModule<T>() where T : BaseModule,new()
        {
            var type = typeof(T);
            if (_moduleMap.TryGetValue(type, out var value))
                return value as T;
            return AddModule<T>();
        }

        #endregion
        
        /// <summary>
        /// 获取Manager
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetManager<T>() where T : BaseManager
        {
            return framework.GetManager<T>();
        }

        protected void Update()
        {
            foreach (var system in _systemMap.Values) 
            {
                system.Update();
            }
        }
        protected void LateUpdate()
        {
            foreach (var system in _systemMap.Values) 
            {
                system.LateUpdate();
            }
        }
        protected virtual void FixedUpdate()
        {
            foreach (var system in _systemMap.Values) 
            {
                system.Tick(Time.deltaTime);
            }
            foreach (var module in _moduleMap.Values) 
            {
                if(module.Enable)
                    module.Tick(Time.deltaTime);
            }
        }
        

        #region EventMethod

        /// <summary>
        /// 向此Manager所属的System广播事件
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void Call<T>(T value = default)
        {
            var eventType = typeof(T);

            // 如果事件存在于字典中，转换为 Action<T> 并调用
            if (_eventMap.TryGetValue(eventType, out var existingDelegate))
            {
                var action = existingDelegate as Action<T>;
                action?.Invoke(value);
            }
        }

        private void Register<T>(Action<T> action)
        {
            if (action == null) return;

            var eventType = typeof(T);

            // 如果当前事件类型已经存在于字典中，将新的事件委托与现有的委托组合
            if (_eventMap.TryGetValue(eventType, out var existingDelegate))
            {
                _eventMap[eventType] = Delegate.Combine(existingDelegate, action);
            }
            else
            {
                // 否则直接添加新的事件委托
                _eventMap[eventType] = action;
            }
        }
        private void UnRegister<T>(Action<T> action)
        {
            if (action == null) return;

            var eventType = typeof(T);

            // 如果事件存在于字典中，移除指定的事件委托
            if (_eventMap.TryGetValue(eventType, out var existingDelegate))
            {
                var newDelegate = Delegate.Remove(existingDelegate, action);
                if (newDelegate == null)
                {
                    _eventMap.Remove(eventType);
                }
                else
                {
                    _eventMap[eventType] = newDelegate;
                }
            }
        }

        #endregion

       
       
    }

    public partial class BaseManager
    {
        public class BaseSystem
        {
            public BaseManager Manager;
            protected Dictionary<Type, BaseModule> _moduleMap = new();
            /// <summary>
            /// 获得系统，只能获取父级Manager中的系统
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            protected T GetSystem<T>() where T : BaseSystem
            {
                return Manager.GetSystem<T>();
            }
            /// <summary>
            /// 获得Manager
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            protected T GetManager<T>() where T : BaseManager
            {
                return Manager.framework.GetManager<T>();
            }
            /// <summary>
            /// 初始化
            /// </summary>
            public virtual void Init() { }

            public virtual void Destroy()
            {
                ClearModule();
                Manager = null;
            }
            public virtual void Update(){}
            public virtual void LateUpdate(){}

            public virtual void Tick(float deltaTime)
            {
                foreach (var module in _moduleMap.Values)
                {
                    if(module.Enable)
                        module.Tick(deltaTime);
                }
            }

            /// <summary>
            /// 添加系统模块
            /// </summary>
            /// <typeparam name="T"></typeparam>
            protected T AddModule<T>() where T : BaseModule,new()
            {
                var type = typeof(T);
                if (_moduleMap.TryGetValue(type, out var value))
                    return value as T;
                var module = new T();
                module.Init();
                _moduleMap[type] = module;
                Debug.Log($"<color=#66ccff>[{GetType()}]</color> add module:{type}");
                return module;
            }

            /// <summary>
            /// 移除系统模块
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            protected T RemoveModule<T>() where T : BaseModule
            {
                var type = typeof(T);
                if (_moduleMap.Remove(type, out var value))
                {
                    value.Destroy();
                    return value as T;
                }
                return default;
            }
            /// <summary>
            /// 清空模块
            /// </summary>
            protected void ClearModule()
            {
                foreach (var module in _moduleMap.Values)
                {
                    module.Destroy();
                }
                _moduleMap.Clear();
            }
        
            /// <summary>
            /// 获取Module
            /// 如果目标Module不存在，会主动添加
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            protected T GetModule<T>() where T : BaseModule,new()
            {
                var type = typeof(T);
                if (_moduleMap.TryGetValue(type, out var value))
                    return value as T;
                return AddModule<T>();
            }

            /// <summary>
            /// 注册事件
            /// </summary>
            /// <param name="action"></param>
            /// <typeparam name="T"></typeparam>
            protected void Register<T>(Action<T> action)=>Manager.Register(action);
            /// <summary>
            /// 注销事件
            /// </summary>
            /// <param name="action"></param>
            /// <typeparam name="T"></typeparam>
            protected void UnRegister<T>(Action<T> action)=>Manager.UnRegister(action);
        }
    }

    /// <summary>
    /// 基础模块类
    /// </summary>
    public class BaseModule
    {
        private bool _enable = false;
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool Enable
        {
            get=> _enable;
            set=> _enable = value;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init(){}
        public virtual void Destroy(){}
        public virtual void Tick(float deltaTime){}
    }
    
}