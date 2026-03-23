using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace TFramework.Runtime.Buff
{
    /// <summary>
    /// Buff容器，管理Buff，绑定Buff和目标
    /// </summary>
    public class BuffSystem
    {
        private Dictionary<string, BaseBuffData> m_buffMap = new();

        private List<BaseBuffData> m_updateBuffDataList = new();

        /// <summary>
        /// BuffList缓存容器，
        /// 只用于遍历Buff更新
        /// </summary>
        private List<BaseBuffData> m_updateBuffDataListCache = new();

        #region Control

        public void Init()
        {
            m_buffMap.Clear();
            BakeExecuteBuff();
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if(m_updateBuffDataList == null)
                return;
            m_updateBuffDataListCache ??= new();
            m_updateBuffDataListCache.Clear();
            m_updateBuffDataListCache.AddRange(m_updateBuffDataList);
            //TimeUpdate
            foreach (var buffData in m_updateBuffDataListCache)
            {
                if(!buffData.Active)
                    continue;
                if(!buffData.NeedUpdate)
                    continue;
                buffData.OnUpdate(deltaTime);
            }
        }

        #endregion

        #region BuffControl

        /// <summary>
        /// 烘焙需要更新的Buff，不执行这一步，Buff不会运行
        /// </summary>
        protected void BakeExecuteBuff()
        {
            m_updateBuffDataList.Clear();
            m_updateBuffDataList.AddRange(m_buffMap.Values);
            //根据更新顺序排序
            m_updateBuffDataList.Sort((x,y)=>x.Order<y.Order?1:-1);
        }

        /// <summary>
        /// 添加BuffList
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="defaultEnable"></param>
        public void AddBuff(IEnumerable<BaseBuffData> dataList, bool defaultEnable = true)
        {
            foreach (var buffData in dataList)
            {
                AddBuff(buffData, defaultEnable, false);
            }
            BakeExecuteBuff();
        }

        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <param name="data"></param>
        /// <param name="defaultEnable"></param>
        /// <param name="bakeOrder"></param>
        public void AddBuff(BaseBuffData data,bool defaultEnable = true,bool bakeOrder = true)
        {
            data.System = this;
            data.SetActive(defaultEnable);
            data.Reset();
            if (m_buffMap.TryGetValue(data.BuffID, out var value))
                value.AddBuff(data);
            else 
                m_buffMap[data.BuffID] = data;
            if(bakeOrder)
                BakeExecuteBuff();
        }

        /// <summary>
        /// 移除Buff
        /// </summary>
        /// <param name="buffID"></param>
        public void RemoveBuff(string buffID)
        {
            RemoveBuff(x => x.BuffID == buffID);
        }

        /// <summary>
        /// 根据规则移除Buff
        /// </summary>
        /// <param name="comparable"></param>
        /// <param name="bakeOrder"></param>
        public void RemoveBuff(Predicate<BaseBuffData> comparable,bool bakeOrder = true)
        {
            if(comparable == null)
                return;
            List<string> removeBuff = ListPool<string>.Get();
            foreach (var value in m_buffMap.Values)
            {
                if(comparable.Invoke(value))
                    removeBuff.Add(value.BuffID);
            }

            foreach (var id in removeBuff)
            {
                m_buffMap.Remove(id, out var buffData);
                buffData.SetActive(false);
            }
            ListPool<string>.Release(removeBuff);
            if(bakeOrder)
                BakeExecuteBuff();
        }

        /// <summary>
        /// 检索Buff
        /// </summary>
        /// <param name="comparable"></param>
        /// <returns></returns>
        public bool AnyBuff(Predicate<BaseBuffData> comparable)
        {
            if (comparable == null)
                return false;
            foreach (var value in m_buffMap.Values)
            {
                if (comparable.Invoke(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 根据条件查询Buff
        /// </summary>
        /// <param name="comparable"></param>
        /// <param name="dataList"></param>
        public void QueryBuff(Predicate<BaseBuffData> comparable,in List<BaseBuffData> dataList)
        {
            if(dataList == null)
                return;
            dataList.Clear();
            foreach (var value in m_buffMap.Values)
            {
                if (comparable.Invoke(value))
                    dataList.Add(value);
            }
        }

        #endregion

        #region Event

        private EventNode m_Event = new EventNode();
        private EventNode m_ObjEvent = new EventNode();
        
        /// <summary>
        /// 注册效果
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void RegisterEffect<T>(Action<T> action) where T : BuffEffect<T>
        {
            m_ObjEvent.Register(action);
        }
        /// <summary>
        /// 注销效果
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        public void UnRegisterEffect<T>(Action<T> action) where T : BuffEffect<T>
        {
            m_ObjEvent.UnRegister(action);
        }

        internal void Effect<T>(T value) where T : BuffEffect<T>
        {
            var res = m_ObjEvent.Call(value);
            if(!res)
                Debug.LogWarning($"[<color=red>Effect</color>]:没有对象处理事件[<color=green>{typeof(T)}</color>]");
        }

        internal void RegisterEvent<T>(Action<T> action)
        {
            m_Event.Register(action);
        }

        internal void UnRegisterEvent<T>(Action<T> action)
        {
            m_Event.UnRegister(action);
        }

        /// <summary>
        /// 通知事件，注册对应事件的BuffData会响应
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void CallEvent<T>(T value)
        {
            m_Event.Call(value);
        }
        
        

        #endregion
    }
}