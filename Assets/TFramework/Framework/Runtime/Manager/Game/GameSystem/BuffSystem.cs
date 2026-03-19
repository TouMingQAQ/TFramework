using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace TFramework.Runtime.Buff
{

    /// <summary>
    /// Buff绑定目标
    /// </summary>
    public interface IBindObject { }
    

    /// <summary>
    /// Buff容器，管理Buff，绑定Buff和目标
    /// </summary>
    public class BuffSystem
    {
        public IBindObject BindObject { get; set; }
        private Dictionary<string, BuffData> m_buffMap = new();

        private List<BuffData> m_updateBuffDataList = new();
        public void Init()
        {
            m_buffMap.Clear();
            BakeExecuteBuff();
        }

        /// <summary>
        /// 烘焙需要更新的Buff，不执行这一步，Buff不会运行
        /// </summary>
        void BakeExecuteBuff()
        {
            m_updateBuffDataList.Clear();
            m_updateBuffDataList.AddRange(m_buffMap.Values);
            //根据更新顺序排序
            m_updateBuffDataList.Sort((x,y)=>x.Order<y.Order?1:-1);
        }
        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <param name="data"></param>
        /// <param name="defaultEnable"></param>
        public void AddBuff(BuffData data,bool defaultEnable = true)
        {
            data.SetActive(defaultEnable);
            data.Reset();
            data.BindObject = BindObject;
            if (m_buffMap.TryGetValue(data.BuffID, out var value))
                value.AddBuff(data);
            else 
                m_buffMap[data.BuffID] = data;
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
        public void RemoveBuff(Predicate<BuffData> comparable)
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
            BakeExecuteBuff();
        }

        /// <summary>
        /// 检索Buff
        /// </summary>
        /// <param name="comparable"></param>
        /// <returns></returns>
        public bool AnyBuff(Predicate<BuffData> comparable)
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
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            //TimeUpdate
            foreach (var buffData in m_updateBuffDataList)
            {
                if(!buffData.Active)
                    continue;
                if(!buffData.NeedUpdate)
                    continue;
                buffData.OnUpdate(deltaTime);
            }
            
        }
    }
}

