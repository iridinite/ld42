using TMPro;
using UnityEngine;

public class PlayerPanel : MonoBehaviour {

	public Pawn MyPawn { get; set; }

	public TMP_Text NameText;
	public GameObject CrossoutIcon;
	public GameObject BackPanel;

	private void Start() {
		NameText.text = $"P{MyPawn.GetPawnID() + 1}";
		NameText.color = GameController.PawnColors[MyPawn.GetPawnID()];
		CrossoutIcon.SetActive(false);

		var offset = (GameController.GetNumPawns() - 1) * -75f;
		GetComponent<RectTransform>().anchoredPosition = new Vector2(offset + MyPawn.GetPawnID() * 150f, -90f);

		MyPawn.Died += this.MyPawn_Died;
	}

	private void MyPawn_Died(Pawn pawn) {
		CrossoutIcon.SetActive(true);
		BackPanel.SetActive(false);
	}

}
