namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class SpecialTrigger : ILDtkEntity
{
    public static SpecialTrigger Default() => new()
    {
        Identifier = "SpecialTrigger",
        Uid = 102,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(10, 67, 243, 255),

    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public int TypeId { get; set; }
    public int Id { get; set; }
    public EntityReference[] Children { get; set; }
    public Vector2[]? Positions { get; set; }
}
#pragma warning restore
