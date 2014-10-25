using System;

using OpenTK.Input;


namespace FunGuy.Modes
{

    public class Thing : Mode
    {
        public Thing()
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
            if (Keyboard [Key.P])
            {
                Console.WriteLine("Position: X: {0} Y: {1} ", Game.Engine.Party[0].X, Game.Engine.Party[0].Y);
                for (int et = 0; et < Game.Engine.TheMap.Things.Count; et++)
                {
                    Console.WriteLine("Thing {0} : Index {1}", et, Game.Engine.TheMap.Things [et].Index);
                }
            }

            if (!Game.Engine.Party[0].CanMove)
            {
                return;
            }

            #region GAME MODE TOGGLING
            if (Keyboard [Key.Space])
            {
                Game.Engine.GameMode = Game.GameModeGame;
            }

            if (Keyboard [Key.Number0])
            {
                Console.WriteLine("Switched to Editor");
                Game.Engine.GameMode = Game.GameModeEditor;
            }
            #endregion

            #region THING EDITING

            if (Keyboard [Key.S] && (!Keyboard [Key.ControlLeft] && !Keyboard [Key.ControlRight]))
                {
                if (Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Height > 1) {
                    Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Height--; }
                }
            else if (Keyboard [Key.W])
                {
                Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Height++;
                }
            if (Keyboard [Key.D])
                {
                if (Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Width > 1) {
                    Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Width--; }
                }
            else if (Keyboard [Key.E])
                {
                Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Width++;
                }
            if (Keyboard [Key.F])
                {
                if (Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Depth > 1) {
                    Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Depth--; }
                }
            else if (Keyboard [Key.R])
                {
                Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Depth++;
                }
            #region Paste Thing
            else if (Keyboard [Key.ControlLeft] && Keyboard [Key.V])
                {
                //  Paste a copy of this thing here
                Console.WriteLine("Pasting copy of this thing.");
                //int indd = TheMap.Things.Find(x => x.Index == ThingIndex).Index;
                int indd = Game.Engine.ThingIndex;

                object otting = System.Activator.CreateInstance(Game.Engine.TheMap.Things [indd].GetType());
                FunGuy.Thing tting = (FunGuy.Thing)otting;

                tting.X = Game.Engine.TheMap.Things [indd].X;
                tting.Y = Game.Engine.TheMap.Things [indd].Y;
                tting.Depth = Game.Engine.TheMap.Things [indd].Depth;
                tting.Height = Game.Engine.TheMap.Things [indd].Height;
                tting.Width = Game.Engine.TheMap.Things [indd].Width;
                tting.SetTextures(Game.Engine.TheMap.Things [indd].TextureList());

                Game.Engine.TheMap.Things.Add(tting);

                Console.WriteLine("ThingCount: {0}", Game.Engine.TheMap.Things.Count);
                for (int et = 0; et < Game.Engine.TheMap.Things.Count; et++)
                    {
                    Console.WriteLine("Thing Index: {0}", Game.Engine.TheMap.Things [et].Index);
                    }
                }
            #endregion
            #endregion

            #region MOVE DOWN
            if (Keyboard [Key.Down])
            {
                // Characters direction
                Game.Engine.CharDirection = CharacterTexture.NextDown(Game.Engine.CharDirection);
                //
           
                FunGuy.Thing thing = Game.Engine.TheMap.Things [Game.Engine.ThingIndex];
                if (thing.Y > 0)
                {
                    int ind = Game.Engine.TheMap.Things.IndexOf(thing);
                    Game.Engine.TheMap.Things [ind].Y--;
                 }

            }
            #endregion

            #region MOVE UP
            if (Keyboard [Key.Up])
            {
                Game.Engine.CharDirection = CharacterTexture.NextUp(Game.Engine.CharDirection);
                //
             
                FunGuy.Thing thing = Game.Engine.TheMap.Things [Game.Engine.ThingIndex];
                if (thing.Y + thing.Depth < Game.Engine.TheMap.Height)
                {
                    int ind = Game.Engine.TheMap.Things.IndexOf(thing);
                    Game.Engine.TheMap.Things [ind].Y++;
                }

            }
            #endregion

            #region MOVE LEFT
            if (Keyboard [Key.Left])
            {
                // Character direction
                Game.Engine.CharDirection = CharacterTexture.NextLeft(Game.Engine.CharDirection);
                //
                FunGuy.Thing thing = Game.Engine.TheMap.Things [Game.Engine.ThingIndex];
                if (thing.X > 0)
                {
                    int ind = Game.Engine.TheMap.Things.IndexOf(thing);
                    Game.Engine.TheMap.Things [ind].X--;
                }


            }
            #endregion

            #region MOVE RIGHT
            if (Keyboard [Key.Right])
            {
                // Character direction
                Game.Engine.CharDirection = CharacterTexture.NextRight(Game.Engine.CharDirection);
                //

                FunGuy.Thing thing = Game.Engine.TheMap.Things [Game.Engine.ThingIndex];
                if (thing.X + thing.Width < Game.Engine.TheMap.Width)
                {
                    int ind = Game.Engine.TheMap.Things.IndexOf(thing);
                    Game.Engine.TheMap.Things [ind].X++;
                }
            }

            #endregion

            #region CYCLE ITEMS

            int tIndex = 0;
            if (Keyboard [Key.Period])
            {
                tIndex = Game.Engine.ThingIndex + 1;
                if ( tIndex > Game.Engine.TheMap.Things.Count -1) { tIndex = 0;}
                Console.WriteLine("ThingIndex: {0}", tIndex);
            }
            else if (Keyboard [Key.Comma])
            {
                tIndex = Game.Engine.ThingIndex - 1;
                if ( tIndex < 0 ) { tIndex = Game.Engine.TheMap.Things.Count -1 ;}
                Console.WriteLine("ThingIndex: {0}", tIndex);
            }
            Game.Engine.ThingIndex = tIndex;


            #endregion

            #region CYCLE THING TYPES
            if (Keyboard [Key.Semicolon] || Keyboard [Key.Quote])
            {

                int next = FunGuy.Thing.AllThings.FindIndex(x => x.GetType() == Game.Engine.TheMap.Things [Game.Engine.ThingIndex].GetType());
                if (Keyboard [Key.Semicolon])
                {
                    next--;
                }
                else if (Keyboard [Key.Quote])
                {
                    next++;
                }

                if (next < 0) {
                    next = FunGuy.Thing.AllThings.Count - 1; }
                else if (next == FunGuy.Thing.AllThings.Count) {
                    next = 0; }

                object obthing = System.Activator.CreateInstance(FunGuy.Thing.AllThings [next].GetType());
                FunGuy.Thing newthing = (FunGuy.Thing)obthing;

                newthing.X = Game.Engine.TheMap.Things [Game.Engine.ThingIndex].X;
                newthing.Y = Game.Engine.TheMap.Things [Game.Engine.ThingIndex].Y;
                Game.Engine.TheMap.Things [Game.Engine.ThingIndex] = newthing;
            }

            #endregion

            #region SAVE
            if (Keyboard [Key.S]
                && (Keyboard [Key.ControlLeft] || Keyboard [Key.ControlRight]))
            {
                Console.WriteLine("Saved Map: {0}", Game.Engine.TheMap.Save());
            }
            #endregion
        }
       

    }
}

