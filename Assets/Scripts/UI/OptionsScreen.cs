using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsScreen : MonoBehaviour {

	public AudioClip SoundTweak;

	public Slider SliderSound, SliderMusic;
	public Toggle ToggleScreenShake, ToggleVsync;
	public TMP_Dropdown DropResolution, DropQuality;

	private List<Resolution> m_SupportedRez;

	private bool m_Ready;

	private void Awake() {
		m_Ready = false;

		// add all resolutions that are at least 720p
		m_SupportedRez = new List<Resolution>();
		m_SupportedRez.AddRange(Screen.resolutions.Where(rez => rez.width >= 1280 && rez.height >= 720));
		DropResolution.AddOptions(m_SupportedRez.Select(rez => $"{rez.width} x {rez.height}").ToList());
		DropQuality.AddOptions(QualitySettings.names.ToList());

		// convert the stored resolution back into an array index
		var rezWidth = PlayerPrefs.GetInt(OptionsFile.SAVEKEY_CONFIG_RES_W);
		var rezHeight = PlayerPrefs.GetInt(OptionsFile.SAVEKEY_CONFIG_RES_H);
		var rezIndex = m_SupportedRez.FindIndex(rez => rez.width == rezWidth && rez.height == rezHeight);
		// if the stored resolution was not found, then used the last one (which is biggest)
		if (rezIndex == -1) rezIndex = m_SupportedRez.Count - 1;

		DropResolution.value = rezIndex;
		DropQuality.value = PlayerPrefs.GetInt(OptionsFile.SAVEKEY_CONFIG_GFX_QUALITY, QualitySettings.GetQualityLevel());
		SliderSound.value = OptionsFile.VolumeSound * SliderSound.maxValue;
		SliderMusic.value = OptionsFile.VolumeMusic * SliderMusic.maxValue;
		ToggleScreenShake.isOn = OptionsFile.ScreenShake;
		ToggleVsync.isOn = OptionsFile.VSync;
		m_Ready = true;
	}

	public void UpdatePrefs() {
		// skip saving changes if we're still setting up the form
		if (!m_Ready) return;

		AudioHelper.PlayOneshot2D(SoundTweak);

		// some settings are computer-specific so we store those in PlayerPrefs
		PlayerPrefs.SetInt(OptionsFile.SAVEKEY_CONFIG_RES_W, m_SupportedRez[DropResolution.value].width);
		PlayerPrefs.SetInt(OptionsFile.SAVEKEY_CONFIG_RES_H, m_SupportedRez[DropResolution.value].height);
		PlayerPrefs.SetInt(OptionsFile.SAVEKEY_CONFIG_GFX_QUALITY, DropQuality.value);

		// the rest goes into the cloud-stored options file
		OptionsFile.SetInt(OptionsFile.SAVEKEY_CONFIG_VOL_SOUND, (int)SliderSound.value);
		OptionsFile.SetInt(OptionsFile.SAVEKEY_CONFIG_VOL_MUSIC, (int)SliderMusic.value);
		OptionsFile.SetInt(OptionsFile.SAVEKEY_CONFIG_SCREENSHAKE, ToggleScreenShake.isOn ? 1 : 0);
		OptionsFile.SetInt(OptionsFile.SAVEKEY_CONFIG_VSYNC, ToggleVsync.isOn ? 1 : 0);
	}

	public void Save() {
		PlayerPrefs.Save();
		OptionsFile.Save();
	}

	public void ApplyGraphics() {
		OptionsFile.ApplyGraphics();
	}

}
