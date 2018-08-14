using UnityEngine;

public class MusicManager : MonoBehaviour {

	public AudioClip[] Songs;

	private AudioSource m_src;
	private bool m_playing;

	private void Start() {
		m_playing = false;
		m_src = gameObject.AddComponent<AudioSource>();
		m_src.spatialBlend = 0f;
		m_src.bypassEffects = true;
		m_src.bypassListenerEffects = true;
		m_src.volume = OptionsFile.GLOBAL_VOL_MUSIC * OptionsFile.VolumeMusic;
		m_src.loop = true;
		m_src.clip = Songs[Random.Range(0, Songs.Length)];
	}

	private void FixedUpdate() {
		if (m_playing || !GameController.IsPawnAllowedMove()) return;

		m_playing = true;
		m_src.Play();
	}

}
