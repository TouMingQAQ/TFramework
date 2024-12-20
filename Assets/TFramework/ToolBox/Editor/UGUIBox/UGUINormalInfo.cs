using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using TwoPaneSplitView = TFramework.ToolBox.UIToolKit.TwoPaneSplitView;

namespace TFramework.ToolBox
{
    public class UGUINormalInfo : UGUIBox.UGUITool
    {
        public override string TabName => "一般信息";
        private Label uiInfo;
        private ScrollView buttonView;
        public override void Init()
        {
            this.style.flexGrow = 1;
            VisualElement root = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    paddingTop = 10,
                    paddingLeft = 5,
                    paddingRight = 5
                }
            };
            VisualElement leftPanel = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 0,
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 5,
                    paddingBottom = 5
                }
            };
            VisualElement rightPanel = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1,
                    paddingLeft = 10,
                    paddingRight = 10,
                    paddingTop = 5,
                    paddingBottom = 5
                }
            };
            root.Add(leftPanel);
            root.Add(rightPanel);
            uiInfo = new Label("Have no selected UI");
            leftPanel.Add(uiInfo);
            buttonView = new ScrollView()
            {
                style =
                {
                    flexGrow = 1
                },
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            rightPanel.Add(buttonView);
            NewLine();
            AddButton("同步UI锚点",AdaptUGUI);
            AddButton("适应UI锚点",AdaptAnchoredUGUI);
            Add(root);
        }
        private Toolbar _toolbar;
        void AddButton(string buttonName,Action onClick)
        {
            ToolbarButton button = new ToolbarButton(onClick)
            {
                text = buttonName
            };
            _toolbar.Add(button);
        }
        void NewLine()
        {
            _toolbar = new Toolbar();
            buttonView.Add(_toolbar);
        }

        void AdaptUGUI()
        {
            if((object)selectRectTransform == null)
                return;
            Undo.RecordObject(selectRectTransform,"");
            Adapt(selectRectTransform);
        }

        void AdaptAnchoredUGUI()
        {
            if((object)selectRectTransform == null)
                return;
            Undo.RecordObject(selectRectTransform,"");
            AdaptAnchored(selectRectTransform);
        }

        public static string GetRectTransformInfo(RectTransform rectTransform)
        {
            if ((object)rectTransform == null)
            {
                return "Select rectTransform is null";
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name:{rectTransform.gameObject.name}");
            sb.AppendLine($"Pivot:{rectTransform.pivot}");
            sb.AppendLine($"AnchoredPosition:{rectTransform.anchoredPosition}");
            sb.AppendLine($"AnchoredPosition3D:{rectTransform.anchoredPosition3D}");
            sb.AppendLine($"AnchorMax:{rectTransform.anchorMax}");
            sb.AppendLine($"AnchorMin:{rectTransform.anchorMin}");
            sb.AppendLine($"SizeDelta:{rectTransform.sizeDelta}");
            sb.AppendLine($"OffsetMax:{rectTransform.offsetMax}");
            sb.AppendLine($"OffsetMin:{rectTransform.offsetMin}");
            return sb.ToString();
        }
        
        protected override void OnSelectionChanged(RectTransform rectTransform)
        {
            uiInfo.text = GetRectTransformInfo(rectTransform);
        }
        private static void AdaptAnchored(RectTransform localRect)
        {
            localRect.anchoredPosition = Vector2.zero;
        }
        private static void Adapt(RectTransform go)
        {
            //位置信息
            Vector3 partentPos = go.transform.parent.position;
            Vector3 localPos = go.transform.position;
            //------获取rectTransform----
            RectTransform partentRect = go.transform.parent.GetComponent<RectTransform>();
            // RectTransform go = go.GetComponent<RectTransform>();
            float partentWidth = partentRect.rect.width;
            float partentHeight = partentRect.rect.height;
            float localWidth = go.rect.width * 0.5f;
            float localHeight = go.rect.height * 0.5f;
            //---------位移差------
            float offX = localPos.x - partentPos.x;
            float offY = localPos.y - partentPos.y;
 
            float rateW = offX / partentWidth;
            float rateH = offY / partentHeight;
            go.anchorMax = go.anchorMin = new Vector2(0.5f + rateW, 0.5f + rateH);
            go.anchoredPosition = Vector2.zero;
 
            partentHeight = partentHeight * 0.5f;
            partentWidth = partentWidth * 0.5f;
            float rateX = (localWidth / partentWidth) * 0.5f;
            float rateY = (localHeight / partentHeight) * 0.5f;
            go.anchorMax = new Vector2(go.anchorMax.x + rateX, go.anchorMax.y + rateY);
            go.anchorMin = new Vector2(go.anchorMin.x - rateX, go.anchorMin.y - rateY);
            go.offsetMax = go.offsetMin = Vector2.zero;
        }

    
    }
    
}