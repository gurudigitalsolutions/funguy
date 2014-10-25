using System;


namespace FunGuy.Modes
{

    public abstract class Mode
    {
        public Mode()
        {
        }

        public abstract void Update();
        public abstract void Render();

    }
}

