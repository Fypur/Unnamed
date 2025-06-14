namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Collectable : ILDtkEntity
{
    public static Collectable Default() => new()
    {
        Identifier = "Collectable",
        Uid = 91,
        Size = new Vector2(8f, 12f),
        Pivot = new Vector2(0f, 0f),
        Tile = new TilesetRectangle()
        {
            X = 128,
            Y = 72,
            W = 8,
            H = 16
        },
        SmartColor = new Color(229, 39, 80, 255),
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }
}
#pragma warning restore
