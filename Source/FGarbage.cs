using System;

namespace FunGuy
{
	public static class FGarbage
	{
		public static void ConvertMaps()
		{
			for (int x = 0; x < 5; x++)
			{
				for (int y = 0; y < 5; y++)
				{
					Map thismap = Map.Loader(string.Format("{0}/Maps/{1}_{2}.map", "/share/code/c#/FunGuy/Resources", x, y));
					thismap.Save();
				}
			}
		}
	}
}

