using System;

using OpenTK.Input;


namespace FunGuy.Modes
{

    public abstract class Mode
    {
        public Mode()
        {
        }

        public abstract void Render();
        public abstract void Update(OpenTK.Input.KeyboardDevice Keyboard);

    }
}

