using System;
using System.Collections.Generic;

namespace TFramework.Runtime.Buff
{
    public interface IBuffEvent { }
    
    public partial class EventNode
    {
        public interface IEventHandle
        {
            public EventNode Node { get; set; }
            public Delegate Action { get; set; }
            public Type EventType { get; set; }

            /// <summary>
            /// 主动注销
            /// </summary>
            public void UnRegister()
            {
                if(Node==null)
                    return;
                if(Action == null)
                    return;
                if(EventType == null)
                    return;
                // 如果事件存在于字典中，移除指定的事件委托
                if (!Node._eventMap.TryGetValue(EventType, out var existingDelegate)) 
                    return;
                var newDelegate = Delegate.Remove(existingDelegate, Action);
                if (newDelegate == null)
                    Node._eventMap.Remove(EventType);
                else
                    Node._eventMap[EventType] = newDelegate;
            }

            public bool Call(params object[] objects)
            {
                if (Action == null)
                    return false;
                Action.DynamicInvoke(objects);
                return true;
            }
        }
        public class Handle : IEventHandle
        {
            public EventNode Node { get; set; }
            public Delegate Action { get; set; }
            public Type EventType { get; set; }
        }

        protected Dictionary<Type, Delegate> _eventMap = new();

        /// <summary>
        /// 广播事件
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public bool Call<T>(T value)
        {
            var eventType = typeof(T);

            // 如果事件存在于字典中，转换为 Action<T> 并调用
            if (_eventMap.TryGetValue(eventType, out var existingDelegate))
            {
                var action = existingDelegate as Action<T>;
                if (action == null)
                    return false;
                action.Invoke(value);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public IEventHandle Register<T>(Action<T> action)
        {
            var handle = CreateHandle<T>(action);
            if (action == null) 
                return handle;

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

            return handle;
        }
        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void UnRegister<T>(Action<T> action)
        {
            if (action == null) 
                return;

            var eventType = typeof(T);

            // 如果事件存在于字典中，移除指定的事件委托
            if (!_eventMap.TryGetValue(eventType, out var existingDelegate)) 
                return;
            var newDelegate = Delegate.Remove(existingDelegate, action);
            if (newDelegate == null)
                _eventMap.Remove(eventType);
            else
                _eventMap[eventType] = newDelegate;
        }

        protected IEventHandle CreateHandle<T>(Action<T> action)
        {
            var type = typeof(T);
            return new Handle()
            {
                Action = action,
                EventType = type,
                Node = this,
            };
        }
    }
}