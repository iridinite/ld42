using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public static class OptionsFile {

	public const string SAVEKEY_CONFIG_VOL_SOUND = "LD42.Config.VolSound";
	public const string SAVEKEY_CONFIG_VOL_MUSIC = "LD42.Config.VolMusic";
	public const string SAVEKEY_CONFIG_SCREENSHAKE = "LD42.Config.Screenshake";
	public const string SAVEKEY_CONFIG_FULLSCREEN = "LD42.Config.Fullscreen";
	public const string SAVEKEY_CONFIG_VSYNC = "LD42.Config.VSync";
	public const string SAVEKEY_CONFIG_RES_W = "LD42.Config.ResW";
	public const string SAVEKEY_CONFIG_RES_H = "LD42.Config.ResH";
	public const string SAVEKEY_CONFIG_GFX_QUALITY = "LD42.Config.Quality";

	public const float GLOBAL_VOL_MUSIC = 0.75f;
	public const float GLOBAL_VOL_SFX = 1f;

	private const byte SETTINGS_FILE_VERSION = 1;

	public static float VolumeSound {
		get { return Mathf.Clamp(GetInt(SAVEKEY_CONFIG_VOL_SOUND, 10), 0, 10) / 10f; }
	}

	public static float VolumeMusic {
		get { return Mathf.Clamp(GetInt(SAVEKEY_CONFIG_VOL_MUSIC, 8), 0, 10) / 10f; }
	}

	public static bool ScreenShake {
		get { return GetInt(SAVEKEY_CONFIG_SCREENSHAKE, 1) == 1; }
	}

	public static bool Fullscreen {
		get { return GetInt(SAVEKEY_CONFIG_FULLSCREEN, 1) == 1; }
	}

	public static bool VSync {
		get { return GetInt(SAVEKEY_CONFIG_VSYNC, 1) == 1; }
	}

	private static readonly Dictionary<string, int> keysInt;
	private static readonly Dictionary<string, float> keysFloat;
	private static readonly Dictionary<string, string> keysString;

	static OptionsFile() {
		// instantiate the dictionaries for storing the key/value pairs
		keysInt = new Dictionary<string, int>();
		keysFloat = new Dictionary<string, float>();
		keysString = new Dictionary<string, string>();

		// load the properties from file
		Load();
		ApplyGraphics();
	}

	/// <summary>
	/// Erases a specified key from each of the dictionaries.
	/// </summary>
	public static void DeleteKey(string key) {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
		key = key.ToUpperInvariant();

		keysInt.Remove(key);
		keysFloat.Remove(key);
		keysString.Remove(key);
	}

	public static void SetInt(string key, int value) {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
		key = key.ToUpperInvariant();

		if (keysInt.ContainsKey(key))
			keysInt[key] = value;
		else
			keysInt.Add(key, value);
	}

	public static int GetInt(string key, int defaultValue = 0) {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

		key = key.ToUpperInvariant();
		return !keysInt.ContainsKey(key)
			? defaultValue
			: keysInt[key];
	}

	public static void SetFloat(string key, float value) {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
		key = key.ToUpperInvariant();

		if (keysFloat.ContainsKey(key))
			keysFloat[key] = value;
		else
			keysFloat.Add(key, value);
	}

	public static float GetFloat(string key, float defaultValue = 0f) {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

		key = key.ToUpperInvariant();
		return !keysFloat.ContainsKey(key)
			? defaultValue
			: keysFloat[key];
	}

	public static void SetString(string key, string value) {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
		key = key.ToUpperInvariant();

		if (keysString.ContainsKey(key))
			keysString[key] = value;
		else
			keysString.Add(key, value);
	}

	public static string GetString(string key, string defaultValue = "") {
		if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

		key = key.ToUpperInvariant();
		return !keysString.ContainsKey(key)
			? defaultValue
			: keysString[key];
	}

	public static void Load() {
		keysInt.Clear();
		keysFloat.Clear();
		keysString.Clear();

		// don't attempt to load a file that doesn't exist
		if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "profile.sav")) return;

		// parse the settings file
		using (FileStream stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "profile.sav", FileMode.Open)) {
			using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8)) {
				if (reader.ReadByte() != SETTINGS_FILE_VERSION) {
					Debug.LogWarning("Settings file in incompatible format, ignoring");
					return;
				}

				int numInts = reader.ReadInt32();
				while (numInts > 0) {
					keysInt.Add(reader.ReadString(), reader.ReadInt32());
					//Debug.Log(keysInt.Last().Key + " = " + keysInt.Last().Value);
					numInts--;
				}

				int numFloats = reader.ReadInt32();
				while (numFloats > 0) {
					keysFloat.Add(reader.ReadString(), reader.ReadSingle());
					//Debug.Log(keysFloat.Last().Key + " = " + keysFloat.Last().Value);
					numFloats--;
				}

				int numStrings = reader.ReadInt32();
				while (numStrings > 0) {
					keysString.Add(reader.ReadString(), reader.ReadString());
					//Debug.Log(keysString.Last().Key + " = " + keysString.Last().Value);
					numStrings--;
				}
			}
		}
	}

	public static void Save() {
		using (FileStream stream = new FileStream(Application.persistentDataPath + Path.DirectorySeparatorChar + "profile.sav", FileMode.Create)) {
			using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8)) {
				writer.Write((byte)SETTINGS_FILE_VERSION);

				writer.Write(keysInt.Count);
				foreach (var pair in keysInt) {
					writer.Write(pair.Key);
					writer.Write(pair.Value);
				}

				writer.Write(keysFloat.Count);
				foreach (var pair in keysFloat) {
					writer.Write(pair.Key);
					writer.Write(pair.Value);
				}

				writer.Write(keysString.Count);
				foreach (var pair in keysString) {
					writer.Write(pair.Key);
					writer.Write(pair.Value);
				}
			}
		}
	}

	public static void ApplyGraphics() {
		int width = PlayerPrefs.GetInt(SAVEKEY_CONFIG_RES_W);
		int height = PlayerPrefs.GetInt(SAVEKEY_CONFIG_RES_H);
		if (width < 1280 || width > 99999) width = Screen.resolutions.Last().width;
		if (height < 720 || height > 99999) height = Screen.resolutions.Last().height;

		// change screen rez
		Screen.SetResolution(width, height, Fullscreen);
		// apply new quality settings
		QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(SAVEKEY_CONFIG_GFX_QUALITY, 4), true);
		QualitySettings.vSyncCount = VSync ? 1 : 0;
	}

}
