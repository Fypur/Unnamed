using Fiourp;
using Microsoft.Xna.Framework.Input;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platformer
{
    public enum SentInput : int
    {
        Left, Right, Up, Down, Jump, Jetpack, Swing
    }

    public static class MultiplayerInput
    {
        public static readonly int InputCount = Enum.GetNames(typeof(SentInput)).Length;
        public static ControlList LeftControls = Input.LeftControls;
        public static ControlList RightControls = Input.RightControls;
        public static ControlList UpControls = Input.UpControls;
        public static ControlList DownControls = Input.DownControls;
        public static ControlList JumpControls = new ControlList(Keys.C, Keys.Space);
        public static ControlList JetpackControls = new ControlList(Keys.X);
        public static ControlList SwingControls = new ControlList(Keys.W);

        public static void SetLocalInputs(bool[] inputs)
        {
            inputs[(int)SentInput.Left] = LeftControls.Is();
            inputs[(int)SentInput.Right] = RightControls.Is();
            inputs[(int)SentInput.Up] = UpControls.Is();
            inputs[(int)SentInput.Down] = DownControls.Is();
            inputs[(int)SentInput.Jump] = JumpControls.Is();
            inputs[(int)SentInput.Jetpack] = JetpackControls.Is();
            inputs[(int)SentInput.Swing] = SwingControls.Is();
        }

        public static void AddInputs(this Message message, bool[] inputs)
        {
            if (inputs.Length != InputCount) throw new Exception("Given bool array doesn't correspond to an input array");
            message.AddBools(inputs, false);
        }

        public static void GetInputs(this Message message, bool[] intoArray) 
        {
            message.GetBools(InputCount, intoArray, 0);
        }
    }
}
