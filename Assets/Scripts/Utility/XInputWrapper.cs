using UnityEngine;
#if UNITY_STANDALONE_WIN
using XInputDotNetPure;
#endif

public class XInputWrapper : MonoBehaviour {

	private float m_RumbleTime;
	private float m_RumbleStrength;

	private static XInputWrapper m_Inst;

	public static XInputWrapper Instance {
		get {
			if (m_Inst == null)
				m_Inst = new GameObject("XInputWrapper").AddComponent<XInputWrapper>();
			return m_Inst;
		}
	}

	public void AddRumble(float time, float intensity) {
		m_RumbleTime = Mathf.Max(time, m_RumbleTime);
		m_RumbleStrength = Mathf.Max(intensity, m_RumbleStrength);
	}

	public bool IsControllerUser() {
#if UNITY_STANDALONE_WIN
		return GamePad.GetState(PlayerIndex.One).IsConnected;
#else
		return false;
#endif
	}

	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}

#if UNITY_STANDALONE_WIN
	private void Update() {
		// controller rumble
		m_RumbleTime -= Time.unscaledDeltaTime;
		if (m_RumbleTime < 0f)
			m_RumbleStrength = 0f;

		if (GamePad.GetState(PlayerIndex.One).IsConnected) {
			GamePad.SetVibration(PlayerIndex.One, m_RumbleStrength, m_RumbleStrength);
		}
	}
#endif

}
