using UnityEngine;
using System.Collections;

public class Empty : GameThing {
	public static Empty NewEmpty() {
		GameObject obj = new GameObject(typeof(Empty).Name);
		Empty e = obj.AddComponent<Empty> ();
		e.obj = obj;

		return e;
	}
}
