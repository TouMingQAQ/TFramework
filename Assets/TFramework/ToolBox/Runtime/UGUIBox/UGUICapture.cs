using UnityEngine;

public class UGUICapture : MonoBehaviour
{
    public Camera camera;
    public Canvas Canvas;

    public Texture2D Capture(RectTransform rectTransform)
    {
        var _texture = new Texture2D(camera.pixelWidth, camera.pixelHeight,TextureFormat.RGB24,false);

        var copy = Instantiate(rectTransform.gameObject, Canvas.transform);
        copy.SetActive(true);
        camera.Render();
        RenderTexture.active = camera.targetTexture;//激活这个rt, 并从中中读取像素。
        _texture.ReadPixels(new Rect(0,0,_texture.width,_texture.height), 0, 0);
        _texture.Apply();
        RenderTexture.active = null;
        DestroyImmediate(copy);
        return _texture;
    }
}
