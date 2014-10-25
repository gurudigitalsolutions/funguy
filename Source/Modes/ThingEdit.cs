using System;

using OpenTK.Input;


namespace FunGuy.Modes
{

    public class ThingEdit : Mode
    {
        //TODO Create ThingEdit mode
        public ThingEdit()
        {
        }

        public override void Render()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(OpenTK.Input.KeyboardDevice Keyboard)
        {
            KeyPress(Keyboard);

        }

        public void KeyPress(OpenTK.Input.KeyboardDevice Keyboard)
        {
        }
    }
}

