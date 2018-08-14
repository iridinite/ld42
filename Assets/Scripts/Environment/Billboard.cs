using UnityEngine;

public class Billboard : MonoBehaviour {

	private Transform m_transform;
	private Transform m_camTransform;

	private void Awake() {
		m_transform = GetComponent<Transform>();
		m_camTransform = Camera.main.GetComponent<Transform>();
	}

	private void Update() {
		m_transform.LookAt(
			m_transform.position + m_camTransform.rotation * Vector3.forward,
			m_camTransform.rotation * Vector3.up);
	}

}
