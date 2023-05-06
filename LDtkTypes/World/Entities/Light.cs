// This file was automatically generated, any modifications will be lost!
#pragma warning disable
namespace LDtkTypes;

using Microsoft.Xna.Framework;
using LDtk;

public class Light : ILDtkEntity
{
    public string Identifier { get; set; }
    public System.Guid Iid { get; set; }
    public int Uid { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 Pivot { get; set; }
    public Rectangle Tile { get; set; }

    public Color SmartColor { get; set; }

    public float Direction { get; set; }
    public float Range { get; set; }
    public float Length { get; set; }
    public Color Color { get; set; }
    public int Opacity { get; set; }
}
#pragma warning restore
