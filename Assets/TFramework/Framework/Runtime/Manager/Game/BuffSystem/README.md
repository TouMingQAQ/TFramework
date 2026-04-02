# BuffSystem

> TFramework 游戏 Buff/状态效果系统
>
> **命名空间**：`TFramework.Runtime.Buff`
> **程序集**：`TFramework.Buff.asmdef`

---

## 目录

- [架构总览](#架构总览)
- [核心概念](#核心概念)
- [快速上手](#快速上手)
- [BufferControl](#buffercontrol)
- [BuffSystem](#buffsystem)
- [BaseBuffData](#basebuffdata)
- [BaseBuffEffect](#basebuffeffect)
- [EventNode 事件系统](#eventnode-事件系统)
- [Buff 生命周期](#buff-生命周期)
- [设计模式说明](#设计模式说明)
- [注意事项](#注意事项)

---

## 架构总览

```
BufferControl ──(1:n)──▶ BuffSystem ──(1:n)──▶ BaseBuffData ──(1:n)──▶ BaseBuffEffect
                              │                      │
                        两个 EventNode           effectList
                        • m_Event              (OnEnable /
                        • m_EffectEvent         OnDisable /
                                              OnUpdate /
                                              Effect)
```

**层级职责**

| 类 | 职责 |
|---|---|
| **BufferControl** | 总控制器，管理多个 BuffSystem；统一广播 Effect 和 CallEvent |
| **BuffSystem** | Buff 容器，驱动每帧 Tick；持有两个 EventNode |
| **BaseBuffData** | 单个 Buff 的数据单元，管理生命周期与叠层 |
| **BaseBuffEffect** | 单个 Buff 效果，执行实际逻辑 |

---

## 核心概念

### BuffID

Buff 的唯一标识符。同一个 `BuffSystem` 中，相同 `BuffID` 的 Buff 会进行**叠层处理**（而非创建新实例）。

### Order

更新优先级，数值越小越先执行。用于 `BakeExecuteBuff()` 排序。

### Active

激活状态。只有 `Active == true` 且 `NeedUpdate == true` 的 Buff 才会参与 Tick 更新。

### Effect

双向通信机制：
- **Buff → 外部**：通过 `BuffEffect<T>.Effect()` 发射泛型事件，外部订阅者收到回调
- **外部 → Buff**：外部通过 `CallEvent<T>()` 向 EventNode 发送事件，BuffData 内部注册对应 Action 响应

---

## 快速上手

### 1. 定义 Effect（效果事件）

定义一个 C# 类实现 `IBuffEvent`，作为事件载体：

```csharp
public class DamageOverTimeEffect : IBuffEvent
{
    public float DamagePerSecond;
    public float Duration;
}
```

### 2. 定义 Effect 执行器

继承 `BuffEffect<T>`，在 `OnUpdate` 中写入实际逻辑：

```csharp
public class DOTEffect : BuffEffect<DamageOverTimeEffect>
{
    private float elapsed;

    public override void OnEnable(DamageOverTimeEffect data) { /* 生效时 */ }
    public override void OnDisable(DamageOverTimeEffect data) { /* 移除时 */ }
    public override void OnUpdate(float deltaTime, DamageOverTimeEffect data)
    {
        elapsed += deltaTime;
        if (elapsed >= data.Duration)
            BaseBuffData.Remove(); // 持续时间结束，移除 Buff
    }
}
```

### 3. 定义 BuffData

继承 `BuffData<T>`，组装 Effect 列表：

```csharp
public class PoisonDebuff : BuffData<PoisonDebuff>
{
    public PoisonDebuff() : base("PoisonDebuff", order: 10) { }

    public float Damage = 10f;
    public float Duration = 5f;

    protected override void OnInit()
    {
        // 初始化时组装 effectList
        var dot = new DOTEffect();
        var data = new DamageOverTimeEffect
        {
            DamagePerSecond = Damage,
            Duration = Duration
        };
        AddEffect(dot);
    }

    protected override void OnAddBuff(PoisonDebuff other)
    {
        // 叠层：刷新持续时间
        Duration = other.Duration;
        Damage = other.Damage;
    }
}
```

### 4. 初始化并使用

```csharp
// 1. 创建并初始化 BuffSystem
var bs = new BuffSystem();
bs.Init();

// 2. 添加到 BufferControl（统一管理多系统）
var ctrl = new BufferControl();
ctrl.AddBuffSystem(bs);

// 3. 注册 Effect 回调（外部订阅）
ctrl.AddEffect<DamageOverTimeEffect>(OnDOT);

// 4. 添加 Buff
var poison = new PoisonDebuff();
bs.AddBuff(poison, defaultEnable: true);

// 5. 每帧 Tick
void Update() => bs.Update(Time.deltaTime);

// 6. 移除 Buff
bs.RemoveBuff("PoisonDebuff");
```

---

## BufferControl

`BufferControl` 是总控制器，一对多管理多个 `BuffSystem` 实例。

```csharp
public class BufferControl
{
    // 添加 / 移除 BuffSystem
    public void AddBuffSystem(BuffSystem system);
    public void RemoveBuffSystem(BuffSystem system);

    // 注册 Effect 回调，广播到所有 BuffSystem
    public void AddEffect<T>(Action<T> action) where T : BuffEffect<T>;

    // 向所有 BuffSystem 发送事件
    public void CallEvent<T>(T value);

    // 查询所有系统中满足条件的 Buff
    public void QueryBuff(Predicate<BaseBuffData> comparable, List<BaseBuffData> dataList);
}
```

### 典型用法

```csharp
// 全局单例 BufferControl，所有子系统共享
public static BufferControl G { get; private set; }

void Start()
{
    G = new BufferControl();
    G.AddBuffSystem(PlayerBuffSystem);
    G.AddBuffSystem(EnemyBuffSystem);

    // 注册一个 Effect 处理器，同时监听所有系统
    G.AddEffect<DamageOverTimeEffect>(OnDOT);
}
```

---

## BuffSystem

`BuffSystem` 是 Buff 容器，绑定在具体的游戏实体上。

```csharp
public class BuffSystem
{
    // 生命周期
    public void Init();
    public void Update(float deltaTime);   // 每帧调用

    // Buff 操作
    public void AddBuff(BaseBuffData data, bool defaultEnable = true, bool bakeOrder = true);
    public void AddBuff(IEnumerable<BaseBuffData> dataList, bool defaultEnable = true);
    public void RemoveBuff(string buffID);
    public void RemoveBuff(Predicate<BaseBuffData> comparable, bool bakeOrder = true);

    // 查询
    public bool AnyBuff(Predicate<BaseBuffData> comparable);
    public void QueryBuff(Predicate<BaseBuffData> comparable, List<BaseBuffData> dataList);
    public void QueryBuff(Predicate<BaseBuffData> comparable, Action<BaseBuffData> onComparable);

    // 事件（仅供内部/子类使用）
    public void CallEvent<T>(T value);                        // 向内部的 EventNode 发事件
    public EventNode RegisterEffect<T>(Action<T> action);     // 注册 Effect 回调
}
```

> **注意**：`AddBuff` 的 `bakeOrder` 参数控制是否立即重建排序列表。批量添加时传 `false`，添加完再调一次 `BakeExecuteBuff()`（`protected`，需子类暴露）会更高效。

---

## BaseBuffData

```csharp
public abstract class BaseBuffData
{
    public string BuffID;       // 唯一标识（用于叠层）
    public int Order;           // 更新优先级
    public bool Active;         // 激活状态
    public bool NeedUpdate;     // 是否参与 Tick，默认 true
    public BuffSystem System;   // 所属 System（注入）

    // 生命周期
    public void SetActive(bool active, bool forceEvent = false);
    protected virtual void OnEnable();   // 激活时调用
    protected virtual void OnDisable();  // 停用时调用
    public virtual void OnUpdate(float deltaTime);

    // 叠层
    public virtual void AddBuff(BaseBuffData data);

    // 主动触发效果
    public virtual void Effect();

    // 工具
    public void AddEffect(BaseBuffEffect effect);
    public void Remove();  // 从 System 中移除自身
}
```

### 泛型约束版本

```csharp
public abstract class BuffData<T> : BaseBuffData where T : BuffData<T>
{
    protected abstract void OnAddBuff(T buffData);  // 子类实现叠层逻辑
}
```

---

## BaseBuffEffect

```csharp
public abstract class BaseBuffEffect
{
    public BaseBuffData BaseBuffData { get; set; }  // 所属 BuffData

    public virtual void OnEnable(BaseBuffData buffdata);
    public virtual void OnDisable(BaseBuffData buffdata);
    public virtual void OnUpdate(float deltaTime, BaseBuffData buffdata);
    public virtual void Effect();
}
```

### 泛型事件版本

```csharp
public abstract class BuffEffect<T> : BaseBuffEffect where T : BuffEffect<T>
{
    public override void Effect()
    {
        BaseBuffData.System.Effect(this as T);  // → EffectEvent.Call(T) → 外部订阅者
    }
}
```

---

## EventNode 事件系统

`EventNode` 是一个轻量级、类型安全的事件总线，底层基于 `Dictionary<Type, Delegate>`。

```csharp
public partial class EventNode
{
    // 注册 / 注销（返回 IEventHandle，支持链式）
    public IEventHandle Register<T>(Action<T> action);
    public void UnRegister<T>(Action<T> action);

    // 发送事件
    public bool Call<T>(T value);  // 有订阅者返回 true，否则 false

    // Handle 级注册（BufferControl 内部使用）
    public void RegisterHandle(IEventHandle handle);
    public void UnRegisterHandle(IEventHandle handle);

    public void Clear();
}

public interface IEventHandle
{
    EventNode Node { get; set; }
    Delegate Action { get; set; }
    Type EventType { get; set; }
    void UnRegister();   // 自动从 Node 注销
    void Register();     // 自动向 Node 注册
    bool Call(params object[] objects);
}
```

---

## Buff 生命周期

```
AddBuff()
  └─ data.System = this
  └─ data.Reset()
  └─ buffMap[id] ? → AddBuff() 叠层 : 新增
  └─ BakeExecuteBuff()  ── 重建列表，按 Order 排序
  └─ data.SetActive(true)
        └─ OnEnable()
              └─ effectList[*].OnEnable()

每帧 Update(dt)
  └─ 遍历 m_updateBuffDataListCache（已排好序）
        └─ if (Active && NeedUpdate)
              └─ buffData.OnUpdate(dt)
                    └─ effectList[*].OnUpdate(dt)

RemoveBuff(id) 或 buffData.Remove()
  └─ buffMap.Remove(id)
  └─ SetActive(false)
        └─ OnDisable()
              └─ effectList[*].OnDisable()
  └─ BakeExecuteBuff()
```

---

## 设计模式说明

### 模板方法模式

`BaseBuffData.OnEnable()`、`OnDisable()`、`OnUpdate()` 对 `effectList` 的遍历是模板方法；子类可以通过重写这些方法在遍历前后插入自定义逻辑。

### 组合模式

`BaseBuffData` 持有 `List<BaseBuffEffect>`，一个 Buff 可以包含多个 Effect（攻击加成 + 移速加成 + 特效表现），各 Effect 独立管理自己的生命周期。

### 观察者模式

`BuffEffect<T>.Effect()` 通过 `EventNode` 向外部广播事件，外部订阅者以 `Action<T>` 回调响应，两者完全解耦。

### 对象池友好

`BuffSystem` 的内部容器设计允许：
- Buff 被移除后，新 Buff 复用相同 `BuffID` 槽位
- `m_updateBuffDataListCache` 避免遍历中修改集合

### 叠层策略模式

`AddBuff` 和 `BuffData<T>.OnAddBuff` 是策略模式的体现，子类自行决定如何处理叠层：刷新持续时间、叠加层数、最大上限、或完全忽略新 Buff。

---

## 注意事项

1. **BakeExecuteBuff 不自动调用**：单次 `AddBuff` 后默认会触发排序；批量添加时请传 `bakeOrder=false`，添加完毕后再排序。
2. **Update 中的集合修改**：`Update` 使用 `m_updateBuffDataListCache` 副本遍历，避免了 `m_updateBuffDataList` 在遍历中被修改的问题。但 Effect 的 `OnUpdate` 中**不要直接调用 `RemoveBuff`**（会修改原集合），应通过 `buffData.Remove()` 排队等待。
3. **Effect 事件的订阅时机**：`AddEffect` 与 `AddBuffSystem` 的调用顺序**不受限制**。`AddEffect` 注册时会遍历所有已加入的 System；`AddBuffSystem` 时也会把已有 Handle 批量注入新 System。两种写法均可。
4. **BuffData.Remove() 是异步的**：`Remove()` 实际调用 `System.RemoveBuff(BuffID)`，会触发 `SetActive(false)` 和 `BakeExecuteBuff()`，Buff 不会在本帧内立即从列表中消失。
5. **EventNode.Call 返回 bool**：用于判断是否有订阅者处理该事件，无订阅时返回 `false`（系统会输出一句 Warning）。
