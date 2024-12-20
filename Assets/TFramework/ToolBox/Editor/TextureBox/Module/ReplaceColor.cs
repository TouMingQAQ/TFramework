using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TFramework.ToolBox
{
    public class ReplaceColor : TextureBox.SpriteEditorModule
    {
        public override string GetTabName => "颜色替换";
        private ColorField sourceColor;
        private ColorField targetColor;
        private MinMaxSlider rangSlider;
        public override void Init(TextureBox box, VisualElement root)
        {
            sourceColor = new ColorField("Source");
            targetColor = new ColorField("Target");
            rangSlider = new MinMaxSlider("Range",0,0,0,1);
            root.Add(rangSlider);
            root.Add(sourceColor);
            root.Add(targetColor);
        }

        public override void Process(in Texture2D sourceTexture, ref Texture2D editorTexture)
        {
            var pixels = sourceTexture.GetPixels();
            var range = rangSlider.value;
            var sourceValue = sourceColor.value;
            var targetValue = targetColor.value;
            for (int i = 0; i < pixels.Length; i++)
            {
                var currentValue = pixels[i];
                var r = Mathf.Abs(currentValue.r - sourceValue.r);
                var g = Mathf.Abs(currentValue.g - sourceValue.g);
                var b = Mathf.Abs(currentValue.b - sourceValue.b);
                // var a = Mathf.Abs(currentValue.a - sourceValue.a);

                // if(a < range.x || a > range.y)
                //     continue;
                if (r < range.x || r > range.y)
                    continue;
                if (g < range.x || g > range.y)
                    continue;
                if (b < range.x || b > range.y)
                    continue;
                pixels[i] = targetValue-sourceValue+currentValue;
            }
            editorTexture.SetPixels(pixels);
        }
    }
}