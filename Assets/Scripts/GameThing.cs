using UnityEngine;
using System.Collections;

public class GameThing : MonoBehaviour {
	public const float fallTime = 0.05f; // seconds per unit

	public GameObject obj;

	public const int UP    = 0;
	public const int DOWN  = 1;
	public const int RIGHT = 2;
	public const int LEFT  = 3;

	protected float lastFell;

	public static T New<T>(int x, int y) where T : GameThing {
		GameObject obj = new GameObject(typeof(T).Name);
		SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
		renderer.sprite = Resources.Load("Sprites/" + typeof(T), typeof(Sprite)) as Sprite;
		T thing = obj.AddComponent<T>();
		thing.obj = obj;

		GameController.game.AddToEverything (thing, x, y);
		obj.transform.position = new Vector2 (x, y);

		return thing;
	}

	public int GetX() {
		return (int)obj.transform.position.x;
	}

	public int GetY() {
		return (int)obj.transform.position.y;
	}

	void Update () {
		UpdateAnimation ();

		if (GameController.game.Paused) {
			return;
		}

		if (!DoGeneralMovement ()) {
			DoSpecificMovement ();
		}
	}

	protected virtual void UpdateAnimation() {
	}

	// Returns true if it should prevent further movement.
	protected virtual bool DoGeneralMovement () {
		if (FellRecently ()) {
			return true;
		}
		if (TryFalling ()) {
			return true;
		}
		return false;
	}

	protected virtual void DoSpecificMovement() {
		
	}

	// True if successful.
	bool TryFalling() {
		if (!HasGravity ()) {
			return false;
		}

		int x = GetX ();
		int y = GetY ();
		GameThing thingFromBelow = GameController.game.GetThingAt (x, y - 1);
		if (!thingFromBelow.AllowFallingThrough (this)) {
			return false;
		}

		thingFromBelow.FallingIn(this);
		ActuallyMove (x, y - 1);
		lastFell = Time.timeSinceLevelLoad;

		return true;
	}

	protected bool FellRecently() {
		float delta = Time.timeSinceLevelLoad - lastFell;
		return delta < fallTime;
	}

	public virtual bool AllowMovingThrough(GameThing other, int intoDirection) {
		return true;
	}

	// aka get out the way
	public virtual void MovingIn(GameThing other, int intoDirection) {
	}

	// Check valid movement before calling this. Use for walking, falling, etc
	protected void ActuallyMove(int newX, int newY) {
		newX = GameController.game.NormalizeX (newX);
		newY = GameController.game.NormalizeY (newY);

		int oldX = GetX ();
		int oldY = GetY ();

		GameController.game.RemoveThingFrom (oldX, oldY);
		GameController.game.AddToEverything (this, newX, newY);
		obj.transform.position = new Vector2 (newX, newY);
	}

	public virtual bool AllowFallingThrough(GameThing other) {
		return true;
	}

	public virtual void FallingIn(GameThing other) {
	}

	public virtual bool HasGravity() {
		return false;
	}

	public void FaceDirection(int direction) {
		Vector3 theScale = obj.transform.localScale;
		if (direction == RIGHT)
			theScale.x = Mathf.Abs(theScale.x);
		if (direction == LEFT)
			theScale.x = -1 * Mathf.Abs(theScale.x);
		obj.transform.localScale = theScale;
	}

	public static int DirectionDX(int direction) {
		if (direction == RIGHT)
			return 1;
		if (direction == LEFT)
			return -1;
		return 0;
	}

	public static int DirectionDY(int direction) {
		if (direction == UP)
			return 1;
		if (direction == DOWN)
			return -1;
		return 0;
	}

	void OnDestroy() {
		Object.Destroy (obj);
	}
}
