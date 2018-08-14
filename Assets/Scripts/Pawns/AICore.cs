using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class AICore {

	private readonly Pawn m_pawn;
	private readonly AIHeatseeker m_heatmap;

	private AIAction m_action;
	private float m_thinkTimer;

	private List<MapPoint> m_path;
	private MapPoint m_farTarget;

	public AICore(Pawn pawn) {
		m_pawn = pawn;
		m_heatmap = new AIHeatseeker(pawn);

		//GameController.BombDetonated += OnBombPlacedOrDetonated;
		GameController.BombPlaced += OnBombPlacedOrDetonated;
	}

	public void Tick(float dt) {
		m_heatmap.Tick(dt);

		m_thinkTimer -= dt;
		if (m_thinkTimer < 0f) {
			m_thinkTimer = 2f;
			MakeNewAction();
		}

		if (m_path != null)
			TickMovement();
	}

	private void OnBombPlacedOrDetonated(Bomb bomb, Pawn owner) {
		// force the AI to reevaluable its surroundings
		m_heatmap.RebuildHeatmap();

		// if the action affected our planned movement path, we must reroute
		if (m_path != null && m_path.Contains(bomb.GetMapPos()))
			InvalidateAction();
	}

#if UNITY_EDITOR
	public string GetDebugString() {
		return m_heatmap.m_debug;
	}
#endif

	public void InvalidateAction() {
		m_action = AIAction.Idle;
		m_path = null;
	}

	public void MakeNewAction() {
		// never change direction mid-jump
		if (m_pawn.IsJumping()) return;

		var hotspot = m_heatmap.GetNextHotspot();
		m_farTarget = hotspot.pos;
		m_action = hotspot.action;

		m_path = AIPathfinder.Pathfind(m_pawn.GetMapPos(), m_farTarget);
	}

	private void OnDestinationReached() {
		m_thinkTimer = 0.15f;

		if (m_action == AIAction.PlaceBomb) {
			// || (m_heatmap.CanSafelyBombAndRetreat() && Random.Range(0, 4) == 0))
			m_heatmap.RebuildHeatmap();
			if (m_heatmap.CanSafelyBombAndRetreat()) {
				m_pawn.PlaceBomb();
			} else {
				m_action = AIAction.Idle;
				m_thinkTimer = 1f;
				//Debug.LogWarning($"P{m_pawn.GetPawnID() + 1}: Skipping bomb place @ {m_pawn.GetMapPos()} because danger");
			}
		}
	}

	private void TickMovement() {
		if (m_path.Count == 0) {
			m_pawn.DesiredMove = Vector2.zero;
			return;
		}

		var nearTarget = m_path[0];
		//var mapDelta = new Vector2(nearTarget.x - m_pawn.GetMapX(), nearTarget.y - m_pawn.GetMapY());
		var worldDelta = GameController.MapToWorld(nearTarget) - m_pawn.GetTransform().position;
		worldDelta.y = 0f;

		var worldDist = worldDelta.magnitude;

		if (worldDist < 0.75f && Map.GetAt(nearTarget) <= MapTile.Empty && !m_pawn.IsJumping())
			m_pawn.Jump();

		if (worldDist < 0.1f) {
			// arrived at the tile, snap to it and dequeue this path element
			m_pawn.SetMapPos(nearTarget);
			m_path.RemoveAt(0);

			if (m_path.Count == 0)
				OnDestinationReached();
			return;
		}

		// otherwise keep moving in that direction
		worldDelta.Normalize();
		m_pawn.DesiredMove = new Vector2(worldDelta.x, worldDelta.z);
	}

}
