using UnityEngine;

namespace TFramework.BuffSystem
{
    public abstract class BaseBuff<T> where T : BuffCentral<T>
    {
        private bool _active = false;
        public T central;

        public bool Active
        {
            get => _active;
            set
            {
                if (_active.Equals(value))
                    return;
                _active = value;
                if (_active)
                {
                    Debug.Log($"[{central.name}]开启Buff:{GetType().Name}");
                    OnEnable();
                }
                else
                {
                    Debug.Log($"[{central.name}]关闭Buff:{GetType().Name}");
                    OnDisable();
                }
            }
        }
        public virtual void OnInit(){}
        protected virtual void OnEnable(){}
        protected virtual void OnDisable(){}
        public virtual void OnUpdate(float deltaTime){}
    }
   
}

