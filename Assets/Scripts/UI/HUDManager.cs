using UnityEngine;

public class HUDManager : MonoBehaviour {

	public GameObject PrefabPlayerPanel;

	private void Start() {
		// spawn UI widgets for all live pawns
		for (int i = 0; i < GameController.GetNumPawns(); i++) {
			var gobj = Instantiate(PrefabPlayerPanel, transform);
			gobj.GetComponent<PlayerPanel>().MyPawn = GameController.GetPawn(i);
		}
	}

}
