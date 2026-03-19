using System;

namespace TFramework.Runtime.Buff
{
    public abstract class BuffEffect
    {
        private EventNode m_eventNode = new EventNode();
        public virtual void OnEnable(BuffData buffdata){}
        public virtual void OnDisable(BuffData buffData){}
        public virtual void OnUpdate(float deltaTime,BuffData buffdata){}//Buff更新
        
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        protected void RegisterEvent<T>(Action<T> action) where T : struct,IBuffEvent
        {
            var handle = m_eventNode.Register<T>(action);
            handle.UnRegister();
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        protected void UnRegisterEvent<T>(Action<T> action) where T : struct,IBuffEvent 
        {
            m_eventNode.UnRegister<T>(action);
        }
        /// <summary>
        /// 通知事件
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void CallEvent<T>(T value = default) where T : IBuffEvent
        {
            m_eventNode.Call<T>(value);
        }
    }

}