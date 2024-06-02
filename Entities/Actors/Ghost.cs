using Fiourp;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    public class Ghost : Actor
    {
        private Vector2[] positions;
        private float[] rotations;
        private string[] textureIds;
        private int[] textureFrames;
        private bool[] jetpacking;

        private static ParticleType greenJetpack = Particles.Jetpack.Copy();
        private static ParticleType greenDust = Particles.Dust.Copy();

        public Ghost(string filePath) : base(Vector2.Zero, 8, 13, 0, new Sprite(Color.Green))
        {
            int lineCount = File.ReadLines(filePath).Count();

            positions = new Vector2[lineCount];
            rotations = new float[lineCount];
            textureIds = new string[lineCount];
            textureFrames = new int[lineCount];
            jetpacking = new bool[lineCount];

            greenJetpack.Color = Color.Green;
            greenDust.Color = Color.Green;

            //https://stackoverflow.com/questions/2081418/parsing-csv-files-in-c-with-header
            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(".");

                for(int i = 0; !parser.EndOfData; i++)
                {
                    string[] fields = parser.ReadFields();

                    positions[i] = new Vector2((int)float.Parse(fields[0]), (int)float.Parse(fields[1]));
                    rotations[i] = float.Parse(fields[2]);
                    textureIds[i] = fields[3].Substring(0, fields[3].Length - 1);
                    textureFrames[i] =  int.Parse(fields[3].Substring(fields[3].Length - 1));
                    jetpacking[i] = fields[4] == "True";
                }
            }

            Sprite.Origin = Vector2.One * 8;
            Sprite.Offset = Vector2.One * 5;

            Pos = positions[0];
            Sprite.Rotation = rotations[0];
            Sprite.Texture = Sprite.AllAnimData["Player"].Animations[textureIds[0]].Frames[textureFrames[0]];

            AddComponent(new Coroutine(Animate()));
        }

        public Ghost(int id) : this(IdToFile(id)) { }

        public static string IdToFile(int id)
        {
            switch (id)
            {
                case 0:
                    return DataManager.contentDirName + "/Other/recorded.csv";
                default:
                    throw new Exception("id not found");
            }
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                Pos = positions[0];
                Engine.CurrentMap.BackgroundSystem.Emit(greenDust, Bounds, 15);

                for (int i = 0; i < positions.Length; i++)
                {
                    Layer = 1;

                    Velocity = positions[i] - Pos;

                    Pos = positions[i];
                    Sprite.Rotation = rotations[i];
                    Sprite.Texture = Sprite.AllAnimData["Player"].Animations[textureIds[i]].Frames[textureFrames[i] - 1];

                    if (jetpacking[i])
                        Engine.CurrentMap.BackgroundSystem.Emit(greenJetpack, MiddlePos - Velocity);

                    yield return null;
                }

                Engine.CurrentMap.BackgroundSystem.Emit(greenDust, Bounds, 15);
            }
        }
    }
}
