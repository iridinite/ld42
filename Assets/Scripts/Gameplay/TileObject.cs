using UnityEngine;

public class TileObject : MonoBehaviour {

	public GameObject[] TilePrefabs;
	public GameObject BreakParticles;

	private Transform m_transform;
	private MapTile m_tile;

	private void Start() {
		m_transform = GetComponent<Transform>();

		if (TilePrefabs[(int)m_tile] != null) {
			var child = Instantiate(TilePrefabs[(int)m_tile], m_transform);
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
		}
	}

	public void SetTile(MapTile tile) {
		m_tile = tile;
	}

	public void Break() {
		Instantiate(BreakParticles, m_transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

}
