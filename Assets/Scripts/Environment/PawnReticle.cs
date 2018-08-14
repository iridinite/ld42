using UnityEngine;

public class PawnReticle : MonoBehaviour {

	public Pawn MyPawn { get; set; }
	public float Speed;

	private Transform m_transform;
	private float m_total;

	private void Start() {
		m_transform = GetComponent<Transform>();

		// this is also used for the powerups
		if (MyPawn == null) return;

		var block = new MaterialPropertyBlock();
		block.SetColor("_TintColor", GameController.PawnColors[MyPawn.GetPawnID()] * 2f);
		GetComponent<Renderer>().SetPropertyBlock(block);
	}

	private void Update() {
		m_total += Speed * Time.deltaTime;
		m_transform.localEulerAngles = new Vector3(0f, 0f, m_total);
	}

}
