// This file was automatically generated, any modifications will be lost!

namespace LDtkTypes;

#pragma warning disable
using Microsoft.Xna.Framework;
using LDtk;

public class GrapplingPoint : ILDtkEntity
{
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
