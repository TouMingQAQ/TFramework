using System.Collections.Generic;
using TFramework.ToolBox;
using TFramework.ToolBox.UIToolKit;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
    public sealed class TextureBox : BaseToolBox
    {
        public override bool Closeable => false;
        public override string TabName => "图集编辑";

        public override bool PreLoad => true;

        public override string VisualTreeAssetPath => "SpriteBox";
        
        private ObjectField _textureField;
        private ScrollView _scrollView;
        private Texture2D _editorTexture;
        private Texture2D _sourceTexture;
        public Texture2D SourceTexture => _sourceTexture;
        private ImageView _sourceImageView;
        private ImageView _editorImageView;
        private List<SpriteEditorModule> _modules = new();
        public TextureBox() : base()
        {
            _textureField = this.Q<ObjectField>("Texture");
            _textureField.RegisterValueChangedCallback(OnTextureChanged);
            _scrollView = this.Q<ScrollView>();
            _sourceImageView = this.Q<ImageView>("TextureViewSource");
            _editorImageView = this.Q<ImageView>("TextureViewEditor");
            this.Q<ToolbarButton>("Process").clicked += Process;
            this.Q<ToolbarButton>("Reset").clicked += Reset;
            AddModule<TextureInfo>();
            AddModule<ReplaceColor>();
        }

        void Process()
        {
            Debug.Log("SpriteBox Process");
            Reset();
            foreach (var editorModule in _modules)
            {
                if(!editorModule.Enable)
                    continue;
                editorModule.Process(in _sourceTexture,ref _editorTexture);
            }
            _editorTexture.Apply();
        }

        void Reset()
        {
            if (_sourceTexture == null)
            {
                _editorImageView.Texture = null;
                _editorTexture = null;
                return;
            }
            _editorTexture = CreateTextureBySetGetPixels(_sourceTexture);
            _editorImageView.Texture = _editorTexture;
        }
        void OnTextureChanged(ChangeEvent<Object> evt)
        {
            _sourceTexture = evt.newValue as Texture2D;
            if (_sourceTexture == null)
            {
                _sourceImageView.Texture = null;
                _editorImageView.Texture = null;
                return;
            }
            _sourceImageView.Texture = _sourceTexture;
            Reset();
            foreach (var editorModule in _modules)
            {
                if(!editorModule.Enable)
                    continue;
                editorModule.OnTextureChange(in _sourceTexture,ref _editorTexture);
            }
        }

        public static void CopyTo(Texture2D sourceTexture2D, Texture2D targetTexture2D)
        {
            if (sourceTexture2D == null || targetTexture2D == null)
            {
                Debug.LogError("CopyError: Texture is null");
                return;
            }

            targetTexture2D.width = sourceTexture2D.width;
            targetTexture2D.height = sourceTexture2D.height;
            targetTexture2D.SetPixels(sourceTexture2D.GetPixels());
            targetTexture2D.Apply();
        }
        public static Texture2D CreateTextureBySetGetPixels(Texture2D sourceTexture)
        {
            if (sourceTexture == null)
            {
                Debug.LogWarning("Texture is null");
                return null;
            }

            if (!sourceTexture.isReadable)
            {
                Debug.LogWarning("SourceTexture is not readable");

                return null;
            }
            int sourceMipLevel = 0;
            Color[] srcPixels = sourceTexture.GetPixels(sourceMipLevel);
            //newTextureByPixels = new Texture2D(sourceTexture.width, sourceTexture.height);
            Texture2D newTextureByPixels = new Texture2D(sourceTexture.width, sourceTexture.height);
            newTextureByPixels.SetPixels(srcPixels);
            newTextureByPixels.Apply();
            return newTextureByPixels;
        }
        void AddModule<T>() where T : SpriteEditorModule, new()
        {
            var module = new T();
            _modules.Add(module);
            VisualElement root = new VisualElement
            {
                style =
                {
                    flexGrow = 1
                }
            };
            Toggle enable = new Toggle
            {
                label = "Enable",
                value = module.Enable
            };
            enable.RegisterValueChangedCallback(_ =>
            {
                root.visible = _.newValue;
                module.Enable = _.newValue;
            });
            root.visible = module.Enable;
            module.Init(this,root);
            Foldout foldout = new Foldout
            {
                text = module.GetTabName,
                value = false
            };
            if(module.CanDisEnable)
                foldout.Add(enable);
            foldout.Add(root);
            _scrollView.Add(foldout);
        }

        public abstract class SpriteEditorModule
        {
            public virtual bool CanDrag { get; set; } = false;
            public virtual bool CanDisEnable { get; set; } = true;
            public virtual bool Enable { get; set; } = false;
            public virtual string GetTabName => nameof(this.GetType);

            public abstract void Init(TextureBox box,VisualElement root);

            public virtual void Process(in Texture2D sourceTexture,ref Texture2D editorTexture)
            {
                
            }

            public virtual void OnTextureChange(in Texture2D sourceTexture, ref Texture2D editorTexture)
            {
                
            }
        }
    }

}