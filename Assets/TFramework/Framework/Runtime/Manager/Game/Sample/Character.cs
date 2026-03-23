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
    }



    #region Effect

    private void OnEnable()
    {
        buffSystem.RegisterEffect<MaxHpBuffEffect>(OnMaxHpBuffEffect);
        buffSystem.RegisterEffect<HealHpEffect>(OnHealHpEffect);
    }

    private void OnDisable()
    {
        buffSystem.UnRegisterEffect<MaxHpBuffEffect>(OnMaxHpBuffEffect);
        buffSystem.UnRegisterEffect<HealHpEffect>(OnHealHpEffect);
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
        var atkHp = Mathf.Max(1, (atk - Def));
        Debug.Log($"{Name} take damage=>{atkHp}");
        Hp -= atkHp;
        Hp = Mathf.Clamp(Hp,0, MaxHp);
    }
    
    public void _10秒真男人()
    {
        buffSystem.AddBuff(new HealAndMaxHp("10秒真男人"));
    }

    private void Update()
    {
        buffSystem.Update(Time.deltaTime);
    }

    private List<BaseBuffData> buffList = new();
    private void FixedUpdate()
    {
        nameText.text = Name;
        hpSlider.value = Hp / MaxHp;
        atkText.text = $"Atk:{Atk}";
        defText.text = $"Def:{Def}";

        buffSystem.QueryBuff(x => true, in buffList);
        StringBuilder sb = new();
        foreach (var buffData in buffList)
        {
            var ID = buffData.BuffID;
            sb.AppendLine($"[<color=green>{ID}</color>][{buffData.Order}_{buffData.Active}");
        }
        buffText.SetText(sb);
    }
}
