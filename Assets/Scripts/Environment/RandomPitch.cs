using UnityEngine;

public class RandomPitch : MonoBehaviour {

	private void Start() {
		transform.localRotation *= Quaternion.Euler(Random.Range(0, 4) * 90f, 0f, 0f);
	}

}
