using UnityEngine;

public static class StringExtention 
{
    public static string InColor(this string message, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
    }
}