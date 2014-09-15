using System;
using System.Collections.Generic;


namespace FunGuy
{
	public class MyCharacter
	{
		public MyCharacter(string name, int mapValue, int texLibID, int index, float height)
		{
			Name = name;
			Value = mapValue;
			TexLibID = texLibID;
			Index = index;
			Height = height;
		}
		public MyCharacter(MyTexture texture, float height)
		{
			Name = texture.Name;
			Value = texture.Value;
			TexLibID = texture.TexLibID;
			Index = texture.Index;
			Height = height;
		}

		public string Name;
		public int Value;
		public int TexLibID;
		public int Index;

		public float Height;

		public const int Down = 0;
		public const int Up = 1;
		public const int Left = 2;
		public const int Right = 3;
	}
}

