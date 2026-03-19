namespace TFramework.Runtime.Buff
{
    /// <summary>
    /// 回合Buff
    /// </summary>
    public class RoundBuff : BuffData
    {
        public RoundBuff(string id, int order = -1, bool defaultActive = true) : base(id, order, defaultActive)
        {
        }

        public override bool NeedUpdate => false;
    }
}