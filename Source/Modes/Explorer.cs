using System;

using OpenTK.Input;


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
            if(Keyboard[Key.Q] || Keyboard[Key.Escape])
            {
                Game.Engine.Exit();
            }





            if (Keyboard [Key.P])
            {
                Console.WriteLine("Position: X: {0} Y: {1} ", Player.X, Player.Y);
                for (int et = 0; et < Game.Engine.TheMap.Things.Count; et++)
                {
                    Console.WriteLine("Thing {0} : Index {1}", et, Game.Engine.TheMap.Things [et].Index);
                }
            }

           

            if (!Player.CanMove)
            {
                return;
            }

            #region GAME MODE TOGGLING

            if (Keyboard [Key.Space])
            {
                Game.Engine.GameMode = Game.GameModeEditor;
            }
            #endregion

            #region MOVE DOWN
            if (Keyboard [Key.Down])
            {
                // Characters direction
                Game.Engine.CharDirection = CharacterTexture.NextDown(Game.Engine.CharDirection);
                //
                if (Player.Y > 0 
                && ((Game.Engine.TheMap.Coordinates [Player.X, Player.Y - 1] > -1) && !Game.Engine.TheMap.IsThingAt(Player.X, Player.Y - 1)))
                {
                    Player.Y--;
                }
                if (Player.Y == 0 && (Game.Engine.OuterMaps [1, 2].Coordinates [Player.X, Game.Engine.OuterMaps [1, 2].Height - 1] > -1))
                {
                    if (Game.Engine.OuterMaps [0, 0] != null) {
                        Game.Engine.OuterMaps [0, 0].UnloadTextures(); }
                    if (Game.Engine.OuterMaps [1, 0] != null) {
                        Game.Engine.OuterMaps [1, 0].UnloadTextures(); }
                    if (Game.Engine.OuterMaps [2, 0] != null) {
                        Game.Engine.OuterMaps [2, 0].UnloadTextures(); }

                    for (int x = 0; x < 3; x++)
                        {
                        for (int y = 1; y < 3; y++)
                            {
                            Game.Engine.OuterMaps [x, y - 1] = Game.Engine.OuterMaps [x, y];
                            }
                        }


                    int cx = Game.Engine.WorldMapX - 1;
                    for (int x = 0; x < 3; x++)
                        {
                        Game.Engine.OuterMaps [x, 2] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", Game.configPath, cx + x, Game.Engine.WorldMapY + 2));
                        if (Game.Engine.OuterMaps [x, 2] != null)
                            {
                            Game.Engine.OuterMaps [x, 2].WorldX = cx + x;
                            Game.Engine.OuterMaps [x, 2].WorldY = Game.Engine.WorldMapY + 2;
                            }
                        }

                    Game.Engine.OuterMaps [1, 0] = Game.Engine.TheMap;
                    Game.Engine.TheMap = Game.Engine.OuterMaps [1, 1];
                    Game.Engine.WorldMapY++;
                    Player.Y = Game.Engine.TheMap.Height - 1;

                }
            }
            #endregion

            #region MOVE UP
            if (Keyboard [Key.Up])
            {
                Game.Engine.CharDirection = CharacterTexture.NextUp(Game.Engine.CharDirection);
                //
                if (Player.Y + 1 < Game.Engine.TheMap.Height
                    && (Game.Engine.TheMap.Coordinates [Player.X, Player.Y + 1] > -1) && !Game.Engine.TheMap.IsThingAt(Player.X, Player.Y + 1))
                {
                    Player.Y++;
                }
                else if (Player.Y + 1 == Game.Engine.TheMap.Height
                    && (Game.Engine.OuterMaps [1, 0].Coordinates [Player.X, 0] > -1))
                {
                    for (int x = 0; x < 3; x++)
                        {
                        for (int y = 1; y > -1; y--)
                            {
                            Game.Engine.OuterMaps [x, y + 1] = Game.Engine.OuterMaps [x, y];
                            }
                        }

                    int cx = Game.Engine.WorldMapX - 1;
                    for (int x = 0; x < 3; x++)
                        {
                        Game.Engine.OuterMaps [x, 0] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", Game.configPath, cx + x, Game.Engine.WorldMapY - 2));
                        if (Game.Engine.OuterMaps [x, 0] != null)
                            {
                            Game.Engine.OuterMaps [x, 0].WorldX = cx + x;
                            Game.Engine.OuterMaps [x, 0].WorldY = Game.Engine.WorldMapY - 2;
                            }
                        }

                    Game.Engine.OuterMaps [1, 2] = Game.Engine.TheMap;
                    Game.Engine.TheMap = Game.Engine.OuterMaps [1, 1];
                    Game.Engine.WorldMapY--;
                    Player.Y = 0;
                }
            }
            #endregion

            #region MOVE LEFT
            if (Keyboard [Key.Left])
            {
                // Character direction
                Game.Engine.CharDirection = CharacterTexture.NextLeft(Game.Engine.CharDirection);
                //
                if (Player.X > 0
                    && ( Game.Engine.TheMap.Coordinates [Player.X - 1, Player.Y] > -1) && !Game.Engine.TheMap.IsThingAt(Player.X - 1, Player.Y))
                {

                    Player.X--;
                }
                else if (Player.X == 0
                    && (Game.Engine.OuterMaps [0, 1].Coordinates [Game.Engine.OuterMaps [0, 1].Width - 1, Player.Y] > -1))
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 1; x > -1; x--)
                        {
                            Game.Engine.OuterMaps [x + 1, y] = Game.Engine.OuterMaps [x, y];
                        }
                    }

                    int cy = Game.Engine.WorldMapY - 1;
                    for (int y = 0; y < 3; y++)
                    {
                        Game.Engine.OuterMaps [0, y] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", Game.configPath, Game.Engine.WorldMapX - 2, cy + y));
                        if (Game.Engine.OuterMaps [0, y] != null)
                            {
                            Game.Engine.OuterMaps [0, y].WorldX = Game.Engine.WorldMapX - 2;
                            Game.Engine.OuterMaps [0, y].WorldY = cy + y;
                            }
                    }

                    Game.Engine.OuterMaps [2, 1] = Game.Engine.TheMap;
                    Game.Engine.TheMap = Game.Engine.OuterMaps [1, 1];
                    Game.Engine.WorldMapX--;
                    Player.X = Game.Engine.TheMap.Width - 1;
                }

            }
            #endregion

            #region MOVE RIGHT
            if (Keyboard [Key.Right])
            {
                // Character direction
                Game.Engine.CharDirection = CharacterTexture.NextRight(Game.Engine.CharDirection);
                //
                if (Player.X + 1 < Game.Engine.TheMap.Width
                    && (Game.Engine.TheMap.Coordinates [Player.X + 1, Player.Y] > -1) && !Game.Engine.TheMap.IsThingAt(Player.X + 1, Player.Y))
                {
                    Player.X++;
                }
                else if (Player.X + 1 == Game.Engine.TheMap.Width
                    && (Game.Engine.OuterMaps [2, 1].Coordinates [0, Player.Y] > -1))
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            Game.Engine.OuterMaps [x, y] = Game.Engine.OuterMaps [x + 1, y];
                        }
                    }

                    int cy = Game.Engine.WorldMapY - 1;
                    for (int y = 0; y < 3; y++)
                    {
                        Game.Engine.OuterMaps [2, y] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", Game.configPath, Game.Engine.WorldMapX + 2, cy + y));
                        if (Game.Engine.OuterMaps [2, y] != null)
                        {
                            Game.Engine.OuterMaps [2, y].WorldX = Game.Engine.WorldMapX + 2;
                            Game.Engine.OuterMaps [2, y].WorldY = cy + y;
                        }
                    }

                    Game.Engine.OuterMaps [0, 1] = Game.Engine.TheMap;
                    Game.Engine.TheMap = Game.Engine.OuterMaps [1, 1];
                    Game.Engine.WorldMapX++;
                    Player.X = 0;
                }
            }
            #endregion
        }
       

    }
}

