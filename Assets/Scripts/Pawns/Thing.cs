using UnityEngine;

public abstract class Thing : MonoBehaviour {

	protected Transform m_transform;
	protected MapPoint m_mapPos;

	protected virtual void Awake() {
		m_transform = GetComponent<Transform>();
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(GameController.MapToWorld(GetMapX(), GetMapY()) + Vector3.up, 0.5f);
	}
#endif

	protected virtual void FixedUpdate() {
		// update map coordinates
		GameController.WorldToMap(m_transform.position, out m_mapPos.x, out m_mapPos.y);
	}

	public Transform GetTransform() {
		return m_transform;
	}

	public MapPoint GetMapPos() {
		return m_mapPos;
	}

	public int GetMapX() {
		return m_mapPos.x;
	}

	public int GetMapY() {
		return m_mapPos.y;
	}

	public void SetMapPos(MapPoint pos) {
		SetMapPos(pos.x, pos.y);
	}

	public void SetMapPos(int x, int y) {
		m_mapPos.x = x;
		m_mapPos.y = y;

		// snap to map-to-world pos, but maintain current y
		var worldPos = GameController.MapToWorld(x, y) + Vector3.up;
		m_transform.position = new Vector3(worldPos.x, m_transform.position.y, worldPos.z);
	}

}
