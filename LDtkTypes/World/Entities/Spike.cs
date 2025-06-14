namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Spike : ILDtkEntity
{
    public static Spike Default() => new()
    {
        Identifier = "Spike",
        Uid = 17,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        Tile = new TilesetRectangle()
        {
            X = 32,
            Y = 56,
            W = 8,
            H = 8
        },
        SmartColor = new Color(112, 120, 116, 255),

        Direction = Direction.Up,
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
