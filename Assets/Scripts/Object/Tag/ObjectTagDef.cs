using UnityEngine;

public class ObjectTagDef : Def
{
    public Color Color { get; init; } = new Color(1f, 0.5f, 0.5f);
    public string ColorHex => "#" + ColorUtility.ToHtmlStringRGB(Color);
}
