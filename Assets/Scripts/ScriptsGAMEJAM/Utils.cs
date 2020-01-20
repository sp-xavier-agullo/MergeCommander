using UnityEngine;

namespace GJ18
{
    public static class Utils
    {
        public static void SetSize(RectTransform t, float width, float height)
        {
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            t.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);   
        }
    }
}