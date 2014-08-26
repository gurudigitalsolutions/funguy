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
		public FunGuy.Map TheMap;
		public FunGuy.Player ThePlayer;
		public int TimeStamp;

		public static int GameModeGame = 0;
		public static int GameModeEditor = 1;

		public int GameMode = GameModeGame;

		/// <summary>Creates a 800x600 window with the specified title.</summary>
		public Game ()
            : base(800, 600, GraphicsMode.Default, "OpenTK Quick Start Sample")
		{
			VSync = VSyncMode.On;
		}

		/// <summary>Load resources here.</summary>
		/// <param name="e">Not used.</param>
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);

			GL.ClearColor (0.1f, 0.2f, 0.5f, 0.0f);
			GL.Enable (EnableCap.DepthTest);

			TexLib.InitTexturing ();

			TimeStamp = Environment.TickCount;

			ThePlayer = new Player ();
			ThePlayer.WorldX = 32;
			ThePlayer.WorldY = 32;

			TheMap = new Map ("Default", "default", 64, 64);

			for (int x = 0; x < TheMap.Width; x++) {
				if (x > 20 && 40 > x) {
					TheMap.Coordinates [x, 28] = -1;
				} else if (x > 0 && 10 > x) {
					TheMap.Coordinates [x, 1] = -2;
				} else if (x > 65 && 64 > x) {
					TheMap.Coordinates [x, 62] = -2;
				} else {
					TheMap.Coordinates [x, 28] = 0;
				}
			}
		}

		/// <summary>
		/// Called when your window is resized. Set your viewport here. It is also
		/// a good place to set up your projection matrix (which probably changes
		/// along when the aspect ratio of your window).
		/// </summary>
		/// <param name="e">Not used.</param>
		protected override void OnResize (EventArgs e)
		{
			base.OnResize (e);

			GL.Viewport (ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView ((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
			GL.MatrixMode (MatrixMode.Projection);
			GL.LoadMatrix (ref projection);
		}

		/// <summary>
		/// Called when it is time to setup the next frame. Add you game logic here.
		/// </summary>
		/// <param name="e">Contains timing information for framerate independent logic.</param>
		protected override void OnUpdateFrame (FrameEventArgs e)
		{
			base.OnUpdateFrame (e);

			if (Keyboard [Key.Escape])
				Exit ();

			if (Keyboard [Key.Space]
				&& ThePlayer.CanMove ()) {

				if (GameMode == GameModeGame) {
					GameMode = GameModeEditor;
				} else {
					if (TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY] > -1) {
						GameMode = GameModeGame;
					}
				}
			}

			if (Keyboard [Key.Up]) {
				if (ThePlayer.WorldY + 1 < TheMap.Height
					&& ThePlayer.CanMove ()
					&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY + 1] > -1)) {
					ThePlayer.WorldY++;
				}
			}

			if (Keyboard [Key.Down]) {
				if (ThePlayer.WorldY > 0
					&& ThePlayer.CanMove ()
					&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY - 1] > -1)) {
					ThePlayer.WorldY--;
				}
			}

			if (Keyboard [Key.Left]) {
				if (ThePlayer.WorldX > 0
					&& ThePlayer.CanMove ()
					&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.WorldX - 1, ThePlayer.WorldY] > -1)) {
					ThePlayer.WorldX--;
				}
			}

			if (Keyboard [Key.Right]) {
				if (ThePlayer.WorldX + 1 < TheMap.Width
					&& ThePlayer.CanMove ()
					&& (GameMode == GameModeEditor || TheMap.Coordinates [ThePlayer.WorldX + 1, ThePlayer.WorldY] > -1)) {
					ThePlayer.WorldX++;
				}
			}

			if (GameMode == GameModeEditor
				&& Keyboard [Key.Tab] && !Keyboard [Key.ShiftLeft]
				&& ThePlayer.CanMove ()) {


				int firstnum = -31415;
				bool snagnext = false;
				bool gotit = false;

				foreach (System.Collections.Generic.KeyValuePair<int, int> kvp in TheMap.TextureSetIDs) {
					if (firstnum == -31415) {
						firstnum = kvp.Key;
					}
					if (snagnext) {
						TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY] = kvp.Key;
						gotit = true;
						snagnext = false;
					} else if (kvp.Key == TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY]) {
						snagnext = true;
					}
				}

				if (!gotit) {
					TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY] = firstnum;
				}
			}

			if (GameMode == GameModeEditor
				&& Keyboard [Key.Tab] && (Keyboard [Key.ShiftLeft] || Keyboard [Key.LShift])
				&& ThePlayer.CanMove ()) {


				int lastnum = -31415;
				int prevnum = 0;
				bool snagnext = false;
				bool gotit = false;
				foreach (System.Collections.Generic.KeyValuePair<int, int> kvp in TheMap.TextureSetIDs) {

					lastnum = kvp.Key;
					if (snagnext) {
						TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY] = prevnum;
						gotit = true;
						snagnext = false;
					} else if (kvp.Key == TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY]) {
						//prevnum = kvp.Key;
						snagnext = true;
					}
					prevnum = lastnum;

				}

				if (!gotit) {
					TheMap.Coordinates [ThePlayer.WorldX, ThePlayer.WorldY] = lastnum;
				}
			}
		}

		/// <summary>
		/// Called when it is time to render the next frame. Add your rendering code here.
		/// </summary>
		/// <param name="e">Contains timing information.</param>
		protected override void OnRenderFrame (FrameEventArgs e)
		{
			base.OnRenderFrame (e);

			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//Matrix4 modelview = Matrix4.LookAt (Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
			Matrix4 modelview = Matrix4.LookAt (new Vector3 ((float)ThePlayer.WorldX + 0.5f, (float)ThePlayer.WorldY - 5.5f, (float)16),  // Camera
			                                   new Vector3 ((float)ThePlayer.WorldX + 0.5f, (float)ThePlayer.WorldY + 0.5f, (float)4),	// Look At
			                                   Vector3.UnitY);

			GL.MatrixMode (MatrixMode.Modelview);
			GL.LoadMatrix (ref modelview);

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


			for (int x = 0; x < TheMap.Width; x++) {
				for (int y = 0; y < TheMap.Height; y++) {
					GL.BindTexture (TextureTarget.Texture2D, TheMap.TextureSetIDs [TheMap.Coordinates [x, y]]);

					GL.Begin (BeginMode.Quads);
					GL.Normal3 (-1.0f, 0.0f, 0.0f);


					GL.TexCoord2 (0.0f, 1.0f);
					GL.Vertex3 ((float)x, (float)y, 4.0f);
					GL.TexCoord2 (1.0f, 1.0f);
					GL.Vertex3 ((float)x + 1, (float)y, 4.0f);
					GL.TexCoord2 (1.0f, 0.0f);
					GL.Vertex3 ((float)x + 1, (float)y + 1, 4.0f);
					GL.TexCoord2 (0.0f, 0.0f);
					GL.Vertex3 ((float)x, (float)y + 1, 4.0f);

					GL.End ();
				}

			}


			//
			//	Draw the character
			//

			if (GameMode == GameModeGame) {
				GL.BindTexture (TextureTarget.Texture2D, ThePlayer.TextureSetIDs [1]);
				GL.Begin (BeginMode.Quads);
				GL.Normal3 (-1.0f, 0.0f, 0.0f);


				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX, (float)ThePlayer.WorldY, 4.05f);
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX + 1, (float)ThePlayer.WorldY - 0.1f, 4.05f);
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX + 1, (float)ThePlayer.WorldY + 0.9f, 5.05f);
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX, (float)ThePlayer.WorldY + 1, 5.05f);

			} else {
				GL.BindTexture (TextureTarget.Texture2D, ThePlayer.TextureSetIDs [0]);
				GL.Begin (BeginMode.Quads);
				GL.Normal3 (-1.0f, 0.0f, 0.0f);


				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX, (float)ThePlayer.WorldY, 4.05f);
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX + 1, (float)ThePlayer.WorldY, 4.05f);
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX + 1, (float)ThePlayer.WorldY + 1f, 4.05f);
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 ((float)ThePlayer.WorldX, (float)ThePlayer.WorldY + 1, 4.05f);
			}


			GL.End ();

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



			SwapBuffers ();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			// The 'using' idiom guarantees proper resource cleanup.
			// We request 30 UpdateFrame events per second, and unlimited
			// RenderFrame events (as fast as the computer can handle).
			using (Game game = new Game()) {
				game.Run (30.0);
			}
		}

	
	}
}