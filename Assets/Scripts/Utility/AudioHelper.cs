using UnityEngine;

public static class AudioHelper {

	//private static AudioHelper m_inst;
	private static AudioSource m_2D;

	private static AudioSource Get2D() {
		if (m_2D == null) {
			var gobj = new GameObject("Audio Helper (2D)");
			//m_inst = gobj.AddComponent<AudioHelper>();
			m_2D = gobj.AddComponent<AudioSource>();
			Object.DontDestroyOnLoad(gobj);
		}

		return m_2D;
	}

	public static void PlayOneshot2D(AudioClip clip) {
		Get2D().PlayOneShot(clip, OptionsFile.GLOBAL_VOL_SFX * OptionsFile.VolumeSound);
	}

	public static void PlayOneshot3D(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f) {
		var gobj = new GameObject("Audio Helper (3D)");
		gobj.transform.position = position;

		var src = gobj.AddComponent<AudioSource>();
		src.clip = clip;
		src.spatialBlend = 0.7f;
		src.minDistance = 4f;
		src.volume = volume * OptionsFile.GLOBAL_VOL_SFX * OptionsFile.VolumeSound;
		src.pitch = pitch;
		src.Play();

		Object.Destroy(gobj, clip.length * (Time.timeScale >= 0.01f ? Time.timeScale : 0.01f));
	}

}
