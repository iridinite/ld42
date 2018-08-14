using UnityEngine;

public class MoveUpDown : MonoBehaviour {

	[Range(0f, 8f)] public float Speed;
	[Range(0f, 4f)] public float Magnitude;

	private Transform m_transform;

	private void Awake() {
		m_transform = GetComponent<Transform>();
	}

	private void Update() {
		m_transform.localPosition = new Vector3(0f, Mathf.Sin(Time.time * Speed) * Magnitude, 0f);
	}

}
