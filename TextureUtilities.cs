using System.IO;
using UnityEngine;

public static class TextureUtilities
{
    /// <summary>
    /// Returns a copy in black and white of the texture.
    /// </summary>
    /// <param name="src">The texture to convert.</param>
    public static Texture2D ToGrayscale(this Texture2D src)
    {
        Texture2D tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, true);
        tex.SetPixels(src.GetPixels());
        Color32[] pixels = tex.GetPixels32();
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Color32 pixel = pixels[x + y * tex.width];
                int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
                int b = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int g = p % 256;
                p = Mathf.FloorToInt(p / 256);
                int r = p % 256;
                float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
                Color c = new Color(l, l, l, 1);
                tex.SetPixel(x, y, c);
            }
        }
        tex.Apply(false);
        return tex;
    }

    /// <summary>
    /// Returns an inverted copy of the texture.
    /// </summary>
    /// <param name="src">The texture to invert.</param>
    public static Texture2D Invert(this Texture2D src)
    {
        Texture2D tex = new Texture2D(src.width, src.height, TextureFormat.RGBA32, true);
        tex.SetPixels(src.GetPixels());
        Color32[] pixels = tex.GetPixels32();
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                Color32 pixel = pixels[x + y * tex.width];
                Color c = new Color(pixel.r / 255f, pixel.g / 255f, pixel.b / 255f);
                tex.SetPixel(x, y, Color.white - c);
            }
        }
        tex.Apply(false);
        return tex;
    }
}
