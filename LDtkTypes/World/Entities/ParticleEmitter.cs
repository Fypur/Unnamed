namespace LDtkTypes;

// This file was automatically generated, any modifications will be lost!
#pragma warning disable

using LDtk;
using Microsoft.Xna.Framework;

public partial class ParticleEmitter : ILDtkEntity
{
    public static ParticleEmitter Default() => new()
    {
        Identifier = "ParticleEmitter",
        Uid = 582,
        Size = new Vector2(1f, 1f),
        Pivot = new Vector2(0f, 0f),
        SmartColor = new Color(25, 121, 215, 255),

        Amount = 3,
        Direction = 0f,
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

    public ParticleTypes Type { get; set; }
    public int Amount { get; set; }
    public float? Direction { get; set; }
    public Color Color { get; set; }
}
#pragma warning restore
