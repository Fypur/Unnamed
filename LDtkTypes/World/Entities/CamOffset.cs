namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class CamOffset : ILDtkEntity
{
    public static CamOffset Default() => new()
    {
        Identifier = "CamOffset",
        Uid = 473,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(144, 180, 0, 255),

    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Vector2 Offset { get; set; }
    public bool Override { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}
#pragma warning restore
