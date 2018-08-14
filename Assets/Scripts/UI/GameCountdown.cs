using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCountdown : MonoBehaviour {

	public AnimationCurve ScaleCurve;
	public AnimationCurve OpacityCurve;

	public AudioClip Countdown1, Countdown2;

	public Image Fader;
	public TMP_Text CountdownText;

	public bool DebugSkipCountdown = false;

	private void Start() {
		//Time.timeScale = Mathf.Epsilon;
		StartCoroutine(Animation());
	}

	private IEnumerator Animation() {
#if UNITY_EDITOR
		if (DebugSkipCountdown) {
			Fader.gameObject.SetActive(false);
			CountdownText.gameObject.SetActive(false);
			GameController.SetPawnAllowedMove();
			yield break;
		}
#endif

		// skip the first frame, so that our delta-time isn't skewed by loading times
		Fader.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.1f);

		// fade in
		float alpha = 1f;
		while (alpha > 0f) {
			alpha -= Time.fixedDeltaTime;
			Fader.color = Color.black * alpha;
			yield return new WaitForFixedUpdate();
		}

		Fader.gameObject.SetActive(false);
		CountdownText.gameObject.SetActive(true);

		// countdown
		for (int i = 3; i >= 0; i--) {
			if (i == 0) {
				AudioHelper.PlayOneshot2D(Countdown2);
				GameController.SetPawnAllowedMove();
				CountdownText.text = "GO!";
			} else {
				AudioHelper.PlayOneshot2D(Countdown1);
				CountdownText.text = i.ToString();
			}

			float progress = 0f;
			while (progress < 1f) {
				progress += Time.fixedDeltaTime;

				var scale = ScaleCurve.Evaluate(progress);
				CountdownText.rectTransform.localScale = new Vector3(scale, scale, scale);
				CountdownText.alpha = OpacityCurve.Evaluate(progress);

				yield return new WaitForFixedUpdate();
			}

		}

		CountdownText.gameObject.SetActive(false);
	}

}
