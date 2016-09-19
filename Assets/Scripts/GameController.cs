using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
	public GameObject Background;

	private static Empty EmptySingleton;
	public static GameController game;

	private List<List<GameThing>> everything = new List<List<GameThing>>();
	private int maxX = 0;
	private int maxY = 0;
	private int currLevel = 0;
	private TextAsset[] levels;

	public bool Paused = false;

	void Start () {
		game = this;
		EmptySingleton = Empty.NewEmpty ();

		Object[] levelObjects = Resources.LoadAll("Levels", typeof(TextAsset));
		levels = new TextAsset[levelObjects.Length];
		for (int i = 0; i < levelObjects.Length; i++) {
			levels [i] = levelObjects [i] as TextAsset;
		}

		StartLevel ();
	}

	private void UpdateCamera() {
		Vector3 pos = transform.position;
		pos.x = maxX / 2;
		pos.y = maxY / 2;
		transform.position = pos;

		pos = Background.transform.position;
		pos.x = maxX / 2;
		pos.y = maxY / 2;
		Background.transform.position = pos;
	}

	public int NormalizeX(int x) {
		x = x % (maxX + 1);
		while (x < 0)
			x += maxX + 1;
		return x;
	}

	public int NormalizeY(int y) {
		y = y % (maxY + 1);
		while (y < 0)
			y += maxY + 1;
		return y;
	}

	public void AddToEverything(GameThing thing, int x, int y) {
		while (everything.Count <= x) {
			everything.Add (new List<GameThing> ());
		}

		List<GameThing> column = everything [x];
		while (column.Count <= y) {
			column.Add (null);
		}

		if (column [y] != null) {
			Object.Destroy (column [y]);
		}
		column [y] = thing;
	}

	public GameThing GetThingAt(int x, int y) {
		x = NormalizeX (x);
		y = NormalizeY (y);

		if (y < 0 || x < 0 || everything.Count <= x)
			return EmptySingleton;
		List<GameThing> column = everything [x];
		if (column.Count <= y)
			return EmptySingleton;
		GameThing thing = column [y];
		if (thing == null)
			return EmptySingleton;
		return thing;
	}

	public void RemoveThingFrom(int x, int y) {
		x = NormalizeX (x);
		y = NormalizeY (y);

		if (everything [x].Count <= y)
			return;
		everything [x] [y] = null;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ResetLevel ();
		}
	}

	void LoadLevel(int number) {
		Clear ();

		TextAsset level = levels [number];
		string text = level.text;
		string[] lines = text.Split ('\n');

		maxY = lines.Length - 1;
		maxX = 0;
		for (int y = 0; y < lines.Length; y++) {
			string line = lines [lines.Length - y - 1];
			line = line.TrimEnd ('\n', '\r');

			if (line.Length - 1 > maxX)
				maxX = line.Length - 1;

			for (int x = 0; x < line.Length; x++) {
				switch (line [x]) {
					case 'p':
						GameThing.New<Player> (x, y);
						break;
					case 'x':
						GameThing.New<Wall> (x, y);
						break;
					case 'o':
						GameThing.New<Block> (x, y);
						break;
					case '*':
						GameThing.New<Goal> (x, y);
						break;
				}
			}
		}

		UpdateCamera ();
	}

	void Clear() {
		foreach (List<GameThing> column in everything) {
			foreach (GameThing thing in column) {
				if (thing != null)
					Object.Destroy (thing);
			}
		}

		everything =  new List<List<GameThing>>();
		maxX = 0;
		maxY = 0;
	}

	public void WinLevel() {
		Paused = true;
		currLevel++;
		currLevel %= levels.Length;
		Invoke ("StartLevel", .5f);
	}

	private void StartLevel() {
		LoadLevel (currLevel);
		Paused = false;
	}

	private void ResetLevel () {
		Paused = true;
		Invoke ("StartLevel", .1f);
	}
}
