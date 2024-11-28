using System;
using System.Collections.Generic;
using TFramework.Console;
using TFramework.ToolBox.UIToolkitKit;

#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
#if UNITY_EDITOR
    public class ConsoleWindow : EditorWindow
    {
        [MenuItem("TFramework/ToolBox/Console")]
        public static void OpenConsoleWindow()
        {
            EditorWindow wnd = EditorWindow.GetWindow<ConsoleWindow>();
            wnd.titleContent = new GUIContent("控制台");
        }

        private void CreateGUI()
        {
            rootVisualElement.Add(new ConsoleBox());
        }
    }
    
#endif
    public class ConsoleBox : BaseToolBox
    {
        public override bool Closeable => false;
        public override string TabName => "控制台";
        public override int PreLoadIndex => BasePreLoadIndex+9999;

        public override bool PreLoad => true;

        public override string VisualTreeAssetPath => "ConsoleBox";
        
        private List<string> _inputCache = new();
        private int _inputCacheIndex = 0;
        public bool debugLog = true;
        public bool debugWarningLog = true;
        public bool debugErrorLog = true;
        public Color logColor = Color.white;
        public Color warningColor = Color.yellow;
        public Color errorColor = Color.red;
        private ListView _tipView;
        private LogView _logView;
        private TextField _input;
        private Button _submitButton;
        /// <summary>
        /// 用于标记是否阻止KeyDown事件
        /// </summary>
        private bool _stopCache = false;
        private int _tipSelectIndex = -1;

        public int TipSelectIndex
        {
            get=>_tipSelectIndex;
            set => _tipSelectIndex = value;
        }
        public ConsoleBox() : base()
        {
            _logView = this.Q<LogView>();
            _input = this.Q<TextField>("Input");
            _submitButton = this.Q<Button>("Submit");
            _tipView = this.Q<ListView>();
            _tipView.makeItem = MakeTipItem;
            _tipView.itemsSource = _tipList;
            _tipView.bindItem = (element, index) =>
            {
                var label = (Label) element;
                label.text = _tipList[index];
            };
            _submitButton.clickable.clicked += Submit;
            _input.RegisterCallback(new EventCallback<KeyDownEvent>(OnKeyDown),TrickleDown.TrickleDown);
        }

        private Label MakeTipItem()
        {
            var label = new Label("")
            {
                style =
                {
                    fontSize = 15
                }
            };
            return label;
        }

        bool HaveTip => _tipList.Count > 0;
        
        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Tab:
                    evt.StopImmediatePropagation();
                    if (HaveTip)
                    {
                        if (TipSelectIndex >= 0 && TipSelectIndex < _tipList.Count)
                        {
                            var value = _tipList[TipSelectIndex];
                            _input.SetValueWithoutNotify(value);
                            _input.SelectRange(value.Length, value.Length);
                            TipSelectIndex = -1;
                            RefreshView();
                        }
                    }
                    break;
                case KeyCode.Return:
                    _stopCache = true;
                    Submit();
                    RefreshView();
                    break;
                case KeyCode.UpArrow:
                    evt.StopImmediatePropagation();
                    if (HaveTip)
                    {
                        if (_tipList.Count > 0 && _tipView.visible)
                        {
                            TipSelectIndex--;
                            if (TipSelectIndex < 0)
                                TipSelectIndex = _tipList.Count - 1;
                            else if(TipSelectIndex >= _tipList.Count)
                                TipSelectIndex = 0;
                            RefreshView();
                        }
                    }else
                    {
                        _stopCache = true;
                        LastInput();
                        schedule.Execute(OnInput);
                    }
                    break;
                case KeyCode.DownArrow:
                    evt.StopImmediatePropagation();
                    if (HaveTip)
                    {
                        if (_tipList.Count > 0 && _tipView.visible)
                        {
                            TipSelectIndex++;
                            if (TipSelectIndex < 0)
                                TipSelectIndex = _tipList.Count - 1;
                            else if(TipSelectIndex >= _tipList.Count)
                                TipSelectIndex = 0;
                            RefreshView();
                        }
                    }
                    else
                    {
                        _stopCache = true;
                        NextInput();
                        schedule.Execute(OnInput);
                    }
                    break;
                case KeyCode.Escape:
                    _stopCache = true;
                    TipSelectIndex = -1;
                    RefreshView();
                    break;
                case KeyCode.None:
                    if (_stopCache)
                    {
                        evt.StopImmediatePropagation();
                        _stopCache = false;
                    }
                    break;
                default:
                    schedule.Execute(OnInput);
                    _stopCache = false;
                    break;
            }

        }
        private HashSet<string> _tipHashSet = new();
        private List<string> _tipList = new();
        private void OnInput()
        {
            var value = _input.text;
            ConsoleControl.CommandTipList(value,_tipHashSet);
            _tipList.Clear();
            foreach (var tip in _tipHashSet)
            {
                _tipList.Add(tip);
            }
            RefreshView();
        }

        private void RefreshView()
        {
            _tipView.visible = _tipList.Count > 0;
            _tipView.selectedIndex = TipSelectIndex;
            _tipView.Rebuild();
        }
       

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Submit()
        {
            var text = _input.text;
            if(string.IsNullOrEmpty(text))
                return;
            _tipList.Clear();
            TipSelectIndex = -1;
            _input.SetValueWithoutNotify(String.Empty);
            _inputCacheIndex = 0;
            _inputCache.Insert(0,text);
            ConsoleControl.ExecuteCommand(text);
        }

        public void LastInput()
        {
            if(_inputCache.Count <= 0)
                return;
            _inputCacheIndex++;
            _inputCacheIndex = Mathf.Clamp(_inputCacheIndex, 0, _inputCache.Count-1);
            var value = _inputCache[_inputCacheIndex];
            _input.SetValueWithoutNotify(value);
        }

        public void NextInput()
        {
            if(_inputCache.Count <= 0)
                return;
            _inputCacheIndex--;
            _inputCacheIndex = Mathf.Clamp(_inputCacheIndex, 0, _inputCache.Count-1);
            var value = _inputCache[_inputCacheIndex];
            _input.SetValueWithoutNotify(value);
        }
        

        public void AddItem(LogView.LogValue data)
        {
            if(_logView == null)
                return;
            _logView.AddItem(data);
        }
    }
}