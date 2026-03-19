namespace TFramework.Runtime.Buff
{
    /// <summary>
    /// 层级Buff
    /// </summary>
    public class LevelBuff : BuffData
    {
        public LevelBuff(string id, int order = -1, bool defaultActive = true) : base(id, order, defaultActive)
        {
        }

        public override bool NeedUpdate => false;
    }
}