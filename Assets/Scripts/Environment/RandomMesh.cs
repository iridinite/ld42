using UnityEngine;

public class RandomMesh : MonoBehaviour {

	public Mesh[] Candidates;

	private void Start() {
		GetComponent<MeshFilter>().mesh = Candidates[Random.Range(0, Candidates.Length)];
	}

}
