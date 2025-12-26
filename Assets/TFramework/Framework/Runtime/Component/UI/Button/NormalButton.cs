using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using VInspector;

namespace TFramework.Component.UI
{
    public class NormalButton : UIBehaviour,IPointerClickHandler,IPointerDownHandler,IPointerUpHandler
    {
        public bool ignoredTime = true;
        #region 点击
        [Tab("Click")]
        public int multiClickCount = 2;
        public float multiClickDuration = 0.2f;

        private int multiClickTimer = 0;
        private float multiClickDurationTimer = 0;
        
        /// <summary>
        /// 单次点击
        /// </summary>
        public UnityEvent onClick;
        /// <summary>
        /// 单次左键点击
        /// </summary>
        public UnityEvent onLeftClick;
        /// <summary>
        /// 单次右键点击
        /// </summary>
        public UnityEvent onRightClick;
        /// <summary>
        /// 多次点击
        /// </summary>
        public UnityEvent onMultiClick;

        #endregion
        
        #region 长按

        
        [SerializeField,ReadOnly,Tab("Press")]
        private bool isPressing = false;
        [SerializeField,ReadOnly]
        private float pointPressTimer = 0;
        [SerializeField,ReadOnly]
        private bool isPressPointDown = false;
        /// <summary>
        /// 长按触发的最短时间
        /// </summary>
        public float minPressDuration = 0.1f;
        /// <summary>
        /// 长按目标时间
        /// 当其值小于等于0时，OnPressHold返回的是当前按下持续的时间
        /// 反之是百分比值
        /// </summary>
        public float pressEndTime = -1;
        /// <summary>
        /// 按压开始
        /// </summary>
        public UnityEvent onPressStart;
        /// <summary>
        /// 按压持续中
        /// </summary>
        public UnityEvent<float> onPressHold;
        /// <summary>
        /// 按压释放
        /// </summary>
        public UnityEvent onPressEnd;
        #endregion


     
        private void LateUpdate()
        {
            PressUpdate();
            MultiClickUpdate();
        }

        void MultiClickUpdate()
        {
            if (multiClickDurationTimer > 0)
            {
                var deltaTime = ignoredTime ? Time.unscaledDeltaTime : Time.deltaTime;
                multiClickDurationTimer -= deltaTime;
            }
            else
            {
                multiClickTimer = 0;
            }
        }

        void ResetPress()
        {
            isPressing = false;
            pointPressTimer = 0;
        }
        void PressUpdate()
        {
            if (isPressPointDown)
            {
                var deltaTime = ignoredTime ? Time.unscaledDeltaTime : Time.deltaTime;
                if (pointPressTimer >= minPressDuration)
                {
                    var currentValue = pointPressTimer - minPressDuration;

                    if (!isPressing)
                    {
                        isPressing = true;
                        onPressStart?.Invoke();
                    }
                    if (pressEndTime > 0)
                    {
                        if(currentValue >= pressEndTime)
                            onPressHold?.Invoke(1);
                        else
                            onPressHold?.Invoke(currentValue/pressEndTime);
                    }
                    else
                    {
                        onPressHold?.Invoke(currentValue);
                    }
                }
                pointPressTimer += deltaTime;
            }
            else
            {
                if (isPressing)
                { 
                    onPressEnd?.Invoke();
                }
                ResetPress();
                pointPressTimer = 0;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(isPressing)
                return;//按压中不触发Click
            onClick?.Invoke();
            if(eventData.button == PointerEventData.InputButton.Left)
                onLeftClick?.Invoke();
            else if(eventData.button == PointerEventData.InputButton.Right)
                onRightClick?.Invoke();
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                isPressPointDown = true;
            multiClickTimer++;
            if (multiClickTimer >= multiClickCount)
            {
                onMultiClick?.Invoke();
                multiClickTimer = 0;
                multiClickDurationTimer = 0;
            }
            else
            {
                multiClickDurationTimer = multiClickDuration;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
                isPressPointDown = false;
        }
    }
}
