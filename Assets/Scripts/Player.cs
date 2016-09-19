using UnityEngine;
using System.Collections;

public class Player : GameThing {
	KeyCode[] UP_KEYS = new KeyCode[]{KeyCode.W, KeyCode.UpArrow, KeyCode.Space};
	KeyCode[] LEFT_KEYS = new KeyCode[]{KeyCode.A, KeyCode.LeftArrow};
	KeyCode[] DOWN_KEYS = new KeyCode[]{KeyCode.S, KeyCode.DownArrow};
	KeyCode[] RIGHT_KEYS = new KeyCode[]{KeyCode.D, KeyCode.RightArrow};

	public const float movementTime = 0.1f; // seconds per unit
	public const float hoverTime = 0.2f; // seconds

	protected float lastMoved;
	protected float lastJumped;

	void Start () {
	}

	private bool KeyPressed(int direction) {
		KeyCode[] codes;
		switch (direction) {
		case UP:
			codes = UP_KEYS;
			break;
		case LEFT:
			codes = LEFT_KEYS;
			break;
		case DOWN:
			codes = DOWN_KEYS;
			break;
		case RIGHT:
			codes = RIGHT_KEYS;
			break;
		default:
			return false;
		}

		foreach (KeyCode code in codes) {
			if (Input.GetKey(code) || Input.GetKeyDown(code))
				return true;
		}
		return false;
	}

	protected override void DoSpecificMovement() {
		if (MovedRecently ()) {
			return;
		}
		if (!JumpedRecently() && TryToJump ()) {
			return;
		}
		if (TryToClimb ()) {
			return;
		}
		if (TryToWalk ()) {
			return;
		}
	}

	bool MovedRecently() {
		float delta = Time.timeSinceLevelLoad - lastMoved;
		return delta < movementTime;
	}

	bool JumpedRecently() {
		float delta = Time.timeSinceLevelLoad - lastJumped;
		return delta < hoverTime;
	}

	bool TryToJump() {
		if (!KeyPressed(UP) || KeyPressed(DOWN))
			return false;

		int x = GetX ();
		int y = GetY ();
		GameThing inTheWay = GameController.game.GetThingAt (x, y + 1);
		if (!inTheWay.AllowMovingThrough (this, UP)) {
			return false;
		}

		// Push if it's a rock
		inTheWay.MovingIn (this, UP);

		ActuallyMove (x, y + 1);
		lastJumped = Time.timeSinceLevelLoad;

		return true;
	}

	bool TryToClimb() {
		if (!KeyPressed (UP) && !KeyPressed (DOWN))
			return false;

		if (KeyPressed (UP) && KeyPressed (DOWN))
			return false;

		// TODO climbing
		return false;
	}

	bool TryToWalk() {
		if (!KeyPressed (RIGHT) && !KeyPressed (LEFT))
			return false;
		if (KeyPressed (RIGHT) && KeyPressed (LEFT))
			return false;
		
		int direction;
		int dx;
		if (KeyPressed (RIGHT)) {
			direction = RIGHT;
			dx = 1;
		} else {
			direction = LEFT;
			dx = -1;
		}
		FaceDirection (direction);

		int x = GetX ();
		int y = GetY ();
		GameThing inTheWay = GameController.game.GetThingAt (x + dx, y);
		if (!inTheWay.AllowMovingThrough (this, direction)) {
			return false;
		}

		// Push if it's a block
		inTheWay.MovingIn (this, direction);

		ActuallyMove (x + dx, y);
		lastMoved = Time.timeSinceLevelLoad;

		return true;
	}

	public override bool AllowMovingThrough(GameThing other, int intoDirection) {
		return false;
	}

	public override bool AllowFallingThrough (GameThing other) {
		return false;
	}

	public override bool HasGravity() {
		return !JumpedRecently ();
	}

	public void YouWon() {
		SpriteRenderer renderer = obj.GetComponent<SpriteRenderer> ();
		renderer.sprite = Resources.Load("Sprites/Win", typeof(Sprite)) as Sprite;
	}
}
