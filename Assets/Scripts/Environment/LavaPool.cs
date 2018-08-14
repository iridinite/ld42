using UnityEngine;

public class LavaPool : MonoBehaviour {

	//private const int LAKE_SIZE = 15;
	private const float TILE_SIZE = 3f;

	public GameObject PrefabLavaTile;
	[Range(1, 100)] public int LakeSize;

	private Transform m_transform;

	private void Awake() {
		m_transform = GetComponent<Transform>();
	}

	private void Start() {
		for (int x = -LakeSize; x <= LakeSize; x++) {
			for (int y = -LakeSize; y <= LakeSize; y++) {
				var gobj = Instantiate(PrefabLavaTile, new Vector3(x * TILE_SIZE, 0f, y * TILE_SIZE), Quaternion.identity);
				gobj.transform.SetParent(m_transform, false);
			}
		}
	}

	private void Update() {
		// panning
		var offset = -Mathf.Repeat(Time.time * 0.3f, TILE_SIZE);
		var sin = Mathf.Sin(Time.time * 0.3f) * 0.5f;
		m_transform.localPosition = new Vector3(offset, sin * 0.5f, sin);
	}

}
