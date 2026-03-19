using System.Collections.Generic;

namespace TFramework.Runtime.Buff
{
    /// <summary>
    /// Buff的核心数据结构
    /// </summary>
    public abstract class BuffData
    {
        #region 属性,Buff的主要数据

        /// <summary>
        /// 激活状态
        /// </summary>
        private bool m_active;
        /// <summary>
        /// Buff名称
        /// </summary>
        private string m_BuffID;
        /// <summary>
        /// 执行列表
        /// </summary>
        private List<BuffEffect> m_effectList;

        /// <summary>
        /// 优先级
        /// </summary>
        private int m_Order;

        #endregion
        
        public BuffData(string id,int order = -1,bool defaultActive = true)
        {
            m_active = defaultActive;
            m_BuffID = id;
            m_effectList = new List<BuffEffect>();
            BindObject = null;
            m_Order = order;
        }

        public void ClearEffect()
        {
            m_effectList.Clear();
        }

        public void AddEffect(BuffEffect effect)
        {
            if(effect == null)
                return;
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
        /// <summary>
        /// 绑定目标
        /// </summary>
        public IBindObject BindObject { get; set; }

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
        
        public void OnUpdate(float deltaTime)
        {
            if(m_effectList == null)
                return;
            if(!m_active)
                return;
            foreach (var buffAction in m_effectList)
            {
                buffAction.OnUpdate(deltaTime,this);
            }
        }


        /// <summary>
        /// Buff叠加
        /// </summary>
        /// <param name="data"></param>
        public virtual void AddBuff(BuffData data) {}
        /// <summary>
        /// 重置Buff
        /// </summary>
        public virtual void Reset()
        {
        }
    }
}