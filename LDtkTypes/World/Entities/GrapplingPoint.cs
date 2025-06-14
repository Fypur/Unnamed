namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class GrapplingPoint : ILDtkEntity
{
    public static GrapplingPoint Default() => new()
    {
        Identifier = "GrapplingPoint",
        Uid = 8,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        Tile = new TilesetRectangle()
        {
            X = 136,
            Y = 72,
            W = 8,
            H = 8
        },
        SmartColor = new Color(72, 94, 255, 255),

        MaxSwingDistance = 100f,
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

    public float MaxSwingDistance { get; set; }
    public Vector2[]? Positions { get; set; }
    public float[] TimeBetweenPositions { get; set; }
    public bool GoingForwards { get; set; }
}
#pragma warning restore
