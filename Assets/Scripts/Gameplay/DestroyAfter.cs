using UnityEngine;

public class DestroyAfter : MonoBehaviour {

	public float Delay;

	private void Start() {
		Destroy(gameObject, Delay);
	}

}
