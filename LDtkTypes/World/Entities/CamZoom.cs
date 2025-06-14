namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class CamZoom : ILDtkEntity
{
    public static CamZoom Default() => new()
    {
        Identifier = "CamZoom",
        Uid = 1518,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(78, 194, 93, 255),

        TargetCamWidth = 320,
        ZoomTime = 1f,
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public int TargetCamWidth { get; set; }
    public float ZoomTime { get; set; }
}
#pragma warning restore
