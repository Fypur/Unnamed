namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class HangingWire : ILDtkEntity
{
    public static HangingWire Default() => new()
    {
        Identifier = "HangingWire",
        Uid = 1383,
        Size = new Vector2(2f, 2f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(91, 87, 84, 255),

    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public Vector2[] ControlPoints { get; set; }
}
#pragma warning restore
