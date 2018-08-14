using UnityEngine;

public class Powerup : Thing {

	public GameObject PickupParticles;
	public AudioClip SoundPowerup;

	private PowerupType m_type;
	private bool m_activated;

	private void Start() {
		m_type = (PowerupType)Random.Range(0, (int)PowerupType.Health);
		m_activated = false;

		if (Random.Range(0, 24) == 0)
			m_type = PowerupType.Health;

		// change the material to use the correct part of the powerup textuer atlas
		var block = new MaterialPropertyBlock();
		block.SetFloat("_PageIndex", (int)m_type);
		GetComponentInChildren<Renderer>().SetPropertyBlock(block);

		// let AI know that we're here
		GameController.Powerups.Add(this);
	}

	private void OnTriggerEnter(Collider other) {
		var pawn = other.gameObject.GetComponent<Pawn>();
		if (pawn == null) return;

		if (m_activated) return;
		m_activated = true;

		switch (m_type) {
			case PowerupType.MaxBombs:
				pawn.BombMax++;
				break;
			case PowerupType.MaxRange:
				pawn.BombRange++;
				break;
			case PowerupType.MoveSpeed:
				pawn.MoveSpeed = Mathf.Min(pawn.MoveSpeed + 0.75f, 7.5f);
				break;
			case PowerupType.Health:
				pawn.Health++;
				break;
		}

		GameController.Powerups.Remove(this);
		AudioHelper.PlayOneshot3D(SoundPowerup, m_transform.position, 1f, Random.Range(0.8f, 1.2f));

		Instantiate(PickupParticles, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

}
