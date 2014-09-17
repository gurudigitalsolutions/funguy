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
		public static int GameModeThings = 2;
		public static int GameModeThingEditor = 3;

		public int GameMode = GameModeGame;

		public int CurrCharValue = 0;
		public int CurrCharDirection = 0;
		public int CurrThingIndex = 0;

		public int WorldMapWidth = 5;
		public int WorldMapHeight = 5;
		public int WorldMapY = 2;
		public int WorldMapX = 2;

		public int MapEdgeTexture = 0;

		public static string configPath = "../../Resources";



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

#if DEBUG
			configPath = "../../Resources";
#else
			configPath = "../../Resources";
#endif

			TimeStamp = Environment.TickCount;

			TheMap = Map.Loader(string.Format("{2}/Maps/{0}_{1}.map", WorldMapX, WorldMapY, configPath));
			TheMap.WorldX = WorldMapX;
			TheMap.WorldY = WorldMapY;

			InitEditorThings();


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

			//Console.WriteLine(" Width: {0}\t Height: {1}", TheMap.Width, TheMap.Height);
			//Console.ReadKey();
		}



		/// <summary>
		/// Inits the editor things.
		/// </summary>
		protected void InitEditorThings()
		{
			Thing pineTree = new PineTree(TheMap.Textures.Find(x => x.Name == "pine1").TexLibID);
			pineTree.Depth = 2;
			pineTree.Width = 2;
			pineTree.Height = 4;
			pineTree.X = 30;
			pineTree.Y = 30;

			Thing.AllThings.Add(pineTree);
//			TheMap.AddThing(pineTree);

			Thing defaultHouse = new DefaultHouse(
				TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
				TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
				TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
				TheMap.Textures.Find(x => x.Name == "woodpanel1").TexLibID, 
				TheMap.Textures.Find(x => x.Name == "thatch1").TexLibID, 
				TheMap.Textures.Find(x => x.Name == "rockroad1").TexLibID);
			defaultHouse.Depth = 2;
			defaultHouse.Height = 2;
			defaultHouse.Width = 2;
			defaultHouse.X = 25;
			defaultHouse.Y = 25;

			Thing.AllThings.Add(defaultHouse);
//			TheMap.AddThing(defaultHouse);
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
				Console.WriteLine("Position: X: {0} Y: {1} ", ThePlayer.PosX, ThePlayer.PosY);
				foreach (Thing et in TheMap.Things)
				{
					Console.WriteLine("Thing ID: {0}", et.Index);
				}
			}

			if (ThePlayer.CanMove())
			{
				/// Toggle edit
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

				if (Keyboard [Key.Number0])
				{
					//	Toggle thing explorer
					if (GameMode == GameModeEditor)
					{
						Console.WriteLine("Switched to Thing Explorer");
						GameMode = GameModeThings;


					}
					else if (GameMode == GameModeThings)
					{
						Console.WriteLine("Switched to Editor");
						GameMode = GameModeEditor;
					}
				}

				if (GameMode == GameModeThings)
				{
					int tind = TheMap.Things.IndexOf(TheMap.Things.Find(x => x.Index == CurrThingIndex));

					if (Keyboard [Key.S] && (!Keyboard [Key.ControlLeft] && !Keyboard [Key.ControlRight]))
					{
						TheMap.Things [tind].Height--;
					}
					else if (Keyboard [Key.W])
					{
						TheMap.Things [tind].Height++;
					}
					if (Keyboard [Key.D])
					{
						TheMap.Things [tind].Width--;
					}
					else if (Keyboard [Key.E])
					{
						TheMap.Things [tind].Width++;
					}
					if (Keyboard [Key.F])
					{
						TheMap.Things [tind].Depth--;
					}
					else if (Keyboard [Key.R])
					{
						TheMap.Things [tind].Depth++;
					}
					else if (Keyboard [Key.ControlLeft] && Keyboard [Key.V])
					{
						//	Paste a copy of this thing here
						Console.WriteLine("Pasting copy of this thing.");
						int indd = TheMap.Things.Find(x => x.Index == CurrThingIndex).Index;

						object otting = System.Activator.CreateInstance(TheMap.Things [indd].GetType());
						Thing tting = (Thing)otting;

						tting.X = TheMap.Things [indd].X;
						tting.Y = TheMap.Things [indd].Y;
						tting.Depth = TheMap.Things [indd].Depth;
						tting.Height = TheMap.Things [indd].Height;
						tting.Width = TheMap.Things [indd].Width;
						tting.SetTextures(TheMap.Things [indd].TextureList());

						TheMap.AddThing(tting);

						Console.WriteLine("ThingCount: {0}", TheMap.Things.Count);
						for (int et = 0; et < TheMap.Things.Count; et++)
						{
							Console.WriteLine("Thing Index: {0}", TheMap.Things [et].Index);
						}
					}
				}

				/// Down
				if (Keyboard [Key.Down])
				{
					// Characters direction
					CurrCharDirection = MyCharacter.Down;
					//
					if (ThePlayer.PosY > 0
						&& (GameMode != GameModeThings)
						&& (GameMode == GameModeEditor || (TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY - 1] > -1) && !TheMap.IsThingAt(ThePlayer.PosX, ThePlayer.PosY - 1)))
					{
						ThePlayer.PosY--;
					}
					else if (GameMode == GameModeThings)
					{
						Thing thing = TheMap.Things.Find(x => x.Index == CurrThingIndex);
						if (thing.Y > 0)
						{
							int ind = TheMap.Things.IndexOf(thing);
							TheMap.Things [ind].Y--;
						}
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
					// The characters direction
					CurrCharDirection = MyCharacter.Up;
					//
					if (ThePlayer.PosY + 1 < TheMap.Height
						&& (GameMode != GameModeThings)
						&& ((GameMode == GameModeEditor || GameMode == GameModeThings) || TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY + 1] > -1) && !TheMap.IsThingAt(ThePlayer.PosX, ThePlayer.PosY + 1))
					{
						ThePlayer.PosY++;
					}
					else if (GameMode == GameModeThings)
					{
						Thing thing = TheMap.Things.Find(x => x.Index == CurrThingIndex);
						if (thing.Y + thing.Depth < TheMap.Height)
						{
							int ind = TheMap.Things.IndexOf(thing);
							TheMap.Things [ind].Y++;
						}
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
					// Character direction
					CurrCharDirection = MyCharacter.Left;
					//
					if (ThePlayer.PosX > 0
						&& (GameMode != GameModeThings)
						&& ((GameMode == GameModeEditor || GameMode == GameModeThings) || TheMap.Coordinates [ThePlayer.PosX - 1, ThePlayer.PosY] > -1) && !TheMap.IsThingAt(ThePlayer.PosX - 1, ThePlayer.PosY))
					{

						ThePlayer.PosX--;
					}
					else if (GameMode == GameModeThings)
					{
						Thing thing = TheMap.Things.Find(x => x.Index == CurrThingIndex);
						if (thing.X > 0)
						{
							int ind = TheMap.Things.IndexOf(thing);
							TheMap.Things [ind].X--;
						}
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
					// Character direction
					CurrCharDirection = MyCharacter.Right;
					//
					if (ThePlayer.PosX + 1 < TheMap.Width
						&& (GameMode != GameModeThings)
						&& ((GameMode == GameModeEditor || GameMode == GameModeThings) || TheMap.Coordinates [ThePlayer.PosX + 1, ThePlayer.PosY] > -1) && !TheMap.IsThingAt(ThePlayer.PosX + 1, ThePlayer.PosY))
					{
						ThePlayer.PosX++;
					}
					else if (GameMode == GameModeThings)
					{
						Thing thing = TheMap.Things.Find(x => x.Index == CurrThingIndex);
						if (thing.X + thing.Width < TheMap.Width)
						{
							int ind = TheMap.Things.IndexOf(thing);
							TheMap.Things [ind].X++;
						}
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
				if (GameMode == GameModeEditor || GameMode == GameModeThings)
				{
					// Modify current tile
					if (Keyboard [Key.Period] || Keyboard [Key.Comma])
					{
						if (GameMode == GameModeEditor)
						{
							// Get the current texture index
							int curr = TheMap.Textures.Find(t => t.Value == TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY]).Index;
							int next = curr;

							if (Keyboard [Key.Period])
							{
								next = curr + 1;
							}
							else if (Keyboard [Key.Comma])
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

							//Console.WriteLine(next);
							TheMap.Coordinates [ThePlayer.PosX, ThePlayer.PosY] = TheMap.Textures.Find(n => n.Index == next).Value;
						}
						else if (GameMode == GameModeThings)
						{
							int curr = 0;
							curr = TheMap.Things.IndexOf(TheMap.Things.Find(x => x.Index == CurrThingIndex));

							int next = curr;
							if (Keyboard [Key.Period])
							{
								next++;
							}
							else if (Keyboard [Key.Comma])
							{
								next--;
							}

							//Console.WriteLine("Next thing number {0}", next);

							if (next >= TheMap.Things.Count)
							{
								next = 1;
							}
							if (next < 1)
							{
								next = TheMap.Things.Count;
							}

							Console.WriteLine("Next thing after math {0}", next);

							CurrThingIndex = TheMap.Things [next - 1].Index;
							//Console.WriteLine("CurrThingIndex changed to {0} {1} {2}", CurrThingIndex, next, TheMap.Things [next - 1].Index);
						}
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

			Matrix4 modelview;
			if (GameMode == GameModeThings)
			{
				Thing thing = TheMap.Things.Find(x => x.Index == CurrThingIndex);
				float thingx = (float)thing.X;
				float thingy = (float)thing.Y;
				float thingwidth = (float)thing.Width;
				float thingdepth = (float)thing.Depth;


				modelview = Matrix4.LookAt(new Vector3(thingx + (thingwidth / 2), thingy + (thingdepth / 2) - 4.0f, (float)16), // Camera
				                           new Vector3(thingx + (thingwidth / 2), thingy + (thingdepth / 2), (float)4), //	Look at
				            Vector3.UnitY);
			}
			else
			{
				modelview = Matrix4.LookAt(new Vector3((float)ThePlayer.PosX + 0.5f, (float)ThePlayer.PosY - 5.5f, (float)16),  // Camera
			                                   new Vector3((float)ThePlayer.PosX + 0.5f, (float)ThePlayer.PosY + 0.5f, (float)4),	// Look At
			                                   Vector3.UnitY);
			}

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

	
			// Load all textures in rear of char
			TheMap.RenderThings(ThePlayer.PosY, TheMap.Height);

			if (GameMode == GameModeGame)
			{
				CurrCharValue = 0;
			} else
			{
				CurrCharValue = -2;
			}

			// Draws the char
			ChangeCharacter(CurrCharValue, CurrCharDirection);

			// Load all textures in front of char
			TheMap.RenderThings(0, ThePlayer.PosY);

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

			if (GameMode != GameModeThings)
			{
				MyCharacter myChar = ThePlayer.Characters.Find(c => c.Value == value && c.Index == index);
				//Console.WriteLine("MyChar: {0}", myChar.TexLibID);
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
			using (Game game = new Game())
			{
				game.Run(30.0);
			}
		}

	
	}
}