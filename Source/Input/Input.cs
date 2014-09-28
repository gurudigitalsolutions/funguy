using System;
using System.Collections.Generic;
//usinq OpenTK.Input;

namespace FunGuy.Input
{

    public abstract class Input
    {
        //public Dictionary<string, bool> PrevIn();

        public Input()
        {
        }

        public abstract Dictionary<string, bool> In();

    }
}

