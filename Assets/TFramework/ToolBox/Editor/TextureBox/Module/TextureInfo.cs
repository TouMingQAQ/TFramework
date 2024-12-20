using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
    public class TextureInfo : TextureBox.SpriteEditorModule
    {
        public override string GetTabName => "Texture Info";
        public override bool Enable => true;
        public override bool CanDisEnable => false;
        private Label info;
        public override void Init(TextureBox box, VisualElement root)
        {
            info = new Label(GetTextureInfo(box.SourceTexture));
            root.Add(info);
        }

        public override void OnTextureChange(in Texture2D sourceTexture, ref Texture2D editorTexture)
        {
            info.text = GetTextureInfo(sourceTexture);
        }

        string GetTextureInfo(Texture2D texture)
        { 
            if(texture == null)
                return "Have no texture";
            return $"Format:{texture.format}" +
                   $"\nReadable:{texture.isReadable}" +
                   $"\nTransparency:{texture.alphaIsTransparency}" +
                   $"\nSRGB:{texture.isDataSRGB}" +
                   $"\nWidth: {texture.width}, Height: {texture.height}";
        }
    }
}