namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Refill : ILDtkEntity
{
    public static Refill Default() => new()
    {
        Identifier = "Refill",
        Uid = 178,
        Size = new Vector2(10f, 10f),
        Pivot = new Vector2(0f, 0f),
        Tile = new TilesetRectangle()
        {
            X = 112,
            Y = 72,
            W = 16,
            H = 16
        },
        SmartColor = new Color(214, 90, 11, 255),

        RespawnTime = 3f,
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public float RespawnTime { get; set; }
    public bool Big { get; set; }
}
#pragma warning restore
