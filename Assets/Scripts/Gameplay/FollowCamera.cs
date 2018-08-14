using UnityEngine;

public class FollowCamera : MonoBehaviour {

	private Camera m_camera;
	private Transform m_transform;

	[Header("Orthographic Size")]
	public AnimationCurve DistanceCurve;
	public float DistanceFactor;
	public float HeightFactor;

	[Header("Interpolation")]
	[Range(0f, 20f)] public float SpeedTranslate;
	[Range(0f, 20f)] public float SpeedZoom;

	private void Awake() {
		m_camera = GetComponentInChildren<Camera>();
		m_transform = GetComponent<Transform>();
	}

	private void FixedUpdate() {
		var groundTarget = Vector3.zero;
		var pawnMaxDist = 0f;

		// find the average position of all pawns
		for (int i = 0; i < GameController.GetNumPawns(); i++) {
			var pawn = GameController.GetPawn(i);
			if (pawn.IsDead()) continue;

			var pawnPosition = pawn.transform.position;
			groundTarget += pawnPosition;

			// find the largest distance between any two pawns
			for (int j = 0; j < GameController.GetNumPawns(); j++) {
				if (i == j) continue;

				var otherPawn = GameController.GetPawn(j);
				if (otherPawn.IsDead()) continue;

				var otherPosition = GameController.GetPawn(j).transform.position;
				var dist = Vector3.Distance(pawnPosition, otherPosition);
				if (dist > pawnMaxDist) pawnMaxDist = dist;
			}
		}

		groundTarget /= GameController.GetNumLivePawns();

		// move the ground target up into the air
		var cameraAngle = new Vector3(0f, 1f, -0.72f).normalized * 12f;
		var cameraTarget = groundTarget + cameraAngle;

		// interpolate
		m_transform.localPosition += (cameraTarget - m_transform.localPosition) * Time.fixedDeltaTime * SpeedTranslate;
		m_camera.orthographicSize += (DistanceCurve.Evaluate(Mathf.Clamp01(pawnMaxDist / DistanceFactor)) * HeightFactor - m_camera.orthographicSize) * Time.fixedDeltaTime * SpeedZoom;
	}

}
