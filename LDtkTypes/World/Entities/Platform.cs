namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Platform : ILDtkEntity
{
    public static Platform Default() => new()
    {
        Identifier = "Platform",
        Uid = 2,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        Tile = new TilesetRectangle()
        {
            X = 120,
            Y = 0,
            W = 24,
            H = 24
        },
        SmartColor = new Color(148, 217, 179, 255),

        GoingForwards = true,
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Vector2[]? Positions { get; set; }
    public float[] TimeBetweenPositions { get; set; }
    public bool GoingForwards { get; set; }
    public EntityReference[] Children { get; set; }
}
#pragma warning restore
