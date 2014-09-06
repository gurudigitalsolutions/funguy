using System;

namespace FunGuy
{
	public class Texture
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FunGuy.Texture"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of the texture.
		/// </param>
		/// <param name='mapValue'>
		/// Map value.
		/// Used for determining player movement
		/// </param>
		/// <param name='texLibID'>
		/// Tex lib ID.
		/// ID given to texture when bitmap is loaded into the TexLib
		/// </param>
		public Texture(string name, int mapValue, int texLibID)
		{
			_Name = name;
			_MapValue = mapValue;
			_LibID = texLibID;
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name of the Texture.
		/// </value>
		public string Name
		{
			get
			{
				return _Name;
			}
		}
		private string _Name;

		/// <summary>
		/// Gets or sets the map value.
		/// </summary>
		/// <value>
		/// The map value.
		/// Negative values indicate character cannot move into this area
		/// </value>
		public int MapValue
		{
			get
			{
				return _MapValue;
			}
		}
		private int _MapValue;

		/// <summary>
		/// Gets or sets the texture library ID.
		/// </summary>
		/// <value>
		/// The texture library ID.
		/// int value assigned to bitmap when loading image into the TexLib.
		/// </value>
		public int LibID
		{
			get
			{
				return _LibID;
			}
		}
		private int _LibID;
	}
}

