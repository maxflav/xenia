using UnityEngine;
using System.Collections;

public class Block : GameThing {
	public override bool AllowMovingThrough(GameThing other, int intoDirection) {
		int x = GetX ();
		int y = GetY ();

		int dx = DirectionDX(intoDirection);
		int dy = DirectionDY(intoDirection);

		GameThing inTheWay = GameController.game.GetThingAt (x + dx, y + dy);
		return inTheWay.AllowMovingThrough (this, intoDirection);
	}

	// aka get out the way
	public override void MovingIn(GameThing other, int intoDirection) {
		int x = GetX ();
		int y = GetY ();

		int dx = DirectionDX(intoDirection);
		int dy = DirectionDY(intoDirection);

		GameThing inTheWay = GameController.game.GetThingAt (x + dx, y + dy);
		inTheWay.MovingIn (this, intoDirection);

		ActuallyMove (x + dx, y + dy);
	}

	public override bool AllowFallingThrough(GameThing other) {
		return false;
	}

	public override bool HasGravity() {
		return true;
	}
}

