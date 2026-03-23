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


#endregion

#region BuffList
/// <summary>
/// 10s真男人
/// </summary>
public class HealAndMaxHp : TimeBuff
{
    public HealAndMaxHp(string id, int order = -1, bool defaultActive = false) : base(id,10, order, defaultActive)
    {
        AddEffect(new HealHpEffect(200f,0.3f));
        AddEffect(new MaxHpBuffEffect(1000));
    }
}

#endregion
