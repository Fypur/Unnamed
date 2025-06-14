namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Light : ILDtkEntity
{
    public static Light Default() => new()
    {
        Identifier = "Light",
        Uid = 1303,
        Size = new Vector2(1f, 1f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(255, 255, 255, 255),

        Range = 360f,
        Length = 70f,
        Color = new Color(255, 255, 255, 1),
        Opacity = 110,
        CollideWithWalls = false,
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public float Direction { get; set; }
    public float Range { get; set; }
    public float Length { get; set; }
    public Color Color { get; set; }
    public int Opacity { get; set; }
    public bool CollideWithWalls { get; set; }
    public float? BlinkTime { get; set; }
}
#pragma warning restore
