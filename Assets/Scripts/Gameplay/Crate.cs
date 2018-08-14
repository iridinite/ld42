using UnityEngine;

public class Crate : Thing {

	private const float DROP_PCT = 0.33f;

	public GameObject PrefabParticles;
	public GameObject PrefabPowerup;

	public void Break() {
		if (Random.value < DROP_PCT)
			Instantiate(PrefabPowerup, GameController.MapToWorld(GetMapPos()) + Vector3.up, Quaternion.identity);
		Instantiate(PrefabParticles, GameController.MapToWorld(GetMapPos()) + Vector3.up, Quaternion.identity);

		Map.SetAt(GetMapX(), GetMapY(), MapTile.Floor);
		Destroy(gameObject);
	}

}
