using UnityEngine;
using System.Collections;

public class Wall : GameThing {
	public override bool AllowMovingThrough(GameThing other, int fromDirection) {
		return false;
	}
	public override bool AllowFallingThrough(GameThing other) {
		return false;
	}
}
