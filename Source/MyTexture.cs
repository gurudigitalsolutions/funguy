using System;

namespace FunGuy
{
	public class MyTexture
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunGuy.Texture"/> class.
		/// </summary>
		/// <param name='textureName'>
		/// Texture name.
		/// </param>
		/// <param name='movementValue'>
		/// Map value.
		/// </param>
		/// <param name='texLibID'>
		/// Tex lib ID.
		/// </param>
		/// <param name='editID'>
		/// Edit ID.
		/// </param>
		public MyTexture(string texName, int texValue, int texLibID, int index)
		{
			Name = texName;
			Value = texValue;
			TexLibID = texLibID;
			Index = index;
			//Console.WriteLine("Name: {0} Value: {1} TexLibID: {2} Index: {3}", texName, texValue, texLibID, index);
		}
		public MyTexture(string texName, int texValue, int texLibID)
		{
			Name = texName;
			Value = texValue;
			TexLibID = texLibID;
			Index = 0;
			//Console.WriteLine("Name: {0} Value: {1} TexLibID: {2}", texName, texValue, texLibID);
		}
		public MyTexture()
		{

		}


		public string Name;

		public int Value;

		public int TexLibID;

		public int Index;
	}
}

