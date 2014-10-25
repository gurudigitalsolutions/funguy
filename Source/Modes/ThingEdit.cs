using System;

using OpenTK.Input;


namespace FunGuy.Modes
{

    public class ThingEdit : Mode
    {
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
                        // Key to quit
            if (Keyboard [Key.Escape] || Keyboard [Key.Q])
            {
                Exit();
            }

            if (Keyboard [Key.F11])
            {
                if (WindowState != WindowState.Fullscreen)
                {
                    WindowBorder = WindowBorder.Hidden;
                    WindowState = WindowState.Fullscreen;
                }
                else
                {
                    WindowBorder = WindowBorder.Resizable;
                    WindowState = WindowState.Normal;
                }
            }



            if (Keyboard [Key.P])
            {
                Console.WriteLine("Position: X: {0} Y: {1} ", Player.X, Player.Y);
                for (int et = 0; et < TheMap.Things.Count; et++)
                {
                    Console.WriteLine("Thing {0} : Index {1}", et, TheMap.Things [et].Index);
                }
            }

            if (!Player.CanMove)
            {
                return;
            }

            #region GAME MODE TOGGLING
            if (Keyboard [Key.Space])
                {

                if (ModeIsGame)
                    {
                    GameMode = GameModeEditor;
                    }
                else
                    {
                    if (TheMap.Coordinates [Player.X, Player.Y] > -1)
                        {
                        GameMode = GameModeGame;
                        }
                    }
                }

            if (Keyboard [Key.Number0])
                {
                //  Toggle thing explorer
                if (ModeIsEditor)
                    {
                    Console.WriteLine("Switched to Thing Explorer");
                    GameMode = GameModeThings;


                    }
                else if (ModeIsThings)
                    {
                    Console.WriteLine("Switched to Editor");
                    GameMode = GameModeEditor;
                    }
                }
            #endregion

            #region THING EDITING
            if (ModeIsThings)
                {

                if (Keyboard [Key.S] && (!Keyboard [Key.ControlLeft] && !Keyboard [Key.ControlRight]))
                    {
                    if (TheMap.Things [ThingIndex].Height > 1) {
                        TheMap.Things [ThingIndex].Height--; }
                    }
                else if (Keyboard [Key.W])
                    {
                    TheMap.Things [ThingIndex].Height++;
                    }
                if (Keyboard [Key.D])
                    {
                    if (TheMap.Things [ThingIndex].Width > 1) {
                        TheMap.Things [ThingIndex].Width--; }
                    }
                else if (Keyboard [Key.E])
                    {
                    TheMap.Things [ThingIndex].Width++;
                    }
                if (Keyboard [Key.F])
                    {
                    if (TheMap.Things [ThingIndex].Depth > 1) {
                        TheMap.Things [ThingIndex].Depth--; }
                    }
                else if (Keyboard [Key.R])
                    {
                    TheMap.Things [ThingIndex].Depth++;
                    }
                #region Paste Thing
                else if (Keyboard [Key.ControlLeft] && Keyboard [Key.V])
                    {
                    //  Paste a copy of this thing here
                    Console.WriteLine("Pasting copy of this thing.");
                    //int indd = TheMap.Things.Find(x => x.Index == ThingIndex).Index;
                    int indd = ThingIndex;

                    object otting = System.Activator.CreateInstance(TheMap.Things [indd].GetType());
                    Thing tting = (Thing)otting;

                    tting.X = TheMap.Things [indd].X;
                    tting.Y = TheMap.Things [indd].Y;
                    tting.Depth = TheMap.Things [indd].Depth;
                    tting.Height = TheMap.Things [indd].Height;
                    tting.Width = TheMap.Things [indd].Width;
                    tting.SetTextures(TheMap.Things [indd].TextureList());

                    TheMap.Things.Add(tting);

                    Console.WriteLine("ThingCount: {0}", TheMap.Things.Count);
                    for (int et = 0; et < TheMap.Things.Count; et++)
                        {
                        Console.WriteLine("Thing Index: {0}", TheMap.Things [et].Index);
                        }
                    }
                }
            #endregion
            #endregion

            #region MOVE DOWN
            if (Keyboard [Key.Down])
                {
                // Characters direction
                CharDirection = CharacterTexture.NextDown(CharDirection);
                //
                if (Player.Y > 0 && (!ModeIsThings)
                    && (ModeIsEditor || (TheMap.Coordinates [Player.X, Player.Y - 1] > -1) && !TheMap.IsThingAt(Player.X, Player.Y - 1)))
                    {
                    Player.Y--;
                    }
                else if (ModeIsThings)
                {
                    Thing thing = TheMap.Things [ThingIndex];
                    if (thing.Y > 0)
                        {
                        int ind = TheMap.Things.IndexOf(thing);
                        TheMap.Things [ind].Y--;
                        }
                }
                else if (Player.Y == 0
                    && (ModeIsEditor || OuterMaps [1, 2].Coordinates [Player.X, OuterMaps [1, 2].Height - 1] > -1))
                {
                    if (OuterMaps [0, 0] != null) {
                        OuterMaps [0, 0].UnloadTextures(); }
                    if (OuterMaps [1, 0] != null) {
                        OuterMaps [1, 0].UnloadTextures(); }
                    if (OuterMaps [2, 0] != null) {
                        OuterMaps [2, 0].UnloadTextures(); }

                    for (int x = 0; x < 3; x++)
                        {
                        for (int y = 1; y < 3; y++)
                            {
                            OuterMaps [x, y - 1] = OuterMaps [x, y];
                            }
                        }


                    int cx = WorldMapX - 1;
                    for (int x = 0; x < 3; x++)
                        {
                        OuterMaps [x, 2] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", configPath, cx + x, WorldMapY + 2));
                        if (OuterMaps [x, 2] != null)
                            {
                            OuterMaps [x, 2].WorldX = cx + x;
                            OuterMaps [x, 2].WorldY = WorldMapY + 2;
                            }
                        }

                    OuterMaps [1, 0] = TheMap;
                    TheMap = OuterMaps [1, 1];
                    WorldMapY++;
                    Player.Y = TheMap.Height - 1;

                    }
                }
            #endregion

            #region MOVE UP
            if (Keyboard [Key.Up])
                {
                CharDirection = CharacterTexture.NextUp(CharDirection);
                //
                if (Player.Y + 1 < TheMap.Height
                    && (!ModeIsThings)
                    && ((ModeIsEditor || ModeIsThings) || TheMap.Coordinates [Player.X, Player.Y + 1] > -1) && !TheMap.IsThingAt(Player.X, Player.Y + 1))
                    {
                    Player.Y++;
                    }
                else if (ModeIsThings)
                    {
                    Thing thing = TheMap.Things [ThingIndex];
                    if (thing.Y + thing.Depth < TheMap.Height)
                        {
                        int ind = TheMap.Things.IndexOf(thing);
                        TheMap.Things [ind].Y++;
                        }
                    }
                else if (Player.Y + 1 == TheMap.Height
                    && (ModeIsEditor || OuterMaps [1, 0].Coordinates [Player.X, 0] > -1))
                    {
                    for (int x = 0; x < 3; x++)
                        {
                        for (int y = 1; y > -1; y--)
                            {
                            OuterMaps [x, y + 1] = OuterMaps [x, y];
                            }
                        }

                    int cx = WorldMapX - 1;
                    for (int x = 0; x < 3; x++)
                        {
                        OuterMaps [x, 0] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", configPath, cx + x, WorldMapY - 2));
                        if (OuterMaps [x, 0] != null)
                            {
                            OuterMaps [x, 0].WorldX = cx + x;
                            OuterMaps [x, 0].WorldY = WorldMapY - 2;
                            }
                        }

                    OuterMaps [1, 2] = TheMap;
                    TheMap = OuterMaps [1, 1];
                    WorldMapY--;
                    Player.Y = 0;
                    }
                }
            #endregion

            #region MOVE LEFT
            if (Keyboard [Key.Left])
                {
                // Character direction
                CharDirection = CharacterTexture.NextLeft(CharDirection);
                //
                if (Player.X > 0
                    && (!ModeIsThings)
                    && ((ModeIsEditor || ModeIsThings) || TheMap.Coordinates [Player.X - 1, Player.Y] > -1) && !TheMap.IsThingAt(Player.X - 1, Player.Y))
                    {

                    Player.X--;
                    }
                else if (ModeIsThings)
                    {
                    Thing thing = TheMap.Things [ThingIndex];
                    if (thing.X > 0)
                        {
                        int ind = TheMap.Things.IndexOf(thing);
                        TheMap.Things [ind].X--;
                        }
                    }
                else if (Player.X == 0
                    && (ModeIsEditor || OuterMaps [0, 1].Coordinates [OuterMaps [0, 1].Width - 1, Player.Y] > -1))
                    {
                    for (int y = 0; y < 3; y++)
                        {
                        for (int x = 1; x > -1; x--)
                            {
                            OuterMaps [x + 1, y] = OuterMaps [x, y];
                            }
                        }

                    int cy = WorldMapY - 1;
                    for (int y = 0; y < 3; y++)
                        {
                        OuterMaps [0, y] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", configPath, WorldMapX - 2, cy + y));
                        if (OuterMaps [0, y] != null)
                            {
                            OuterMaps [0, y].WorldX = WorldMapX - 2;
                            OuterMaps [0, y].WorldY = cy + y;
                            }
                        }

                    OuterMaps [2, 1] = TheMap;
                    TheMap = OuterMaps [1, 1];
                    WorldMapX--;
                    Player.X = TheMap.Width - 1;
                    }

                }
            #endregion

            #region MOVE RIGHT
            if (Keyboard [Key.Right])
                {
                // Character direction
                CharDirection = CharacterTexture.NextRight(CharDirection);
                //
                if (Player.X + 1 < TheMap.Width
                    && (!ModeIsThings)
                    && ((ModeIsEditor || ModeIsThings) || TheMap.Coordinates [Player.X + 1, Player.Y] > -1) && !TheMap.IsThingAt(Player.X + 1, Player.Y))
                    {
                    Player.X++;
                    }
                else if (ModeIsThings)
                    {
                    Thing thing = TheMap.Things [ThingIndex];
                    if (thing.X + thing.Width < TheMap.Width)
                        {
                        int ind = TheMap.Things.IndexOf(thing);
                        TheMap.Things [ind].X++;
                        }
                    }
                else if (Player.X + 1 == TheMap.Width
                    && (ModeIsEditor || OuterMaps [2, 1].Coordinates [0, Player.Y] > -1))
                    {
                    for (int y = 0; y < 3; y++)
                        {
                        for (int x = 0; x < 2; x++)
                            {
                            OuterMaps [x, y] = OuterMaps [x + 1, y];
                            }
                        }

                    int cy = WorldMapY - 1;
                    for (int y = 0; y < 3; y++)
                        {
                        OuterMaps [2, y] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", configPath, WorldMapX + 2, cy + y));
                        if (OuterMaps [2, y] != null)
                            {
                            OuterMaps [2, y].WorldX = WorldMapX + 2;
                            OuterMaps [2, y].WorldY = cy + y;
                            }
                        }

                    OuterMaps [0, 1] = TheMap;
                    TheMap = OuterMaps [1, 1];
                    WorldMapX++;
                    Player.X = 0;
                    }
                }
            #endregion

            #region CYCLE ITEMS
            if ((Keyboard [Key.Period] || Keyboard [Key.Comma]))
            {
                int next;
                if (ModeIsEditor)
                {
                    //next = Texture.DefaultMapTexts.FindIndex(i => i.Value == TileIndex);
                    next = TheMap.Textures.FindIndex(i => i.Value == TileIndex);
                    if (Keyboard [Key.Period]) {
                        next++;}
                    else if (Keyboard [Key.Comma]){
                        next--;}

                    if (next < 0){
                        next = TheMap.Textures.Count - 1;}
                    else if (TheMap.Textures.Count - 1 < next){
                        next = 0;}

                    //TheMap.Coordinates [Player.X, Player.Y] = Texture.DefaultMapTexts [next].Value;
                    TheMap.Coordinates [Player.X, Player.Y] = TheMap.Textures [next].Value;
                }
                else if (ModeIsThings)
                    {
                    next = ThingIndex;
               
   

                    if (Keyboard [Key.Period]) {
                        next++;}
                    else if (Keyboard [Key.Comma]){
                        next--;}

                    if (next < 0){
                        next = TheMap.Things.Count - 1;}
                    else if (TheMap.Things.Count - 1 < next){
                        next = 0;}

                    ThingIndex = next;
                }
            }
            #endregion

            #region CYCLE THING TYPES
            if (Keyboard [Key.Semicolon] || Keyboard [Key.Quote])
            {

                int next = Thing.AllThings.FindIndex(x => x.GetType() == TheMap.Things [ThingIndex].GetType());
                if (ModeIsThings)
                {
                    if (Keyboard [Key.Semicolon])
                    {
                        next--;
                    }
                    else if (Keyboard [Key.Quote])
                    {
                        next++;
                    }

                    if (next < 0) {
                        next = Thing.AllThings.Count - 1; }
                    else if (next == Thing.AllThings.Count) {
                        next = 0; }

                    object obthing = System.Activator.CreateInstance(Thing.AllThings [next].GetType());
                    Thing newthing = (Thing)obthing;

                    newthing.X = TheMap.Things [ThingIndex].X;
                    newthing.Y = TheMap.Things [ThingIndex].Y;
                    TheMap.Things [ThingIndex] = newthing;
                }
            }

            #endregion

            #region SAVE
            if (Keyboard [Key.S]
                && (Keyboard [Key.ControlLeft] || Keyboard [Key.ControlRight]))
                {
                Console.WriteLine("Saved Map: {0}", TheMap.Save());
                }
            #endregion
        }
    }
}

