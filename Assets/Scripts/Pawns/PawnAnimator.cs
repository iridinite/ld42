using UnityEngine;

public class PawnAnimator : MonoBehaviour {

	private Animator m_animator;
	private Pawn m_pawn;

	private void Awake() {
		m_animator = GetComponentInChildren<Animator>();
		m_pawn = GetComponent<Pawn>();
	}

	private void Update() {
		if (m_pawn.IsDead() || !m_pawn.IsSpriteVisible()) return;

		if (GameController.IsGameOver() && GameController.GetFirstLivePawn() == m_pawn) {
			m_animator.SetTrigger("Ascend");
		}

		if (!GameController.IsPawnAllowedMove()) {
			m_animator.SetFloat("Speed", 0f);
			return;
		}

		var move = m_pawn.DesiredMove;
		if (move.sqrMagnitude > 0f) {
			m_animator.SetFloat("Move Horizontal", move.x);
			m_animator.SetFloat("Move Vertical", move.y);
		}
		m_animator.SetFloat("Speed", Mathf.Clamp01(move.sqrMagnitude));

		//m_animator.ResetTrigger("DropBomb");
	}

}
