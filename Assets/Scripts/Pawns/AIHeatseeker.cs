using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class AIHeatseeker {

	public class PotentialGoal {
		public MapPoint pos;
		public AIAction action;
		public int score;
	}

	private readonly Pawn m_pawn;

	private readonly List<PotentialGoal> m_hotspots;
	private readonly List<MapPoint> m_accessible;
	private float m_accessibleUpdate;

#if UNITY_EDITOR
	public string m_debug;
#endif

	public AIHeatseeker(Pawn pawn) {
		m_pawn = pawn;
		m_hotspots = new List<PotentialGoal>();
		m_accessible = new List<MapPoint>();
		m_accessibleUpdate = -(pawn.GetPawnID() * 0.2f); // spread different pawns' updates over time
	}

	public void Tick(float dt) {
		m_accessibleUpdate -= dt;
		if (m_accessibleUpdate <= 0f) {
			m_accessibleUpdate += 1f;
			RebuildHeatmap();
		}
	}

	public PotentialGoal GetNextHotspot() {
		// pick any of the top 5 hotspots
		return m_hotspots[Random.Range(0, Mathf.Min(m_hotspots.Count, 5))];
	}

	public bool CanSafelyBombAndRetreat() {
		// not exactly accurate but at least it'll help ensure AI won't bomb the only accessible tile they're standing on
		return m_accessible.Count > 2;
	}

	public void RebuildHeatmap() {
#if UNITY_EDITOR
		Stopwatch sw = Stopwatch.StartNew();
#endif

		m_hotspots.Clear();
		m_accessible.Clear();
		FloodFill(m_pawn.GetMapPos());
		ComputeAllScores();

#if UNITY_EDITOR
		sw.Stop();
		m_debug = $"P{m_pawn.GetPawnID() + 1}: Heatmap ({m_accessible.Count} acc / {m_hotspots.Count} hot) generated in {sw.ElapsedMilliseconds} ms";
#endif
	}

	private void FloodFill(MapPoint center) {
		var stack = new Stack<MapPoint>();
		stack.Push(center);

		while (stack.Count > 0) {
			var point = stack.Pop();
			m_accessible.Add(point);

			for (var dir = Cardinal.North; dir <= Cardinal.West; dir++) {
				var delta = GameController.CardinalToDelta(dir);

				MapPoint neighbor = point + delta;
				if (m_accessible.Contains(neighbor)) continue;
				if (!Map.IsValid(neighbor)) continue;

				var neighborType = Map.GetAt(neighbor);
				if (neighborType == MapTile.Empty) {
					// one empty tile is okay if the next tile in the same direction is floor, because then we can jump across
					var farside = neighbor + delta; // new MapPoint(neighbor.x + dx, neighbor.y + dy);
					if (!Map.IsValid(farside) || Map.GetAt(farside) != MapTile.Floor) continue;

				} else if (neighborType != MapTile.Floor) {
					// ignore walls
					continue;
				}

				// if this tile is in an immediate blast zone, treat it as impassable
				var bombHere = GameController.GetFirstBombInRange(neighbor);
				if (bombHere != null && !bombHere.GetRange().Contains(m_pawn.GetMapPos())) {
					continue;
				}

				stack.Push(neighbor);
			}
		}
	}

	private void ComputeAllScores() {
		foreach (var point in m_accessible) {
			// skip over walls or empty tiles, because we never want to use those as end goals
			if (!Map.IsPassable(point) || Map.GetAt(point) == MapTile.Empty) continue;

			// convert all accessible points into goals
			var goal = new PotentialGoal();
			goal.score = ComputeScore(point, out goal.action);
			goal.pos = point;
			m_hotspots.Add(goal);
		}

		// sort the hotspots by score, higher is better
		m_hotspots.Sort((lhs, rhs) => rhs.score.CompareTo(lhs.score));
	}

	private int ComputeScore(MapPoint point, out AIAction action) {
		int total = 0;

		action = AIAction.Walk;

		// deduct points for high distances from the pawn
		var distOverall = Mathf.Abs(point.x - m_pawn.GetMapX()) + Mathf.Abs(point.y - m_pawn.GetMapY());
		total -= distOverall / 2;

		// accounting for other pawns...
		for (int i = 0; i < GameController.GetNumPawns(); i++) {
			if (i == m_pawn.GetPawnID()) continue;

			// deduct points for remaining far away from other pawns
			var otherPawn = GameController.GetPawn(i);
			var distPawn = m_pawn.GetMapPos().Manhattan(otherPawn.GetMapPos());
			total -= distPawn / 2;

			// prefer tiles near other pawns and try to bomb them
			if (distPawn <= Mathf.Min(m_pawn.BombRange, 3)) {
				total += 12;
				action = AIAction.PlaceBomb;
			}
		}

		// heavily penalize tiles that are in immediate blast zones
		var danger = GameController.ComputeDangerLevel(point);
		if (danger > 0f) {
			total -= (int)(Mathf.Max(0.5f, danger) * 60f);
		}

		// look for nearby crates, and add points if we can blow up any
		for (var dir = Cardinal.North; dir <= Cardinal.West; dir++) {
			var neighbor = GameController.CardinalToDelta(dir) + point;
			if (Map.IsValid(neighbor) && Map.GetAt(neighbor) == MapTile.Crate) {
				action = AIAction.PlaceBomb;
				total += 5;
			}
		}

		// big boost for powerups in this spot, we really want to grab those
		if (GameController.Powerups.Any(pup => pup.GetMapPos() == point))
			total += 25;

		return total;
	}

}
