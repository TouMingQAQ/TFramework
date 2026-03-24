using System;
using System.Collections.Generic;

namespace TFramework.Runtime.Buff
{
    /// <summary>
    /// Buff的核心数据结构
    /// </summary>
    public abstract class BaseBuffData
    {
        public virtual Type Type => typeof(BaseBuffData);
        #region 属性,Buff的主要数据

        /// <summary>
        /// 激活状态
        /// </summary>
        private bool m_active = false;
        /// <summary>
        /// Buff名称
        /// </summary>
        private string m_BuffID = "";
        /// <summary>
        /// 执行列表
        /// </summary>
        private List<BaseBuffEffect> m_effectList = new List<BaseBuffEffect>();

        /// <summary>
        /// 优先级
        /// </summary>
        private int m_Order = -1;

        #endregion
        
        public BaseBuffData(string id,int order = -1,bool defaultActive = false)
        {
            m_active = defaultActive;
            m_BuffID = id;
            m_Order = order;
        }

        public void ClearEffect()
        {
            m_effectList.Clear();
        }

        public void AddEffect(BaseBuffEffect effect)
        {
            if(effect == null)
                return;
            effect.BaseBuffData = this;
            m_effectList.Add(effect);
        }
        /// <summary>
        /// ID
        /// </summary>
        public string BuffID => m_BuffID;
        /// <summary>
        /// 更新优先级
        /// </summary>
        public int Order => m_Order;

        /// <summary>
        /// 是否需要更新
        /// </summary>
        public virtual bool NeedUpdate => true;
        
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool Active
        {
            get => m_active;
            set => SetActive(value, false);
        }

        public BuffSystem System { get; set; } = null;

        /// <summary>
        /// 设置激活
        /// </summary>
        /// <param name="active"></param>
        /// <param name="forceEvent"></param>
        public void SetActive(bool active, bool forceEvent = false)
        {
            if(!forceEvent && m_active == active)
                return;
            if(active)
                OnEnable();
            else
                OnDisable();
            m_active = active;
        }
        
        
        protected virtual void OnEnable()
        {
            if(m_effectList == null)
                return;
            foreach (var buffAction in m_effectList)
            {
                buffAction.OnEnable(this);
            }
        }

        protected virtual void OnDisable()
        {
            if(m_effectList == null)
                return;
            foreach (var buffAction in m_effectList)
            {
                buffAction.OnDisable(this);
            }
        }
        
        public virtual void OnUpdate(float deltaTime)
        {
            if(m_effectList == null)
                return;
            if(!m_active)
                return;
            foreach (var buffEffect in m_effectList)
            {
                buffEffect.OnUpdate(deltaTime,this);
            }
        }
        /// <summary>
        /// Buff叠加
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddBuff(BaseBuffData data) {}

        public virtual void Effect()
        {
            if(m_effectList == null)
                return;
            foreach (var buffEffect in m_effectList)
            {
                buffEffect.Effect();
            }
        }
 
        /// <summary>
        /// 重置Buff
        /// </summary>
        public virtual void Reset()
        {
        }

        public virtual void Remove()
        {
            System.RemoveBuff(BuffID);
        }
    }

    /// <summary>
    /// 带泛型约束的Buff
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BuffData<T> : BaseBuffData where T : BuffData<T>
    {
        public BuffData(string id, int order = -1, bool defaultActive = false) : base(id, order, defaultActive)
        {
        }

        public override void AddBuff(BaseBuffData data)
        {
            base.AddBuff(data);
            if(data is T buffData)
                OnAddBuff(buffData);
        }

        protected abstract void OnAddBuff(T buffData);
    }
}