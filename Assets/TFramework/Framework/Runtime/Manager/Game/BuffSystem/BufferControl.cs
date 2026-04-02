using System;
using System.Collections.Generic;
using TFramework.Runtime.Buff;
using UnityEngine;

namespace TFramework.Runtime.Buff
{
    public class BuffEventHandle : EventNode.IEventHandle
    {
        public EventNode Node { get; set; }
        public Delegate Action { get; set; }
        public Type EventType { get; set; }
    }
    /// <summary>
    /// Buff控制器，主要对接多个BuffSystem
    /// 一对多
    /// </summary>
    public class BufferControl
    {
        private Dictionary<Type, EventNode.IEventHandle> eventHandleMap = new();
        private List<BuffSystem> _buffSystemList = new();
        /// <summary>
        /// 添加BuffSystem
        /// </summary>
        /// <param name="system"></param>
        public void AddBuffSystem(BuffSystem system)
        {
            _buffSystemList.Add(system);
            foreach (var value in eventHandleMap.Values)
            {
                system.EffectEvent.RegisterHandle(value);
            }
        }

        /// <summary>
        /// 移除BuffSystem
        /// </summary>
        /// <param name="system"></param>
        public void RemoveBuffSystem(BuffSystem system)
        {
            if(!_buffSystemList.Remove(system))
                return;
            foreach (var value in eventHandleMap.Values)
            {
                system.EffectEvent.UnRegisterHandle(value);
            }
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void ClearBuffSystem()
        {
            foreach (var buffSystem in _buffSystemList)
            {
                foreach (var value in eventHandleMap.Values)
                {
                    buffSystem.EffectEvent.UnRegisterHandle(value);
                }
            }
            _buffSystemList.Clear();
        }
        /// <summary>
        /// 注册效果
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void AddEffect<T>(Action<T> action) where T : BuffEffect<T>
        {
            var type = typeof(T);
            if (eventHandleMap.TryGetValue(type, out var handle))
            {
                Debug.LogWarning("重复注册效果");
                return;
            }
            handle = new BuffEventHandle()
            {
                Action = action,
                EventType = type,
                Node = null,
            };
            eventHandleMap[type] = handle;

            foreach (var buffSystem in _buffSystemList)
            {
                buffSystem.EffectEvent.RegisterHandle(handle);
            }
        }
        /// <summary>
        /// 通知事件，注册对应事件的BuffData会响应
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void CallEvent<T>(T value)
        {
            foreach (var buffSystem in _buffSystemList)
            {
                buffSystem.CallEvent(value);
            }
        }

        /// <summary>
        /// 查询Buff
        /// </summary>
        /// <param name="comparable"></param>
        /// <param name="dataList"></param>
        public void QueryBuff(Predicate<BaseBuffData> comparable, List<BaseBuffData> dataList)
        {
            if(dataList == null)
                return;
            dataList.Clear();
            foreach (var buffSystem in _buffSystemList)
            {
                buffSystem.QueryBuff(comparable, dataList.Add);
            }
        }
    }

}
