using System.Collections.Generic;
using UnityEngine;

namespace TFramework.BuffSystem
{
    public abstract class BuffCentral<TBuff> : MonoBehaviour where TBuff : BuffCentral<TBuff>
    {
        public List<BaseBuff<TBuff>> buffList;
        protected List<BaseBuff<TBuff>> _addBuffList;
        protected List<BaseBuff<TBuff>> _removeBuffList;
        private void Awake()
        {
            buffList = new();
            _addBuffList = new();
            _removeBuffList = new();
        }
        /// <summary>
        /// 开启关闭Buff
        /// </summary>
        /// <param name="active"></param>
        /// <typeparam name="T"></typeparam>
        public void ActiveBuffer<T>(bool active) where T : BaseBuff<TBuff>
        {
            foreach (var baseBuff in buffList)
            {
                if (baseBuff is T buff)
                {
                    buff.Active = active;
                }
            }
        }
        /// <summary>
        /// 获得Buffs
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryGetBuffs<T>(in List<T> list) where T : BaseBuff<TBuff>
        {
            list.Clear();
            foreach (var buff in buffList)
            {
                if (buff is T baseBuff)
                {
                    list.Add(baseBuff);
                }
            }
            return list.Count > 0;
        }
        /// <summary>
        /// 清空当前Buff
        /// </summary>
        public void ClearBuff()
        {
            foreach (var buff in buffList)
            {
                _removeBuffList.Add(buff);
            }
        }
        /// <summary>
        /// 移除指定类型Buff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveBuff<T>() where T : BaseBuff<TBuff>
        {
            if (!AllowRemoveBuff<T>())
            {
                OnRemoveBuffError<T>();
                return;
            }
            foreach (var buff in buffList)
            {
                if (buff is T)
                {
                    _removeBuffList.Add(buff);
                }
            }
        }
        
        /// <summary>
        /// 添加Buff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddBuff<T>() where T : BaseBuff<TBuff>
        {
            if (!AllowAddBuff<T>())
            {
                OnAddBuffError<T>();
                return;
            }
            T buff = System.Activator.CreateInstance<T>();
            buff.central = this as TBuff;
            buff.OnInit();
            _addBuffList.Add(buff);
        }

        private void Update()
        {
            foreach (var buff in buffList)
            {
                if(buff.Active)
                    buff.OnUpdate(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            foreach (var buff in _addBuffList)
            {
                buff.Active = true;
                buffList.Add(buff);
            }
            _addBuffList.Clear();
            foreach (var buff in _removeBuffList)
            { 
                var removeSuccess = buffList.Remove(buff);
                if(removeSuccess)
                    buff.Active = false;
            }
            _removeBuffList.Clear();
        }
        /// <summary>
        /// 判断是否允许移除Buff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool AllowRemoveBuff<T> () where T : BaseBuff<TBuff>
        {
            return true;
        }
        /// <summary>
        /// 判断是否允许添加Buff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual bool AllowAddBuff<T>() where T : BaseBuff<TBuff>
        {
            return true;
        }
        
        protected virtual void OnAddBuffError<T>() where T : BaseBuff<TBuff>
        {
            Debug.LogError($"Add Buff Error:{typeof(T)}");
        }
        protected virtual void OnRemoveBuffError<T>() where T : BaseBuff<TBuff>
        {
            Debug.LogError($"Remove Buff Error:{typeof(T)}");
        }
    }
}

