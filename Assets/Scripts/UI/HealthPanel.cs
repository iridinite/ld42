using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanel : MonoBehaviour {

	private Pawn m_pawn;

	public GameObject PrefabHeartIcon;
	public Sprite IconHeartFull, IconHeartDrained;

	private List<Image> m_hearts;
	private int m_lastKnownHealth;

	private void Start() {
		m_hearts = new List<Image>();
		m_pawn = GetComponentInParent<PlayerPanel>().MyPawn;
		m_lastKnownHealth = -1;
	}

	private void Update() {
		if (m_lastKnownHealth == m_pawn.Health) return;
		m_lastKnownHealth = m_pawn.Health;

		while (m_lastKnownHealth > m_hearts.Count) {
			// need to instantiate new hearts
			var gobj = Instantiate(PrefabHeartIcon, transform);
			var rect = gobj.GetComponent<RectTransform>();
			rect.anchoredPosition = new Vector2((m_hearts.Count % 3) * 32f - 32f, (int)(m_hearts.Count / 3f) * -32f - 8f);
			m_hearts.Add(gobj.GetComponent<Image>());
		}

		// update all heart icons
		for (int i = 0; i < m_hearts.Count; i++)
			m_hearts[i].sprite = (m_pawn.Health - 1 >= i) ? IconHeartFull : IconHeartDrained;
	}

}
