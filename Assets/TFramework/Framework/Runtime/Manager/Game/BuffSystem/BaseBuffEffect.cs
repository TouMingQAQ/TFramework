using System;
using UnityEngine;

namespace TFramework.Runtime.Buff
{
    public abstract class BaseBuffEffect
    {
        public BaseBuffData BaseBuffData { get; set; } = null;
        public virtual void OnEnable(BaseBuffData buffdata){}
        public virtual void OnDisable(BaseBuffData baseBuffData){}
        public virtual void OnUpdate(float deltaTime,BaseBuffData buffdata){}//Buff更新
        
        public virtual void Effect(){}
    }

    public abstract class BuffEffect<T> : BaseBuffEffect where T : BuffEffect<T>
    {
        public override void Effect()
        {
            BaseBuffData.System.Effect(this as T);
        }
    }

}