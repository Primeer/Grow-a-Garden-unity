using UnityEngine.UI;

namespace Game.Scripts.Common.Utilities
{
    public static class ColorUtils
    {
        public static void SetAlpha(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}