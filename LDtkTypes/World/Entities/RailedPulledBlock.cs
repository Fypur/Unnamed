namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class RailedPulledBlock : ILDtkEntity
{
    public static RailedPulledBlock Default() => new()
    {
        Identifier = "RailedPulledBlock",
        Uid = 12,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(3, 177, 241, 255),

        MaxSwingDistance = 300f,
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Vector2[]? RailPositions { get; set; }
    public float MaxSwingDistance { get; set; }
}
#pragma warning restore
