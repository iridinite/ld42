using UnityEngine;

public class RandomPageIndex : MonoBehaviour {

	public int NumberOfPages;

	private void Start() {
		var block = new MaterialPropertyBlock();
		block.SetFloat("_PageIndex", Random.Range(0, NumberOfPages));
		GetComponent<Renderer>().SetPropertyBlock(block);
	}

}
