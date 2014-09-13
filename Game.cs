// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenTK.Input;
using FunGuy;
using System.Collections.Generic;

namespace StarterKit
{
	class Game : GameWindow
	{
		public Map TheMap;
		public Map[,] OuterMaps = new Map[3, 3];
		public Player ThePlayer;
		public int TimeStamp;

		public static int GameModeGame = 0;
		public static int GameModeEditor = 1;

		public int GameMode = GameModeGame;

		public int CurrCharValue = 0;
		public int CurrCharIndex = 0;

		public int WorldMapWidth = 5;
		public int WorldMapHeight = 5;
		public int WorldMapY = 2;
		public int WorldMapX = 2;

		public int MapEdgeTexture = 0;

		public static string configPath = string.Format("{0}/{1}", Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "FunGuy");

		/// <summary>Creates a 800x600 window with the specified title.</summary>
		public Game()
            : base(800, 600, GraphicsMode.Default, "Fun Guy RPG")
		{
			VSync = VSyncMode.On;
		}

		/// <summary>Load resources here.</summary>
		/// <param name="e">Not used.</param>
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

			configPath = "/share/code/c#/FunGuy/Resources";

			if (!System.IO.Directory.Exists(configPath))
			{
				Console.WriteLine("Created configuration path: {0}", configPath);
				System.IO.Directory.CreateDirectory(configPath);
			}

			TimeStamp = Environment.TickCount;


			//TheMap = new Map("Map1", "default", 64, 64);
			//TheMap.WorldX = 2;
			//TheMap.WorldY = 2;
			//TheMap.Save();
			TheMap = Map.Loader(string.Format("{2}/Maps/{0}_{1}.map", WorldMapX, WorldMapY, configPath));
			TheMap.WorldX = WorldMapX;
			TheMap.WorldY = WorldMapY;

//			Thing something = new DefaultHouse(
//				TheMap.Textures.Find(x => x.Name == "lava1").TexLibID, 
//				TheMap.Textures.Find(x => x.Name == "lava1").TexLibID, 
//				TheMap.Textures.Find(x => x.Name == "lava1").TexLibID, 
//				TheMap.Textures.Find(x => x.Name == "lava1").TexLibID, 
//				TheMap.Textures.Find(x => x.Name == "lava1").TexLibID, 
//				TheMap.Textures.Find(x => x.Name == "rockroad1").TexLibID);
//			something.Depth = 2;
//			something.Height = 2;
//			something.Width = 2;
//			something.X = 25;
//			something.Y = 25;
//
//			TheMap.Things.Add(something);

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
					OuterMaps [cx, cy] = Map.Loader(string.Format("{2}/Maps/{0}_{1}.map", ex, ey, configPath));
					OuterMaps [cx, cy].WorldX = ex;
					OuterMaps [cx, cy].WorldY = ey;

					cy++;
				}
				cx++;
			}

			ThePlayer = new Player();
			ThePlayer.PosX = TheMap.StartX;
			ThePlayer.PosY = TheMap.StartY;
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

			OnKeyPress();
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

			if (ThePlayer.CanMove())
			{

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

				if (Keyboard [Key.Space])
				{

					if (GameMode == GameModeGame)
					{
						GameMode = GameModeEditor;
					} else
					{
						if (TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY] > -1)
						{
							GameMode = GameModeGame;
						}
					}
				}



				if (Keyboard [Key.Down])
				{
					foreach (MyCharacter item in ThePlayer.Characters.FindAll(c=> c.Value == CurrCharValue))
					{
						CurrCharIndex = 0;
					}
					if (ThePlayer.PosY > 0
						&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY - 1] > -1))
					{
						ThePlayer.PosY--;
					}
					else if (ThePlayer.PosY == 0
						&& (GameMode == GameModeEditor || OuterMaps [1, 2].Coordinates [ThePlayer.PosX, OuterMaps [1, 2].Height - 1] > -1))
					{
						OuterMaps [0, 0].UnloadTextures();
						OuterMaps [1, 0].UnloadTextures();
						OuterMaps [2, 0].UnloadTextures();

						for (int x = 0; x < 3; x++)
						{
							for (int y = 1; y < 3; y++)
							{
								OuterMaps [x, y - 1] = OuterMaps [x, y];
							}
						}
						//OuterMaps[0, 1] = TheMap;

						int cx = WorldMapX - 1;
						for (int x = 0; x < 3; x++)
						{
							OuterMaps [x, 2] = Map.Loader(string.Format("{0}/Maps/{1}_{2}.map", configPath, cx + x, WorldMapY + 2));
							if (OuterMaps [x, 2] != null)
							{
								OuterMaps [x, 2].WorldX = cx + x;
								OuterMaps [x, 2].WorldY = WorldMapY + 2;
							}
						}

						OuterMaps [1, 0] = TheMap;
						TheMap = OuterMaps [1, 1];
						WorldMapY++;
						ThePlayer.PosY = TheMap.Height - 1;

					}
				}

				if (Keyboard [Key.Up])
				{
					if (ThePlayer.PosY + 1 < TheMap.Height
						&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY + 1] > -1))
					{
						foreach (MyCharacter item in ThePlayer.Characters.FindAll(c=> c.Value == CurrCharValue))
						{
							if (item.Index == 1)
							{
								CurrCharIndex = item.Index;
							}
						}
						ThePlayer.PosY++;
					}
					else if (ThePlayer.PosY + 1 == TheMap.Height
						&& (GameMode == GameModeEditor || OuterMaps [1, 0].Coordinates [ThePlayer.PosX, 0] > -1))
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
							OuterMaps [x, 0] = Map.Loader(string.Format("{0}/Maps/{1}_{2}.map", configPath, cx + x, WorldMapY - 2));
							if (OuterMaps [x, 0] != null)
							{
								OuterMaps [x, 0].WorldX = cx + x;
								OuterMaps [x, 0].WorldY = WorldMapY - 2;
							}
						}

						OuterMaps [1, 2] = TheMap;
						TheMap = OuterMaps [1, 1];
						WorldMapY--;
						ThePlayer.PosY = 0;
					}
				}

				if (Keyboard [Key.Left])
				{
					if (ThePlayer.PosX > 0
						&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.PosX - 1, ThePlayer.PosY] > -1))
					{
						foreach (MyCharacter item in ThePlayer.Characters.FindAll(c=> c.Value == CurrCharValue))
						{
							if (item.Index == 2)
							{
								CurrCharIndex = item.Index;
							}
						}
						ThePlayer.PosX--;
					}
					else if (ThePlayer.PosX == 0
						&& (GameMode == GameModeEditor || OuterMaps [0, 1].Coordinates [OuterMaps [0, 1].Width - 1, ThePlayer.PosY] > -1))
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
							OuterMaps [0, y] = Map.Loader(string.Format("{0}/Maps/{1}_{2}.map", configPath, WorldMapX - 2, cy + y));
							if (OuterMaps [0, y] != null)
							{
								OuterMaps [0, y].WorldX = WorldMapX - 2;
								OuterMaps [0, y].WorldY = cy + y;
							}
						}

						OuterMaps [2, 1] = TheMap;
						TheMap = OuterMaps [1, 1];
						WorldMapX--;
						ThePlayer.PosX = TheMap.Width - 1;
					}

				}

				if (Keyboard [Key.Right])
				{
					if (ThePlayer.PosX + 1 < TheMap.Width
						&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.PosX + 1, ThePlayer.PosY] > -1))
					{
						foreach (MyCharacter item in ThePlayer.Characters.FindAll(c=> c.Value == CurrCharValue))
						{
							if (item.Index == 3)
							{
								CurrCharIndex = item.Index;
							}
						}
						ThePlayer.PosX++;
					}
					else if (ThePlayer.PosX + 1 == TheMap.Width
						&& (GameMode == GameModeEditor || OuterMaps [2, 1].Coordinates [0, ThePlayer.PosY] > -1))
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
							OuterMaps [2, y] = Map.Loader(string.Format("{0}/Maps/{1}_{2}.map", configPath, WorldMapX + 2, cy + y));
							if (OuterMaps [2, y] != null)
							{
								OuterMaps [2, y].WorldX = WorldMapX + 2;
								OuterMaps [2, y].WorldY = cy + y;
							}
						}

						OuterMaps [0, 1] = TheMap;
						TheMap = OuterMaps [1, 1];
						WorldMapX++;
						ThePlayer.PosX = 0;
					}
				}

				/* Game Editor Mode */
				if (GameMode == GameModeEditor)
				{
					// Modify current tile
					if (Keyboard [Key.Period] || Keyboard [Key.Comma])
					{
						// Get the current texture index
						int curr = TheMap.Textures.Find(t => t.Value == TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY]).Index;
						int next = curr;

						if (Keyboard [Key.Period])
						{
							next = curr + 1;
						}

						if (Keyboard [Key.Comma])
						{
							next = curr - 1;
						}

						if (next > TheMap.Textures.Count)
						{
							next = 1;
						}
						if (next < 1)
						{
							next = TheMap.Textures.Count;
						}

						Console.WriteLine(next);
						TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY] = TheMap.Textures.Find(n => n.Index == next).Value;
					}

					if (Keyboard [Key.S]
						&& (Keyboard [Key.ControlLeft] || Keyboard [Key.ControlRight]))
					{
						Console.WriteLine("Saved Map: {0}", TheMap.Save());
					}
				}
			}
		}

		/// Called when it is time to render the next frame. Add your rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//Matrix4 modelview = Matrix4.LookAt (Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
			Matrix4 modelview = Matrix4.LookAt(new Vector3((float)ThePlayer.PosX + 0.5f, (float)ThePlayer.PosY - 5.5f, (float)16),  // Camera
			                                   new Vector3((float)ThePlayer.PosX + 0.5f, (float)ThePlayer.PosY + 0.5f, (float)4),	// Look At
			                                   Vector3.UnitY);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);

//			GL.Begin (BeginMode.Triangles);
//
//			GL.Color3 (0.5f, 0.5f, 0.5f);
//			GL.Vertex3 (-1.0f, -1.0f, 4.0f);
//			GL.Color3 (1.0f, 0.0f, 0.0f);
//			GL.Vertex3 (1.0f, -1.0f, 4.0f);
//			GL.Color3 (0.2f, 0.9f, 1.0f);
//			GL.Vertex3 (0.0f, 1.0f, 4.0f);
//
//			GL.End ();

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
									int plusplusy = 0;
									if (wmy == 0)
									{
										plusplusy = 64;
									}
									if (wmy == 1)
									{
										plusplusy = 0;
									}
									if (wmy == 2)
									{
										plusplusy = -64;
									}

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

			TheMap.RenderThings();

			if (GameMode == GameModeEditor)
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

			if (GameMode == GameModeGame)
			{
				CurrCharValue = 0;
			} else
			{
				CurrCharValue = -2;
			}
			ChangeCharacter(CurrCharValue, CurrCharIndex);



			GL.End();

//			GL.BindTexture (TextureTarget.Texture2D, TheMap.Textures ["grass"]);
//
//			GL.Begin (BeginMode.Quads);
//			GL.Normal3 (-1.0f, 0.0f, 0.0f);
//
//
//			GL.TexCoord2 (0.0f, 0.0f);
//			GL.Vertex3 (-1.0f, -1.0f, 4.0f);
//			GL.TexCoord2 (1.0f, 0.0f);
//			GL.Vertex3 (-1.5f, -1.0f, 4.0f);
//			GL.TexCoord2 (1.0f, 1.0f);
//			GL.Vertex3 (-1.5f, -1.5f, 4.0f);
//			GL.TexCoord2 (0.0f, 1.0f);
//			GL.Vertex3 (-1.0f, -1.5f, 4.0f);
//
//			GL.End ();



			SwapBuffers();
		}
		protected void ChangeCharacter(int value)
		{
			ChangeCharacter(value, 0);
		}

		protected void ChangeCharacter(int value, int index)
		{
			MyCharacter myChar = ThePlayer.Characters.Find(c => c.Value == value && c.Index == index);
			GL.BindTexture(TextureTarget.Texture2D, myChar.TexLibID);
			GL.Begin(BeginMode.Quads);
			GL.Normal3(-1.0f, 0.0f, 0.0f);


			// left top, right top, right bottom, left bottom
			// left bottom is 0 0
			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3((float)ThePlayer.PosX, (float)ThePlayer.PosY, 4.05f);
			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3((float)ThePlayer.PosX + 1, (float)ThePlayer.PosY, 4.05f);
			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3((float)ThePlayer.PosX + 1, (float)ThePlayer.PosY + 1, 4.05f + myChar.Height);
			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3((float)ThePlayer.PosX, (float)ThePlayer.PosY + 1, 4.05f + myChar.Height);

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
			using (Game game = new Game())
			{
				game.Run(30.0);
			}
		}

	
	}
}