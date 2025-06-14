namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class StreetLight : ILDtkEntity
{
    public static StreetLight Default() => new()
    {
        Identifier = "StreetLight",
        Uid = 416,
        Size = new Vector2(16f, 40f),
        Pivot = new Vector2(0f, 0f),
        Tile = new TilesetRectangle()
        {
            X = 0,
            Y = 0,
            W = 16,
            H = 40
        },
        SmartColor = new Color(148, 217, 179, 255),

    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Vector2? TriggerTopLeft { get; set; }
    public Vector2? TriggerBottomRight { get; set; }
}
#pragma warning restore
