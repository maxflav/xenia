using UnityEngine;
using System.Collections;

public class Goal : GameThing {
	public override bool AllowMovingThrough(GameThing other, int intoDirection) {
		return other is Player;
	}

	public override bool AllowFallingThrough(GameThing other) {
		return other is Player;
	}

	public override void MovingIn(GameThing other, int intoDirection) {
		CheckWin (other);
	}

	public override void FallingIn(GameThing other) {
		CheckWin (other);
	}

	private void CheckWin(GameThing other) {
		if (other is Player) {
			GameController.game.WinLevel ();
			(other as Player).YouWon ();
		}
	}
}
