using System.Reflection;
using UnityEngine;

namespace Logger.Utilities
{
    public static class StringHelper
    {
        public static string GetColoredString(this string targetString, Color color)
        {
            string colorName = "";
            PropertyInfo[] props = color.GetType().GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo prop in props)
            {
                if ((Color) prop.GetValue(null, null) == color)
                {
                    colorName = prop.Name;
                }
            }

            if (colorName == "")
            {
                colorName = color.ToString();
            }

            return $"<color={colorName}{targetString}</color>";
        }

        public static string GetBoldString(this string targetString) => $"<b> {targetString} </b>";
        public static string GetCurvedString(this string targetString) => $"<i> {targetString} </i>";
    }
}