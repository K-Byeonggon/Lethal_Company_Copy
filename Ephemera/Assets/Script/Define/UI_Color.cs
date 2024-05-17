using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Color
{
    public static Color NormalColor { get; } = HexToColor("#70FFA5");
    public static Color HideColor { get; } = HexToColor("#626F66");
    public static Color CautionColor { get; } = HexToColor("#FFFF70");
    public static Color WarningColor { get; } = HexToColor("#FF7070");

    // 헥사데시멀 문자열을 Color로 변환하는 메서드
    private static Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid hex color code: " + hex);
            return Color.white; // 실패 시 흰색 반환 또는 원하는 기본값 반환
        }
    }
}
