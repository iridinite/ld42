using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

	public AudioClip SoundClick;

	public GameObject PrefabAscendBeam;

	public GameObject GameOverPanel;
	public GameObject ExitGamePanel;
	public TMP_Text WinnerText;

	private void Start() {
		GameController.PawnDied += this.GameController_PawnDied;
	}

	private void GameController_PawnDied(Pawn pawn) {
		if (!GameController.IsGameOver()) return;

		var winner = GameController.GetFirstLivePawn();
		GameOverPanel.SetActive(true);
		WinnerText.text = $"PLAYER {winner.GetPawnID() + 1}";
		WinnerText.color = GameController.PawnColors[winner.GetPawnID()];
		StartCoroutine(AscendAnimation());
	}

	private IEnumerator AscendAnimation() {
		yield return new WaitForSeconds(1f);

		// so the below shake commands won't be overridden by other things still happening in the map
		ScreenShaker.Clear();

		var winner = GameController.GetFirstLivePawn();

		// disable physics and such on the pawn
		winner.GetComponent<Rigidbody>().detectCollisions = false;
		winner.GetComponent<Rigidbody>().isKinematic = true;

		// spawn the ascend beam and make it invisibly thin to begin with
		var beam = Instantiate(PrefabAscendBeam, winner.transform.position, Quaternion.identity);
		var beamtf = beam.transform;
		beamtf.localScale = new Vector3(0f, 20f, 0f);

		while (beamtf.localScale.x < 1f) {
			// shake the screen as the beam opens up
			ScreenShaker.Shake(999f, beamtf.localScale.x * 0.08f);
			// increase the width of the beam over time
			beamtf.localScale += new Vector3(0.8f * Time.fixedDeltaTime, 0f, 0.8f * Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}

		ScreenShaker.Shake(999f, 0.12f);
		var ascendProgress = 0f;
		while (ascendProgress < 1f) {
			ascendProgress += Time.fixedDeltaTime * 0.6f;

			winner.transform.localPosition += Vector3.up * Time.fixedDeltaTime * 1.2f;
			yield return new WaitForFixedUpdate();
		}

		ExitGamePanel.SetActive(true);

		// and close the beam again
		winner.HideSprite();
		while (beamtf.localScale.x > 0f) {
			ScreenShaker.Clear();
			ScreenShaker.Shake(999f, beamtf.localScale.x * 0.08f);
			// increase the width of the beam over time
			beamtf.localScale -= new Vector3(1f * Time.fixedDeltaTime, 0f, 1f * Time.fixedDeltaTime);
			yield return new WaitForFixedUpdate();
		}

		Destroy(beam);
		ScreenShaker.Clear();
	}

	public void ButtonPlayAgain() {
		AudioHelper.PlayOneshot2D(SoundClick);
		SceneManager.LoadScene("Game");
	}

	public void ButtonToMenu() {
		AudioHelper.PlayOneshot2D(SoundClick);
		SceneManager.LoadScene("Title");
	}

}
