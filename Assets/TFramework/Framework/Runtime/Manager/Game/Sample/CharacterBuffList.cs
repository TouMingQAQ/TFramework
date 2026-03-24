using System;
using TFramework.Runtime.Buff;
using TFramework.Runtime.Sample;
using UnityEngine;

#region EffectList

public class TimerEffect<T> : BuffEffect<T> where T : BuffEffect<T>
{
    private float effectTimer;
    private float effectDuration;
    public TimerEffect(float effectDuration)
    {
        this.effectDuration = Mathf.Max(effectDuration, Time.deltaTime);
        effectTimer = this.effectDuration;
    }
    public override void OnUpdate(float deltaTime, BaseBuffData buffdata)
    {
        if(effectDuration <= 0)
            return;
        if (effectTimer > 0)
        {
            effectTimer -= deltaTime;
        }
        else
        {
            effectTimer = effectDuration;
            Effect();
        }
    }
}
public class MaxHpBuffEffect : BuffEffect<MaxHpBuffEffect>
{
    private float maxHpChange;
    public float MaxHpChange;

    public MaxHpBuffEffect(float maxHpChange)
    {
        this.maxHpChange = maxHpChange;
    }
    public override void OnEnable(BaseBuffData buffdata)
    {
        MaxHpChange = maxHpChange;
        Effect();

    }

    public override void OnDisable(BaseBuffData baseBuffData)
    {
        MaxHpChange = -maxHpChange;
        Effect();
    }
}


public class HealHpEffect : TimerEffect<HealHpEffect>
{
    public float HealHp = 0;
    public HealHpEffect(float healHp,float effectDuration) : base(effectDuration)
    {
        HealHp = Mathf.Max(0, healHp);
    }
}

public class DamageEffect : TimerEffect<DamageEffect>
{
    public float DamageHp { get; set; }
    public DamageEffect(float damageHp,float effectDuration) : base(effectDuration)
    {
        DamageHp = damageHp;
    }
}


#endregion

#region BuffList
/// <summary>
/// 10s真男人
/// </summary>
public class HealAndMaxHp : TimeBuff<HealAndMaxHp>
{
    public HealAndMaxHp(string id, int order = -1, bool defaultActive = false) : base(id,10, order, defaultActive)
    {
        AddEffect(new HealHpEffect(200f,0.3f));
        AddEffect(new MaxHpBuffEffect(1000));
    }
}
public interface IDamageBuff{}//攻击类Buff标签
public class Damage : TimeBuff<Damage>,IDamageBuff
{
    public override Type Type => typeof(Damage);
    public Damage(string id, float time, int order = -1, bool defaultActive = true) : base(id, time, order, defaultActive)
    {
        AddEffect(new DamageEffect(300,1f));
    }
}

public class ClearEffect : BaseBuffEffect
{
    public override void Effect()
    {
        BaseBuffData.System.QueryBuff(x=>x is ITimeBuff,x=>(x as ITimeBuff)?.ReStart());//重置所有TimeBuff
        
        BaseBuffData.System.RemoveBuff(x=>x is IDamageBuff);//移除所有DamageBuff
    }
}
/// <summary>
/// 一次性Clear
/// </summary>
public class Clear : OnceBuff
{
    public Clear(string id, int order = -1, bool defaultActive = false) : base(id, order, defaultActive)
    {
        AddEffect(new ClearEffect());
    }
}
/// <summary>
/// 倒计时Clear
/// </summary>
public class ClearOnTime : TimeBuff<ClearOnTime>
{
    public ClearOnTime(string id, float time, int order = -1, bool defaultActive = true) : base(id, time, order, defaultActive)
    {
        AddEffect(new ClearEffect());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Effect();
    }
}



#endregion
