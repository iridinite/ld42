using UnityEngine;

public class RandomScale : MonoBehaviour {

	[Range(0f, 4f)] public float XMagnitude;
	[Range(0f, 4f)] public float YMagnitude;
	[Range(0f, 4f)] public float ZMagnitude;

	private void Start() {
		transform.localScale += new Vector3(
			XMagnitude * Random.Range(-1f, 1f),
			YMagnitude * Random.Range(-1f, 1f),
			ZMagnitude * Random.Range(-1f, 1f));
	}

}
