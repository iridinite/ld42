using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Thing {

	private const float FUSE_TIME = 3.25f;
	private const float DELAY_INCREMENT = 0.06f;

	public GameObject PrefabExplosion;

	public GameObject PrefabExploCenter;
	public GameObject PrefabExploDir;

	public AudioClip[] SoundExplosion;

	public AnimationCurve FuseRoll;
	public AnimationCurve FuseScale;

	private List<MapPoint> m_cachedRange;
	private List<Cardinal> m_cachedCardinals;

	private float m_fuse;
	private Pawn m_owner;
	private int m_range;
	private bool m_exploded;

	private void Start() {
		m_fuse = FUSE_TIME;
		m_exploded = false;
	}

	private void Update() {
		m_transform.localScale = Vector3.one * FuseScale.Evaluate(1f - m_fuse / FUSE_TIME);

		m_fuse -= Time.deltaTime;
		if (m_fuse > 0f) return;

		Explode();
	}

	public float GetFuseFactor() {
		return m_fuse / FUSE_TIME;
	}

	public Pawn GetOwner() {
		return m_owner;
	}

	public void SetOwner(Pawn owner) {
		m_owner = owner;
	}

	public void SetRange(int range) {
		m_range = range;
	}

	public void ChainExplode() {
		// set the fuse to explode very soon (or leave it if it's actually already shorter)
		m_fuse = Mathf.Min(m_fuse, 0.01f);
	}

	public List<MapPoint> GetRange(bool forceRebuild = false) {
		if (m_cachedRange == null || forceRebuild) {
			m_cachedRange = new List<MapPoint>();
			m_cachedRange.Add(GetMapPos());

			for (Cardinal dir = Cardinal.North; dir <= Cardinal.West; dir++) {
				for (int range = 1; range <= m_range; range++) {
					var point = (GameController.CardinalToDelta(dir) * range) + GetMapPos();
					// stop when hitting a wall
					if (!Map.IsValid(point)) break;
					var tile = Map.GetAt(point);
					if (tile == MapTile.Wall) break;
					// spawn explosion manager
					m_cachedRange.Add(point);
					// if we're going through a crate, stop here instead (after the explosion was placed)
					if (tile == MapTile.Crate) break;
				}
			}
		}

		return m_cachedRange;
	}

	public void Explode() {
		if (m_exploded) return;
		m_exploded = true;

		var range = GetRange(true);
		foreach (var point in range) {
			SpawnExplosion(point, point.Manhattan(GetMapPos()) * DELAY_INCREMENT);
		}

		// break the tile underneath this bomb
		Map.BreakAt(GetMapX(), GetMapY());

		GameController.OnBombDetonated(this);
		GetComponentInChildren<Renderer>().enabled = false;

		StartCoroutine(DestroyCoroutine());

		AudioHelper.PlayOneshot3D(SoundExplosion[Random.Range(0, SoundExplosion.Length)], m_transform.position, 1f, Random.Range(0.8f, 1.2f));
	}

	private void SpawnExplosion(MapPoint point, float delay) {
		var gobj = Instantiate(PrefabExplosion);
		gobj.transform.position = GameController.MapToWorld(point) + Vector3.up;

		var expl = gobj.GetComponent<BombExplosion>();
		expl.PrefabParticles = point == GetMapPos() ? PrefabExploCenter : PrefabExploDir;
		expl.Owner = m_owner;
		expl.Delay = delay;
	}

	private IEnumerator DestroyCoroutine() {
		yield return new WaitForSeconds(1f);

		// remove the bomb from the game
		// > we make the bomb itself last slightly longer so that AI won't immediately treat the bomb as gone,
		// > even though the blast radius is still active and might hurt them.
		GameController.UnregisterBomb(this);
		Destroy(gameObject);
	}

}
