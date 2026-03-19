namespace TFramework.Runtime.Buff
{
    /// <summary>
    /// 标签Buff
    /// </summary>
    public class TagBuff : BuffData
    {
        public TagBuff(string id, int order = -1, bool defaultActive = true) : base(id, order, defaultActive)
        {
            
        }

        public override bool NeedUpdate => false;
    }
}