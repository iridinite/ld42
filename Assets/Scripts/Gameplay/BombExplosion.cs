using System.Collections;
using UnityEngine;

public class BombExplosion : MonoBehaviour {

	public GameObject PrefabParticles;

	public Pawn Owner;
	public float Delay;

	private Collider m_collider;

	private void Start() {
		m_collider = GetComponent<Collider>();
		StartCoroutine(Cycle());
	}

	private IEnumerator Cycle() {
		yield return new WaitForSeconds(Delay);

		var gobj = Instantiate(PrefabParticles, transform);
		gobj.transform.localPosition = Vector3.zero;
		gobj.transform.localRotation = Quaternion.identity;

		ScreenShaker.Shake(0.3f, 0.3f);
		XInputWrapper.Instance.AddRumble(0.4f, 0.95f);

		m_collider.enabled = true;
		yield return new WaitForSeconds(0.15f);

		m_collider.enabled = false;
		yield return new WaitForSeconds(1f);

		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider hit) {
		var pawn = hit.gameObject.GetComponent<Pawn>();
		if (pawn != null)
			pawn.TakeDamage();

		var crate = hit.gameObject.GetComponent<Crate>();
		if (crate != null)
			crate.Break();

		var bomb = hit.gameObject.GetComponent<Bomb>();
		if (bomb != null)
			bomb.ChainExplode();
	}

}
