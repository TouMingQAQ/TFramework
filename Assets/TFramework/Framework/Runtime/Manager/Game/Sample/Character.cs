using System;
using System.Collections.Generic;
using System.Text;
using TFramework.Runtime.Buff;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HpUpEffect : BaseBuffEffect
{
    public override void OnEnable(BaseBuffData buffdata)
    {
        base.OnEnable(buffdata);
        Effect();
    }

    public override void OnDisable(BaseBuffData baseBuffData)
    {
        base.OnDisable(baseBuffData);
        Effect();
    }
}
public class Character : MonoBehaviour
{
    public BuffSystem buffSystem = new BuffSystem();
    public BuffSystem globalBuffSystem = new BuffSystem();
    public BufferControl control = new();
    public string Name;
    public float Hp;
    public float MaxHp;
    public float Atk;
    public float Def;

    public TMP_Text nameText;
    public Slider hpSlider;
    public TMP_Text atkText;
    public TMP_Text defText;
    public TMP_Text buffText;

    private void Awake()
    {
        buffSystem.Init();
        globalBuffSystem.Init();
       
        
        control.AddEffect<MaxHpBuffEffect>(OnMaxHpBuffEffect);
        control.AddEffect<HealHpEffect>(OnHealHpEffect);
        control.AddEffect<DamageEffect>(OnDamageEffect);
    }



    #region Effect

    private void OnEnable()
    {
        control.AddBuffSystem(buffSystem);
        control.AddBuffSystem(globalBuffSystem);
    }

    private void OnDisable()
    {
        control.ClearBuffSystem();
    }
    void OnMaxHpBuffEffect(MaxHpBuffEffect effect)
    {
        MaxHp += effect.MaxHpChange;
        Hp = Mathf.Clamp(Hp,0, MaxHp);
    }

    void OnHealHpEffect(HealHpEffect effect)
    {
        HealHp(effect.HealHp);
    }
    void OnDamageEffect(DamageEffect effect)
    {
        Damage(effect.DamageHp);   
    }
    #endregion



    public void HealHp(float heal)
    {
        heal = Mathf.Max(0, heal);
        Hp += heal;
        Hp = Mathf.Clamp(Hp,0, MaxHp);
    }

    public void Damage(float atk)
    {
        if(Hp <= 0)
            return;
        atk = Mathf.Max(0, atk);
        var atkHp = Mathf.Max(0.1f, (atk - Def));
        Debug.Log($"{Name} take damage=>{atkHp}");
        Hp -= atkHp;
        Hp = Mathf.Clamp(Hp,0, MaxHp);
    }
    
    public void _10秒真男人()
    {
        globalBuffSystem.AddBuff(new HealAndMaxHp("10S`God"));
    }

    public void ClearBuff()
    {
        buffSystem.AddBuff(new Clear("Clear"));
    }

    public void DamageBuff()
    {
        buffSystem.AddBuff(new Damage("Damage",10));
    }

    public void ClearOnTimeBuff()
    {
        buffSystem.AddBuff(new ClearOnTime("ClearOnTimeBuff",2));
    }

    private void Update()
    {
        buffSystem.Update(Time.deltaTime);
        globalBuffSystem.Update(Time.deltaTime);
    }

    private List<BaseBuffData> buffList = new();
    private void FixedUpdate()
    {
        nameText.text = Name;
        hpSlider.value = Hp / MaxHp;
        atkText.text = $"Atk:{Atk}";
        defText.text = $"Def:{Def}";

        control.QueryBuff(x => true, buffList);
        StringBuilder sb = new();
        foreach (var buffData in buffList)
        {
            var ID = buffData.BuffID;
            sb.AppendLine($"[<color=green>{ID}</color>][{buffData.Order}_{buffData.Active}");
        }
        buffText.SetText(sb);
    }
}
