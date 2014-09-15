using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace FunGuy
{
	[Serializable()]
	public class DefaultHouse : Thing
	{
		public int FrontWall = 0;
		public int RearWall = 0;
		public int Floor = 0;
		public int Roof = 0;
		public int LeftWall = 0;
		public int RightWall = 0;

		public DefaultHouse()
		{

		}

		public DefaultHouse(SerializationInfo info, StreamingContext ctxt)
		{
			FrontWall = (int)info.GetValue("FrontWall", typeof(int));
			RearWall = (int)info.GetValue("RearWall", typeof(int));
			Floor = (int)info.GetValue("Floor", typeof(int));
			Roof = (int)info.GetValue("Roof", typeof(int));
			LeftWall = (int)info.GetValue("LeftWall", typeof(int));
			RightWall = (int)info.GetValue("RightWall", typeof(int));


			// from thing
			X = (int)info.GetValue("X", typeof(int));
			Y = (int)info.GetValue("Y", typeof(int));
			Width = (int)info.GetValue("Width", typeof(int));
			Depth = (int)info.GetValue("Depth", typeof(int));
			Height = (int)info.GetValue("Height", typeof(int));
			IsSolid = (bool)info.GetValue("IsSolid", typeof(bool));
		}

		public DefaultHouse(int frontWall, int rearWall, int leftWall, int rightWall, int roof, int floor)
		{
			FrontWall = frontWall;
			RearWall = rearWall;
			LeftWall = leftWall;
			RightWall = rightWall;
			Roof = roof;
			Floor = floor;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("FrontWall", FrontWall);
			info.AddValue("RearWall", RearWall);
			info.AddValue("Floor", Floor);
			info.AddValue("Roof", Roof);
			info.AddValue("LeftWall", LeftWall);
			info.AddValue("RightWall", RightWall);
			info.AddValue("X", X);
			info.AddValue("Y", Y);
			info.AddValue("Width", Width);
			info.AddValue("Depth", Depth);
			info.AddValue("Height", Height);
			info.AddValue("IsSolid", IsSolid);

		}

		public override int[] TextureList()
		{
			int[] texList = new int[6];
			texList [0] = FrontWall;
			texList [1] = RearWall;
			texList [2] = LeftWall;
			texList [3] = RightWall;
			texList [4] = Roof;
			texList [5] = Floor;

			return texList;
		}

		public override void SetTextures(int[] texList)
		{
			FrontWall = texList [0];
			RearWall = texList [1];
			LeftWall = texList [2];
			RightWall = texList [3];
			Roof = texList [4];
			Floor = texList [5];
		}

		public override void Render()
		{
			// Flea-zord
			GL.BindTexture(TextureTarget.Texture2D, Floor);
			GL.Begin(BeginMode.Quads);

			GL.Normal3(-1.0f, 0.0f, 0.0f);


			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3(X, Y, 4.01f);

			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3(X, Y + Depth, 4.01f);

			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3(X + Width, Y + Depth, 4.01f);

			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3(X + Width, Y, 4.01f);
			GL.End();


			//	Front wall
			GL.BindTexture(TextureTarget.Texture2D, FrontWall);
			GL.Begin(BeginMode.Quads);
			
			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3(X, Y, 4.0f + Height);

			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3(X + Width, Y, 4.0f + Height);

			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3(X + Width, Y, 4.0f);

			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3(X, Y, 4.0f);

			GL.End();

			// left wall
			GL.BindTexture(TextureTarget.Texture2D, LeftWall);
			GL.Begin(BeginMode.Quads);
			
			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3(X, Y, 4.0f);

			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3(X, Y + Depth, 4.0f);

			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3(X, Y + Depth, 4.0f + Height);

			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3(X, Y, 4.0f + Height);

			GL.End();

			// right wall
			GL.BindTexture(TextureTarget.Texture2D, RightWall);
			GL.Begin(BeginMode.Quads);
			
			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3(X + Width, Y, 4.0f);

			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3(X + Width, Y + Depth, 4.0f);

			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3(X + Width, Y + Depth, 4.0f + Height);

			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3(X + Width, Y, 4.0f + Height);

			GL.End();

			//	Back wall
			GL.BindTexture(TextureTarget.Texture2D, RearWall);
			GL.Begin(BeginMode.Quads);
			
			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3(X, Y + Depth, 4.0f);

			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3(X, Y + Depth, 4.0f + Height);

			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3(X + Width, Y + Depth, 4.0f + Height);

			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3(X + Width, Y + Depth, 4.0f);

			GL.End();

			// Roof
			GL.BindTexture(TextureTarget.Texture2D, Roof);
			GL.Begin(BeginMode.Quads);

			GL.Normal3(-1.0f, 0.0f, 0.0f);


			GL.TexCoord2(0.0f, 1.0f);
			GL.Vertex3(X, Y, 4f + Height);

			GL.TexCoord2(1.0f, 1.0f);
			GL.Vertex3(X, Y + Depth, 4f + Height);

			GL.TexCoord2(1.0f, 0.0f);
			GL.Vertex3(X + Width, Y + Depth, 4f + Height);

			GL.TexCoord2(0.0f, 0.0f);
			GL.Vertex3(X + Width, Y, 4f + Height);
			GL.End();

		}

		public override void Update()
		{

		}
	}
}

