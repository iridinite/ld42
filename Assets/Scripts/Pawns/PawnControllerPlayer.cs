using UnityEngine;

[RequireComponent(typeof(Pawn))]
public class PawnControllerPlayer : MonoBehaviour {

	private Pawn m_pawn;

	private void Awake() {
		m_pawn = GetComponent<Pawn>();
	}

	private void Update() {
		if (!GameController.IsPawnAllowedMove()) return;

		m_pawn.DesiredMove = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		if (Input.GetButtonDown("Jump"))
			m_pawn.Jump();
		if (Input.GetButtonDown("Fire1"))
			m_pawn.PlaceBomb();
	}

}
