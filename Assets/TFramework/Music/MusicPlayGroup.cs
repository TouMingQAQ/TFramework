using System.Collections.Generic;
using UnityEngine;
using Debug = Console.Command.Debug;

namespace TFramework.Music
{
    public abstract class MusicPlayGroup<T,TInfo> : MonoBehaviour where TInfo : MusicInfo where T : MusicPlay<TInfo>
    {
        [SerializeField]
        protected int currentIndex = 0;
        [SerializeField]
        protected T play;
        [SerializeField]
        protected List<TInfo> infoGroup = new();
        /// <summary>
        /// 下一首
        /// </summary>
        public void Next()
        {
            Play(currentIndex+1);
        }
        /// <summary>
        /// 上一首
        /// </summary>
        public void Last()
        {
            Play(currentIndex-1);
        }

        public void LoadGroup(IEnumerable<TInfo> group, bool autoPlay = true)
        {
            infoGroup.Clear();
            infoGroup.AddRange(group);
            currentIndex = 0;
            if (autoPlay)
            {
                Play(currentIndex);
            }
            else
            {
                Pause();
            }
        }

        public void Play(int index)
        {
            if(index < 0 || index >= infoGroup.Count)
                return;
            if(infoGroup.Count <= 0)
                return;
            var info = infoGroup[index];
            if (info == null)
            {
                Debug.LogError("PlayInfo is Null");
                return;
            }

            currentIndex = index;
            play.LoadMusic(info);
            Play();
        }

        public virtual void Play() => play.Play();
        public virtual void Pause() => play.Pause();
        public virtual void ReStart() => play.ReStart();
        public virtual void SwitchMute() => play.SwitchMute();

        public virtual string MusicName => play.MusicName;
        public virtual string MusicWriter => play.MusicWriter;
        public virtual Sprite MusicCover => play.MusicCover;
        
        public virtual float Progress
        {
            get => play.PlayProgress;
            set => play.PlayProgress = value;
        }

        public virtual float Volume
        {
            get => play.Volume;
            set => play.Volume = value;
        }

        public float CurrentTime => play.CurrentTime;
        public float TotalTime => play.TotalTime;
        public string CurrentTimeStr => CoverTime(CurrentTime);
        public string TotalTimeStr => CoverTime(TotalTime);

        protected string CoverTime(float second)=>$"{Mathf.FloorToInt(second / 60):00}:{Mathf.FloorToInt(second % 60):00}";
    }
}

