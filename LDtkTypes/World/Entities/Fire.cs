namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Fire : ILDtkEntity
{
    public static Fire Default() => new()
    {
        Identifier = "Fire",
        Uid = 185,
        Size = new Vector2(4f, 4f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(239, 15, 15, 255),

    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Direction Direction { get; set; }
}
#pragma warning restore
