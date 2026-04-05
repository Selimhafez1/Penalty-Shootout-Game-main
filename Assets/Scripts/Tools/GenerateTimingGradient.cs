using System.IO;
using UnityEngine;

//creates a gradient texture for the timing bar based on the specifics in the TimingBarController.cs script
public class GenerateTimingGradient
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Generate Timing Gradient PNG")]
    public static void Generate()
    {
        int width = 1024;
        int height = 32;

        // Match TimingBarController exactly
        float center = 0.5f;
        float perfectWindow = 0.06f;
        float goodWindow = 0.24f;
        float edgeFade = 0.12f;
        float innerFade = 0.10f;

        goodWindow = Mathf.Max(goodWindow, perfectWindow + 0.001f);

        float innerBoundary = perfectWindow + innerFade;
        float outerBoundary = goodWindow + edgeFade;

        Color red = new Color(1f, 0f, 0f, 1f);
        Color yellow = new Color(1f, 0.85f, 0f, 1f);
        Color green = new Color(0f, 1f, 0f, 1f);

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        for (int x = 0; x < width; x++)
        {
            float t = x / (float)(width - 1);
            float dist = Mathf.Abs(t - center);

            Color c;

            // 1) Perfect zone = solid green
            if (dist <= perfectWindow)
            {
                c = green;
            }
            // 2) Inner fade = green -> yellow
            else if (dist <= innerBoundary)
            {
                float k = Mathf.InverseLerp(perfectWindow, innerBoundary, dist);
                c = Color.Lerp(green, yellow, Mathf.SmoothStep(0f, 1f, k));
            }
            // 3) Good zone = solid yellow
            else if (dist <= goodWindow)
            {
                c = yellow;
            }
            // 4) Outer fade = yellow -> red
            else if (dist <= outerBoundary)
            {
                float k = Mathf.InverseLerp(goodWindow, outerBoundary, dist);
                c = Color.Lerp(yellow, red, Mathf.SmoothStep(0f, 1f, k));
            }
            // 5) Fully red zone
            else
            {
                c = red;
            }

            SetColumn(tex, x, height, c);
        }

        tex.Apply();

        byte[] png = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, "TimingGradient.png");
        File.WriteAllBytes(path, png);

        UnityEditor.AssetDatabase.Refresh();
        Debug.Log($"Saved gradient to: {path}");
    }

    private static void SetColumn(Texture2D tex, int x, int height, Color c)
    {
        for (int y = 0; y < height; y++)
            tex.SetPixel(x, y, c);
    }
#endif
}