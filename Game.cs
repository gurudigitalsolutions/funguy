// Released to the public domain. Use, modify and relicense at will.
using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;

using System.Collections.Generic;

using FunGuy;


namespace FunGuy
{

    class Game : GameWindow
    {
        public static Game Engine;
        public Map TheMap;
        public Player[] Characters = new Player[10];
//        {
//            get { return OuterMaps [1, 1]; }
//            set { OuterMaps [1, 1] = value; }
//        }



        public Map[,] OuterMaps = new Map[3, 3];
//        public Player Player;
        public int TimeStamp;
        public int CharIndex = 0;
        public int CharDirection = 0;


        public int TileIndex
        {
            get { return TheMap.Coordinates [Player.X, Player.Y];}
        }


        public int ThingIndex = 0;
        public int GameMode = GameModes.Game;

        public Modes.Explorer ModeExplorer = new FunGuy.Modes.Explorer();
        public Modes.Thing ModeThing = new FunGuy.Modes.Thing();
        //public Modes.ThingEdit ModeThingEdit = new FunGuy.Modes.ThingEdit();
        public Modes.TileEdit ModeTileEdit = new FunGuy.Modes.TileEdit();

        public int WorldMapWidth = 5;
        public int WorldMapHeight = 5;
        public int WorldMapY = 2;
        public int WorldMapX = 2;
        public int MapEdgeTexture = 0;
        public static string configPath = "../../Resources";
        public const int GameModeGame = 0;
        public const int GameModeEditor = 1;
        public const int GameModeThings = 2;
        public const int GameModeThingEditor = 3;




        /// <summary>
        /// Gets a value indicating whether this <see cref="FunGuy.Game"/> mode is game.
        /// </summary>
        /// <value>
        /// <c>true</c> if mode is game; otherwise, <c>false</c>.
        /// </value>
        public bool ModeIsGame { get { return (GameMode == GameModes.Game); } }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FunGuy.Game"/> mode is editor.
        /// </summary>
        /// <value>
        /// <c>true</c> if mode is editor; otherwise, <c>false</c>.
        /// </value>
        public bool ModeIsEditor { get { return (GameMode == GameModes.Editor); } }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FunGuy.Game"/> mode is things.
        /// </summary>
        /// <value>
        /// <c>true</c> if mode is things; otherwise, <c>false</c>.
        /// </value>
        public bool ModeIsThings { get { return (GameMode == GameModes.Things); } }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FunGuy.Game"/> mode is things editor.
        /// </summary>
        /// <value>
        /// <c>true</c> if mode is things editor; otherwise, <c>false</c>.
        /// </value>
        public bool ModeIsThingsEditor { get { return (GameMode == GameModes.ThingEditor); } }


        /// <summary>
        /// Initializes a new instance of the <see cref="FunGuy.Game"/> class.
        /// Creates a 800x600 window with the specified title.
        /// </summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "Fun Guy RPG")
        {
            VSync = VSyncMode.On;
        }


        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name='e'>
        /// E. event arguments
        /// </param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //FGarbage.ConvertMaps();
            //Environment.Exit(0);
            GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            TexLib.InitTexturing();

            MapEdgeTexture = TexLib.CreateRGBATexture(32, 32, new byte[]{92, 255, 0, 0,
                                                                        92, 255, 0, 0,
                                                                        92, 255, 0, 255,
                92, 255, 0, 255}
            );



            TimeStamp = Environment.TickCount;

            TheMap = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", configPath, WorldMapX, WorldMapY));
            TheMap.WorldX = WorldMapX;
            TheMap.WorldY = WorldMapY;



            int cx = 0;
            for (int ex = WorldMapX - 1; ex < WorldMapX + 2; ex++)
            {
                if (ex < 0)
                { 
                    continue;
                }
                int cy = 0;
                for (int ey = WorldMapY - 1; ey < WorldMapY + 2; ey++)
                {
                    if (ey < 0)
                    {
                        continue;
                    }
                    OuterMaps [cx, cy] = Map.Loader(string.Format("{0}/Maps/{1}/{2}.map", configPath, ex, ey));
                    OuterMaps [cx, cy].WorldX = ex;
                    OuterMaps [cx, cy].WorldY = ey;

                    cy++;
                }
                cx++;
            }

            Player.X = TheMap.StartX;
            Player.Y = TheMap.StartY;
           

//            House newhouse = new House("woodpanel");
//            newhouse.X = 30;
//            newhouse.Y = 30;
//            //TheMap.Things.Add(newhouse);
//            TheMap.AddThing(newhouse);
//
//            Tree newtree = new Tree("pinetree");
//            newtree.X = 36;
//            newtree.Y = 36;
//            newtree.Depth = 2;
//            newtree.Width = 2;
//            newtree.Height = 4;
//            //TheMap.Things.Add(newtree);
//            TheMap.AddThing(newtree);
            Input.Keyboard keyboard = new Input.Keyboard();

          
            Console.WriteLine("{0} {1} {2} {3}", Key.A.ToString(), Key.Enter, Key.B, Key.BackSpace);
        }



        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }



        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            if(ModeIsGame) { ModeExplorer.Update(Keyboard); }
            else if(ModeIsEditor) { ModeTileEdit.Update(Keyboard); }
            else if(ModeIsThings) { ModeThing.Update(Keyboard); }
            //else if(ModeIsThingsEditor) { ModeThingEdit.Update(Keyboard); }

            //OnKeyPress();
        }



        /// <summary>
        /// Raises the key press event.
        /// </summary>
        protected void OnKeyPress()
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



        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Matrix4 modelview = Matrix4.LookAt (Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);

            Matrix4 modelview;
            if (ModeIsThings)
            {
                Thing thing = TheMap.Things [ThingIndex];

                float thingx = (float)thing.X;
                float thingy = (float)thing.Y;
                float thingwidth = (float)thing.Width;
                float thingdepth = (float)thing.Depth;


                modelview = Matrix4.LookAt(new Vector3(thingx + (thingwidth / 2), thingy + (thingdepth / 2) - 4.0f, (float)16), // Camera
                                           new Vector3(thingx + (thingwidth / 2), thingy + (thingdepth / 2), (float)4), //  Look at
                            Vector3.UnitY);
            }
            else
            {
                modelview = Matrix4.LookAt(new Vector3((float)Player.X + 0.5f, (float)Player.Y - 5.5f, (float)16),  // Camera
                                               new Vector3((float)Player.X + 0.5f, (float)Player.Y + 0.5f, (float)4),   // Look At
                                               Vector3.UnitY);
            }

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

//          GL.Begin (BeginMode.Triangles);
//
//          GL.Color3 (0.5f, 0.5f, 0.5f);
//          GL.Vertex3 (-1.0f, -1.0f, 4.0f);
//          GL.Color3 (1.0f, 0.0f, 0.0f);
//          GL.Vertex3 (1.0f, -1.0f, 4.0f);
//          GL.Color3 (0.2f, 0.9f, 1.0f);
//          GL.Vertex3 (0.0f, 1.0f, 4.0f);
//
//          GL.End ();

            for (int wmx = 0; wmx < 3; wmx++)
            {
                for (int wmy = 0; wmy < 3; wmy++)
                {
                    if (wmx > -1 && wmx < WorldMapWidth && wmy > -1 && wmy < WorldMapHeight && (wmx != 1 || wmy != 1))
                    {
                        if (OuterMaps [wmx, wmy] == null)
                        {
                            //Console.WriteLine("Outer Map {0} {1} null", wmx, wmy);
                        }
                        else
                        {
                            for (int x = 0; x < OuterMaps[wmx,wmy].Width; x++)
                            {
                                for (int y = 0; y < OuterMaps[wmx,wmy].Height; y++)
                                {
//                                    int plusplusy = 0;
//                                    if (wmy == 0)
//                                    {
//                                        plusplusy = 64;
//                                    }
//                                    if (wmy == 1)
//                                    {
//                                        plusplusy = 0;
//                                    }
//                                    if (wmy == 2)
//                                    {
//                                        plusplusy = -64;
//                                    }

                                    int plusplusx = -64;

                                    GL.BindTexture(TextureTarget.Texture2D, OuterMaps [wmx, wmy].Textures.Find(v => v.Value == OuterMaps [wmx, wmy].Coordinates [x, y]).TexLibID);

                                    GL.Begin(BeginMode.Quads);
                                    GL.Normal3(-1.0f, 0.0f, 0.0f);


                                    GL.TexCoord2(0.0f, 1.0f);
                                    GL.Vertex3((float)x + (wmx * 64) + plusplusx, (float)y + 64 - (wmy * 64), 4.0f);
                                    GL.TexCoord2(1.0f, 1.0f);
                                    GL.Vertex3((float)x + (wmx * 64) + 1 + plusplusx, (float)y + 64 - (wmy * 64), 4.0f);
                                    GL.TexCoord2(1.0f, 0.0f);
                                    GL.Vertex3((float)x + (wmx * 64) + 1 + plusplusx, (float)y + 64 - (wmy * 64) + 1, 4.0f);
                                    GL.TexCoord2(0.0f, 0.0f);
                                    GL.Vertex3((float)x + (wmx * 64) + plusplusx, (float)y + 64 - (wmy * 64) + 1, 4.0f);

                                    GL.End();
                                }
                            }
                        }
                    }
                }

            } 
            for (int x = 0; x < TheMap.Width; x++)
            {
                for (int y = 0; y < TheMap.Height; y++)
                {
                    GL.BindTexture(TextureTarget.Texture2D, TheMap.Textures.Find(v => v.Value == TheMap.Coordinates [x, y]).TexLibID);

                    GL.Begin(BeginMode.Quads);
                    GL.Normal3(-1.0f, 0.0f, 0.0f);


                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3((float)x, (float)y, 4.0f);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3((float)x + 1, (float)y, 4.0f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3((float)x + 1, (float)y + 1, 4.0f);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3((float)x, (float)y + 1, 4.0f);

                    GL.End();
                }

            }



            if (ModeIsEditor)
            {
                GL.BindTexture(TextureTarget.Texture2D, MapEdgeTexture);

                for (int y = -64; y < 128; y++)
                {
                    GL.Begin(BeginMode.Quads);
                    GL.Normal3(-1.0f, 0.0f, 0.0f);

                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(-0.1f, (float)y, 4.1f);
                    GL.TexCoord2(1.0f, 1.1f);
                    GL.Vertex3(0.1f, (float)y, 4.1f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(0.1f, (float)(y + 1), 4.1f);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(-0.1, (float)(y + 1), 4.1f);
                    GL.End();

                    GL.Begin(BeginMode.Quads);
                    GL.Normal3(-1.0f, 0.0f, 0.0f);

                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(63.6f, -64f, 4.3f);
                    GL.TexCoord2(1.0f, 1.1f);
                    GL.Vertex3(64.4f, -64f, 4.3f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(64.4f, 128f, 4.3f);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(63.6f, 128f, 4.3f);
                    GL.End();
                }
            }

    
            // Load all textures in rear of char
            TheMap.RenderThings(Player.Y, TheMap.Height);

            if (ModeIsGame)
            {
                CharIndex = 0;
            }
            else
            {
                CharIndex = -2;
            }

            // Draws the char
            ChangeCharacter(CharIndex, CharDirection);

            // Load all textures in front of char
            TheMap.RenderThings(0, Player.Y);

            GL.End();

//          GL.BindTexture (TextureTarget.Texture2D, TheMap.Textures ["grass"]);
//
//          GL.Begin (BeginMode.Quads);
//          GL.Normal3 (-1.0f, 0.0f, 0.0f);
//
//
//          GL.TexCoord2 (0.0f, 0.0f);
//          GL.Vertex3 (-1.0f, -1.0f, 4.0f);
//          GL.TexCoord2 (1.0f, 0.0f);
//          GL.Vertex3 (-1.5f, -1.0f, 4.0f);
//          GL.TexCoord2 (1.0f, 1.0f);
//          GL.Vertex3 (-1.5f, -1.5f, 4.0f);
//          GL.TexCoord2 (0.0f, 1.0f);
//          GL.Vertex3 (-1.0f, -1.5f, 4.0f);
//
//          GL.End ();



            SwapBuffers();
        }


        protected void ChangeCharacter(int value)
        {
            ChangeCharacter(value, 0);
        }


        protected void ChangeCharacter(int value, int index)
        {

            if (!ModeIsThings)
            {
                CharacterTexture myChar = Player.Characters.Find(c => c.Value == value && c.Index == index);
                //Console.WriteLine("MyChar: {0}", myChar.TexLibID);
                GL.BindTexture(TextureTarget.Texture2D, myChar.TexLibID);
                GL.Begin(BeginMode.Quads);
                GL.Normal3(-1.0f, 0.0f, 0.0f);

                if (ModeIsGame)
                {

                    // left top, right top, right bottom, left bottom
                    // left bottom is 0 0
                   
                    float ly = Player.Y;
                    float ry = Player.Y;
                    float lx = Player.X;
                    float rx = (float)(Player.X + 1);

                    if (CharDirection == CharacterTexture.Down || 
                        CharDirection == CharacterTexture.DownTwo ||
                        CharDirection == CharacterTexture.DownThree ||
                        CharDirection == CharacterTexture.Up ||
                        CharDirection == CharacterTexture.UpTwo ||
                        CharDirection == CharacterTexture.UpThree)
                    {
                        ly = ly + 0.5f;
                        ry = ry + 0.5f;
                    }
                    else if (CharDirection == CharacterTexture.Left ||
                        CharDirection == CharacterTexture.LeftTwo ||
                        CharDirection == CharacterTexture.LeftThree)
                    {
                        ly = ly + 0.8f;
                        ry = ry + 0.2f;

                        lx = lx + 0.3f;
                        rx = rx - 0.3f;
                    }
                    else if (CharDirection == CharacterTexture.Right ||
                        CharDirection == CharacterTexture.RightTwo ||
                        CharDirection == CharacterTexture.RightThree)
                    {
                        ly = ly + 0.2f;
                        ry = ry + 0.8f;

                        lx = lx + 0.3f;
                        rx = rx - 0.3f;
                    }

                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3(lx, ly, 4.05f);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3(rx, ry, 4.05f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3(rx, ry, 4.05f + myChar.Height);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3(lx, ly, 4.05f + myChar.Height);
                }
                else
                {
                    // left top, right top, right bottom, left bottom
                    // left bottom is 0 0
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3((float)Player.X, (float)Player.Y, 4.05f);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3((float)Player.X + 1, (float)Player.Y, 4.05f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3((float)Player.X + 1, (float)Player.Y + 1f, 4.05f + myChar.Height);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3((float)Player.X, (float)Player.Y + 1f, 4.05f + myChar.Height);
                }
                GL.End();
            }
        }




        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // The 'using' idiom guarantees proper resource cleanup.
            // We request 30 UpdateFrame events per second, and unlimited
            // RenderFrame events (as fast as the computer can handle).

            using(Game.Engine = new Game())
            {
                Game.Engine.Run(30.0);
            }
         
        }


       

    
    }



    public static class GameModes
    {
        public const int Game = 0;
        public const int Editor = 1;
        public const int Things = 2;
        public const int ThingEditor = 3;
    }



}