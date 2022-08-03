using System;
using Godot;

namespace Additions.Debugging
{
    [AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class DefaultColorAttribute : Attribute
    {
        public Color commonColor;
        public Color unommonColor;

        public DefaultColorAttribute(string defaultColor, bool isHex = false, bool onlyUncommon = true)
        {
            Color color = isHex ? new Color(defaultColor) : Color.ColorN(defaultColor);

            this.commonColor = onlyUncommon ? Colors.White : color;
            this.unommonColor = color;
        }
        public DefaultColorAttribute(string defaultCommonColor, string defaultUnommonColor, bool commomIsHex = false, bool uncommonIsHex = false)
        {
            Color commonColor = commomIsHex ? new Color(defaultCommonColor) : Color.ColorN(defaultCommonColor);
            Color uncommonColor = uncommonIsHex ? new Color(defaultUnommonColor) : Color.ColorN(defaultUnommonColor);

            this.commonColor = commonColor;
            this.unommonColor = uncommonColor;
        }
    }
}