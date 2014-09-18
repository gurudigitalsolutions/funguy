using System;

namespace FunGuy
{
    public static class FGarbage
    {
        public static void ConvertMaps(int[,] aaa)
		{
            for (int x = 0; x < aaa.GetLength(0); x++)
            {
                for (int y = 0; y < aaa.GetLength(0); y++)
                {
                    Console.Write("{0}-", aaa [x, y]);
                }
                Console.WriteLine();
            }			
		}
    }
}

