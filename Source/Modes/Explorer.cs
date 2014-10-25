using System;


namespace FunGuy.Modes
{
    /// <summary>
    /// The game mode for exploring the world map
    /// </summary>
    public class Explorer : Mode
    {
        public Explorer()
        {
        }

        public override void Update()
        {
            if(Game.Engine.ModeIsGame)
            {

            }
        }
        public override void Render()
        {
            throw new System.NotImplementedException();
        }
    }
}

