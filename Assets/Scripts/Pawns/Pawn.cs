using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pawn : Thing {

	private const float BOMB_COOLDOWN = 0.1f;
	private const float DAMAGE_COOLDOWN = 1.25f;
	private const float JUMP_FORCE = 11f;

	[Header("Assets")]
	public Sprite[] DoritosTemplates;
	public GameObject PrefabReticle;

	public AudioClip SoundJump, SoundHit, SoundKill, SoundPlaceBomb;

	[Header("Local References")]
	public GameObject SpriteObject;
	public SpriteRenderer DoritosObject;

	public Vector2 DesiredMove { get; set; }

	public event Action<Pawn> Died;

	public int BombMax { get; set; } = 1;
	public int BombRange { get; set; } = 1;
	public int Health { get; set; } = 3;
	public float MoveSpeed { get; set; } = 3.75f;

	private Rigidbody m_rigidbody;
	private Animator m_animator;
	private GameObject m_reticle;
	private Transform m_reticleTr;

	private float m_bombCooldown;
	private float m_invulnerability;
	private int m_bombsOwned;
	private int m_pawnID;
	private bool m_canJump;
	private bool m_isJumping;

	protected override void Awake() {
		base.Awake();

		m_rigidbody = GetComponent<Rigidbody>();
		m_animator = GetComponentInChildren<Animator>();

		// spawn the floor reticle and assign myself to it, so it picks the right color
		m_reticle = Instantiate(PrefabReticle, Vector3.down * 0.74f, Quaternion.identity);
		m_reticleTr = m_reticle.GetComponent<Transform>();
		foreach (var reticle in m_reticle.GetComponentsInChildren<PawnReticle>())
			reticle.MyPawn = this;

		// to decrement owned bomb count
		GameController.BombDetonated += this.GameController_BombDetonated;
	}

	private void Start() {
		DoritosObject.sprite = DoritosTemplates[m_pawnID];
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();

		if (IsDead()) return;
		if (!GameController.IsPawnAllowedMove()) return;

		// pawn fell out of the map?
		if (m_transform.position.y < -5f) {
			Kill();
			return;
		}

		// make sure the reticle is on the ground
		var reticlePos = m_transform.position;
		reticlePos.y = 0.51f;
		m_reticleTr.position = reticlePos;

		// expire timers
		if (m_bombCooldown > 0f)
			m_bombCooldown -= Time.fixedDeltaTime;
		if (m_invulnerability > 0f)
			m_invulnerability -= Time.fixedDeltaTime;

		// jump allowed if touching ground and not moving up
		m_canJump = m_rigidbody.velocity.y <= 0.001f &&
			Physics.Raycast(m_transform.position, Vector3.down, 1f);
		if (m_canJump)
			m_isJumping = false;

		// obtain desired movement vector
		var move3D = new Vector3(DesiredMove.x, 0f, DesiredMove.y);
		if (move3D.sqrMagnitude <= 0.25f) return;
		var moveDist = MoveSpeed * Time.fixedDeltaTime;
		// if movement runs into a wall, transform the vector so the illegal part is removed
		RaycastHit hit;
		if (Physics.SphereCast(m_transform.position, 0.4f, move3D, out hit, moveDist, ~LayerMask.GetMask("Player")))
			move3D = TransformDesired(move3D, hit.normal);
		// perform movement
		m_transform.Translate(move3D * moveDist);
	}

	private static Vector3 TransformDesired(Vector3 input, Vector3 normal) {
		Vector3 undesired = normal * Vector3.Dot(input, normal);
		Vector3 desired = input - undesired;
		return desired;
	}

	public void PlaceBomb() {
		if (m_bombsOwned >= BombMax) return;
		if (m_bombCooldown > 0f) return;
		m_bombCooldown = BOMB_COOLDOWN;

		if (GameController.PlaceBombAt(GetMapX(), GetMapY(), this)) {
			m_bombsOwned++;
			m_animator.SetTrigger("DropBomb");
			AudioHelper.PlayOneshot3D(SoundPlaceBomb, m_transform.position, 1f, Random.Range(0.8f, 1.2f));
		}
	}

	public void Jump() {
		if (!m_canJump) return;
		m_canJump = false;
		m_isJumping = true;

		m_rigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.VelocityChange);
		AudioHelper.PlayOneshot3D(SoundJump, m_transform.position, 1f, Random.Range(0.8f, 1.2f));
	}

	public bool IsJumping() {
		return m_isJumping;
	}

	public void TakeDamage() {
		if (m_invulnerability > 0f) return;
		m_invulnerability = DAMAGE_COOLDOWN;

		Health--;
		if (Health <= 0)
			Kill();
		else
			StartCoroutine(InvulnFlash());

		AudioHelper.PlayOneshot3D(SoundHit, m_transform.position, 1f, Random.Range(0.8f, 1.2f));
	}

	public void Kill() {
		this.Health = 0;
		this.enabled = false;

		StopAllCoroutines();
		HideSprite();

		AudioHelper.PlayOneshot3D(SoundKill, m_transform.position, 1f, Random.Range(0.9f, 1.1f));

		Died?.Invoke(this);
	}

	public void HideSprite() {
		SpriteObject.SetActive(false);
		DoritosObject.gameObject.SetActive(false);
		m_reticle?.SetActive(false);
	}

	public bool IsSpriteVisible() {
		return SpriteObject.activeSelf;
	}

	public bool IsDead() {
		return Health <= 0;
	}

	public void SetPawnID(int myID) {
		m_pawnID = myID;
	}

	public int GetPawnID() {
		return m_pawnID;
	}

	private void GameController_BombDetonated(Bomb bomb, Pawn owner) {
		// one of my own bombs exploded, so remove from tally
		if (owner == this) m_bombsOwned--;
	}

	private IEnumerator InvulnFlash() {
		while (m_invulnerability > 0f) {
			SpriteObject.SetActive(false);
			yield return new WaitForSeconds(0.03f);
			SpriteObject.SetActive(true);
			yield return new WaitForSeconds(0.03f);
		}
	}

}
