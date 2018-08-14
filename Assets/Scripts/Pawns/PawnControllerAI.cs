using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnControllerAI : MonoBehaviour {

	public TMPro.TMP_Text DebugText;

	private Pawn m_pawn;

	private AICore m_brain;

	private void Awake() {
		m_pawn = GetComponent<Pawn>();
		m_brain = new AICore(m_pawn);
	}

	private void FixedUpdate() {
		if (!GameController.IsPawnAllowedMove()) return;

		m_brain.Tick(Time.fixedDeltaTime);
	}

#if UNITY_EDITOR
	private void LateUpdate() {
		if (DebugText != null)
			DebugText.text += m_brain.GetDebugString() + "\r\n";
	}

	private void Update() {
		if (DebugText != null)
			DebugText.text = "";
	}
#endif

}
