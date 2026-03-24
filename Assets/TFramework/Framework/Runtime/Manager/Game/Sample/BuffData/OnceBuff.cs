using TFramework.Runtime.Buff;

namespace TFramework.Runtime.Sample
{
    /// <summary>
    /// 一次性Buff
    /// </summary>
    public class OnceBuff : BuffData<OnceBuff>
    {
        public OnceBuff(string id, int order = -1, bool defaultActive = false) : base(id, order, defaultActive)
        {
        }
        protected override void OnEnable()
        {
            //激活并移除
            Effect();
            Remove();
        }
        protected override void OnAddBuff(OnceBuff buffData)
        {
            
        }
    }
}