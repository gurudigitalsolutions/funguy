using System;

namespace FunGuy
{
	public class Player
	{
		public Player ()
		{
		}

		public int WorldX { get; set; }
		public int WorldY { get; set; }
		public int LastMovedTime{ get; set; }
		public int MoveInterval = 250;

		public bool CanMove ()
		{
			if (Environment.TickCount - LastMovedTime > MoveInterval) {
				LastMovedTime = Environment.TickCount;
				return true;
			}

			return false;
		}
	}
}

