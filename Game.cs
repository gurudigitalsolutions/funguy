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
        public Map[,] OuterMaps = new Map[3, 3];

        public Player[] Party = new Player[10];

        public int TimeStamp;
        public int LastTimeStamp;

        public int CharIndex = 0;
        //public int CharDirection = 0;
        public int EditorGridTex = 0;

        public int TileIndex
        {
            get { return TheMap.Coordinates [Party[0].X, Party[0].Y];}
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
            GL.Enable(EnableCap.Blend);

            TexLib.InitTexturing();

            EditorGridTex = TexLib.CreateTextureFromFile(string.Format("{0}/PNGs/Misc/editsquare.png", configPath));
            MapEdgeTexture = TexLib.CreateRGBATexture(32, 32, new byte[]{92, 255, 0, 0,
                                                                        92, 255, 0, 255,
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

            Party[0] = new Player("trip");
            Party[0].X = TheMap.StartX;
            Party[0].Y = TheMap.StartX;
           
            Party[1] = new Player("crystal");
            Party[1].X = TheMap.StartX - 1;
            Party[1].Y = TheMap.StartY;

            for(int ep = 2; ep < 5; ep++)
            {
                Party[ep] = new Player("crystal");
                Party[ep].X = Party[ep - 1].X - 1;
                Party[ep].Y = Party[ep - 1].Y;
            }

//            Fluid newwater = new Fluid("water");
//            newwater.X = 40;
//            newwater.Y = 29;
//            newwater.Width = 5;
//            newwater.Depth = 3;
//            TheMap.AddThing(newwater);
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

          
            LastTimeStamp = Environment.TickCount;
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

            LastTimeStamp = Environment.TickCount;
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
                modelview = Matrix4.LookAt(new Vector3((float)Party[0].X + 0.5f, (float)Party[0].Y - 5.5f, (float)16),  // Camera
                                               new Vector3((float)Party[0].X + 0.5f, (float)Party[0].Y + 0.5f, (float)4),   // Look At
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
            TheMap.RenderThings(Party[0].Y, TheMap.Height);

            if (ModeIsGame)
            {
                CharIndex = 0;
            }
            else
            {
                CharIndex = -2;
            }

            // Draws the char
            ChangeCharacter(CharIndex, Party[0].Direction);

            // Load all textures in front of char
            TheMap.RenderThings(0, Party[0].Y);

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


                if (ModeIsGame)
                {
                    List<Player> lparty = new List<Player>();
                    lparty.AddRange(Party);
                    lparty.RemoveAll(a => a == null);
                    lparty.Sort((a,b)=> a.Y.CompareTo(b.Y));
                    lparty.Reverse();

                    foreach (Player eplayer in lparty)
                    {

                        eplayer.Render();
                    }
                }
                else
                {
                    // left top, right top, right bottom, left bottom
                    // left bottom is 0 0
                    //CharacterTexture myChar = Party[0].Characters.Find(c => c.Value == value && c.Index == index);
                    //Console.WriteLine("MyChar: {0}", myChar.TexLibID);
                    GL.BindTexture(TextureTarget.Texture2D, EditorGridTex);
                    GL.Begin(BeginMode.Quads);
                    GL.Normal3(-1.0f, 0.0f, 0.0f);
                    GL.TexCoord2(0.0f, 1.0f);
                    GL.Vertex3((float)Party[0].X, (float)Party[0].Y, 4.05f);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex3((float)Party[0].X + 1, (float)Party[0].Y, 4.05f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex3((float)Party[0].X + 1, (float)Party[0].Y + 1f, 4.05f);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex3((float)Party[0].X, (float)Party[0].Y + 1f, 4.05f);

                    GL.End();
                }

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