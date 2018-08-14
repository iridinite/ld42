using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

	public AudioClip SoundClick;

	public GameObject MenuMain;
	public GameObject MenuPlay;
	public GameObject MenuOptions;

	public void ButtonPlay() {
		AudioHelper.PlayOneshot2D(SoundClick);
		MenuMain.SetActive(false);
		MenuPlay.SetActive(true);
	}

	public void ButtonPlayReally(int pawns) {
		AudioHelper.PlayOneshot2D(SoundClick);
		GameRoot.NumPawns = pawns;
		SceneManager.LoadScene("Game");
	}

	public void ButtonBack() {
		AudioHelper.PlayOneshot2D(SoundClick);
		MenuMain.SetActive(true);
		MenuPlay.SetActive(false);
		MenuOptions.SetActive(false);
	}

	public void ButtonOptions() {
		AudioHelper.PlayOneshot2D(SoundClick);
		MenuMain.SetActive(false);
		MenuOptions.SetActive(true);
	}

	public void ButtonExit() {
		AudioHelper.PlayOneshot2D(SoundClick);
		Application.Quit();
	}

}
