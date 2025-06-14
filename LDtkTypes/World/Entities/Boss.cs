namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class Boss : ILDtkEntity
{
    public static Boss Default() => new()
    {
        Identifier = "Boss",
        Uid = 1393,
        Size = new Vector2(32f, 32f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(227, 58, 19, 255),

    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public int Id { get; set; }
}
#pragma warning restore
