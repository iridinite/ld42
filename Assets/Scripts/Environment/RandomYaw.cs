using UnityEngine;

public class RandomYaw : MonoBehaviour {

	private void Start() {
		transform.localRotation *= Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);
	}

}
