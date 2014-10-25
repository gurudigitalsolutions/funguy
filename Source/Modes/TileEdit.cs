using System;

using OpenTK.Input;


namespace FunGuy.Modes
{

    public class TileEdit : Mode
    {
        public TileEdit()
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
            if (!Player.CanMove)
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
                //  Toggle thing explorer
                if (Game.Engine.ModeIsEditor)
                {
                    Console.WriteLine("Switched to Thing Explorer");
                    Game.Engine.GameMode = Game.GameModeThings;


                }
                else if (Game.Engine.ModeIsThings)
                {
                    Console.WriteLine("Switched to Editor");
                    Game.Engine.GameMode = Game.GameModeEditor;
                }
            }
            #endregion



            #region MOVE DOWN
            if (Keyboard [Key.Down])
            {
                // Characters direction
                Game.Engine.CharDirection = CharacterTexture.NextDown(Game.Engine.CharDirection);
                //
                if (Player.Y > 0
                    && (Game.Engine.ModeIsEditor))
                {
                    Player.Y--;
                }
                else if (Player.Y == 0
                    && (Game.Engine.ModeIsEditor))
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
                    && ((Game.Engine.ModeIsEditor )))
                {
                    Player.Y++;
                }
                else if (Player.Y + 1 == Game.Engine.TheMap.Height
                    && (Game.Engine.ModeIsEditor))
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
                    && ((Game.Engine.ModeIsEditor)))
                {

                    Player.X--;
                }
                else if (Player.X == 0
                    && (Game.Engine.ModeIsEditor))
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
                    && ((Game.Engine.ModeIsEditor)))
                {
                    Player.X++;
                }
                else if (Player.X + 1 == Game.Engine.TheMap.Width
                    && (Game.Engine.ModeIsEditor))
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

            #region CYCLE ITEMS
            if ((Keyboard [Key.Period] || Keyboard [Key.Comma]))
            {
                int next;
                if (Game.Engine.ModeIsEditor)
                {
                    //next = Texture.DefaultMapTexts.FindIndex(i => i.Value == TileIndex);
                    next = Game.Engine.TheMap.Textures.FindIndex(i => i.Value == Game.Engine.TileIndex);
                    if (Keyboard [Key.Period]) {
                        next++;}
                    else if (Keyboard [Key.Comma]){
                        next--;}

                    if (next < 0){
                        next = Game.Engine.TheMap.Textures.Count - 1;}
                    else if (Game.Engine.TheMap.Textures.Count - 1 < next){
                        next = 0;}

                    //TheMap.Coordinates [Player.X, Player.Y] = Texture.DefaultMapTexts [next].Value;
                    Game.Engine.TheMap.Coordinates [Player.X, Player.Y] = Game.Engine.TheMap.Textures [next].Value;
                }

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

