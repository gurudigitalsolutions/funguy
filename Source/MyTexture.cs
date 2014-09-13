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

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name of the Texture.
		/// </value>
		public string Name;

		/// <summary>
		/// Gets or sets the value to display on the map.
		/// </summary>
		/// <value>
		/// The map value.
		/// Negative values indicate character cannot move into this area
		/// </value>
		public int Value;

		/// <summary>
		/// Gets or sets the texture library ID.
		/// </summary>
		/// <value>
		/// The texture library ID.
		/// int value assigned to bitmap when loading image into the TexLib.
		/// </value>
		public int TexLibID;

		/// <summary>
		/// Gets the ID used for tile edit mode.
		/// </summary>
		/// <value>
		/// The 1 based index for edit mode.
		/// </value>
		public int Index;
	}
}

