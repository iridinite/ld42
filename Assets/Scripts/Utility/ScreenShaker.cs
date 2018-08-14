using UnityEngine;

public class ScreenShaker : MonoBehaviour {

	[Range(1f, 50f)] public float ShakeSpeed;

	private Transform m_transform;

	private static float m_shaketime;
	private static float m_shaketimemax;
	private static float m_shakeintensity;

	public static void Shake(float time, float intensity) {
		if (intensity < m_shakeintensity && time < m_shaketime) return;

		m_shakeintensity = intensity;
		m_shaketimemax = time;
		m_shaketime = time;
	}

	public static void Clear() {
		m_shakeintensity = 0;
		m_shaketimemax = 0;
		m_shaketime = 0;
	}

	private void Awake() {
		m_transform = GetComponent<Transform>();
	}

	private void Update() {
		if (m_shaketime <= 0f || !OptionsFile.ScreenShake) {
			m_transform.localPosition = Vector3.zero;
			m_transform.localRotation = Quaternion.identity;
			return;
		}

		m_shaketime -= Time.deltaTime;

		var power = (m_shaketime / m_shaketimemax) * m_shakeintensity;
		var height1 = Mathf.Sin(Time.time * ShakeSpeed) * power;
		var height2 = Mathf.Cos(Time.time * ShakeSpeed) * power * 2f;

		m_transform.localPosition = new Vector3(height1, height1, 0f);
		m_transform.localRotation = Quaternion.Euler(0f, 0f, height2);
	}

}
