using UnityEngine;

public class GameRoot : MonoBehaviour {

	public static int NumPawns { get; set; } = 2;

	public GameObject PrefabPawnPlayer;
	public GameObject PrefabPawnAI;
	public GameObject PrefabBomb;

	private void Awake() {
		GameController.Initialize(this, NumPawns);
	}

	public Pawn CreatePawn(int index, bool player) {
		var gobj = Instantiate(player ? PrefabPawnPlayer : PrefabPawnAI);
		gobj.transform.localPosition = GameController.MapToWorld(Map.GetSpawnPoint(index)) + Vector3.up;

		return gobj.GetComponent<Pawn>();
	}

	public Bomb CreateBomb(int x, int y) {
		var gobj = Instantiate(PrefabBomb);
		gobj.GetComponent<Thing>().SetMapPos(x, y);
		gobj.transform.position += Vector3.up;

		return gobj.GetComponent<Bomb>();
	}

}
