using TFramework.Runtime.Buff;

namespace TFramework.Runtime.Sample
{
    public class TimeBuff : BuffData<TimeBuff>
    {
        public float Time { get; set; }
        protected float m_timer { get; set; }

        public TimeBuff(string id,float time, int order = -1, bool defaultActive = true) : base(id, order, defaultActive)
        {
            this.Time = time;
            m_timer = time;
        }

        public override void Reset()
        {
            base.Reset();
            m_timer = Time;
        }

        public override bool NeedUpdate => true;
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if(!Active)
                return;
            if (m_timer <= 0)
            {
                Active = false;
                Remove();
                return;
            }
            else
            {
                m_timer -= deltaTime;
            }
        }

        protected override void OnAddBuff(TimeBuff buffData)
        {
            //相同计时类Buff，时间长的更好
            if (buffData.Time > this.Time)
            {
                var disTime = buffData.Time - this.Time;
                m_timer += disTime;
                this.Time = buffData.Time;
            }
            else
            {
                if (buffData.Time > this.m_timer)
                    this.m_timer = buffData.Time;
            }
        }
    }
}