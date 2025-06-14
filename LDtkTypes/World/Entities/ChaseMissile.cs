namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class ChaseMissile : ILDtkEntity
{
    public static ChaseMissile Default() => new()
    {
        Identifier = "ChaseMissile",
        Uid = 1403,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(215, 118, 67, 255),

        Time = 1.5f,
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Vector2[]? ControlPoints { get; set; }
    public float Time { get; set; }
}
#pragma warning restore
