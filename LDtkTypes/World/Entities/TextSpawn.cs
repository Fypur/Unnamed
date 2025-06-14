namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class TextSpawn : ILDtkEntity
{
    public static TextSpawn Default() => new()
    {
        Identifier = "TextSpawn",
        Uid = 70,
        Size = new Vector2(8f, 8f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(94, 227, 45, 255),

        Color = new Color(255, 255, 255, 1),
    };

    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public string Text { get; set; }
    public Vector2 TextPos { get; set; }
    public Color Color { get; set; }
    public int XOffset { get; set; }
    public int YOffset { get; set; }
}
#pragma warning restore
