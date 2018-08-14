using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameController {

	public static event Action<Bomb, Pawn> BombPlaced;
	public static event Action<Bomb, Pawn> BombDetonated;
	public static event Action<Pawn> PawnDied;

	private const int MAX_PAWNS = 4;

	private static GameRoot m_root;
	private static Pawn[] m_pawns;
	private static int m_numPawns;
	private static int m_livePawns;
	private static bool m_moveAllowed;

	public static List<Powerup> Powerups;
	private static List<Bomb> m_bombs;

	public static readonly Color[] PawnColors = {
		new Color(1f, 0.15f, 0.15f),
		new Color(0.1f, 0.6f, 1f),
		new Color(0f, 0.8f, 0.15f),
		new Color(1f, 0.8f, 0.15f)
	};

	public static void Initialize(GameRoot root, int pawns) {
		m_root = root;
		m_numPawns = pawns;
		m_livePawns = pawns;
		m_moveAllowed = false;

		BombPlaced = null;
		BombDetonated = null;
		PawnDied = null;

		Powerups = new List<Powerup>();
		m_bombs = new List<Bomb>();
		m_pawns = new Pawn[4];
		for (int i = 0; i < pawns; i++) {
			m_pawns[i] = m_root.CreatePawn(i, i == 0);
			m_pawns[i].SetPawnID(i);
			m_pawns[i].Died += OnPawnDied;
		}

		Map.Initialize();
	}

	private static void OnPawnDied(Pawn pawn) {
		// globally forward this message
		m_livePawns--;
		PawnDied?.Invoke(pawn);
	}

	public static void SetPawnAllowedMove() {
		m_moveAllowed = true;
	}

	public static bool IsPawnAllowedMove() {
		return m_moveAllowed && !IsGameOver();
	}

	public static bool IsGameOver() {
		return GetNumLivePawns() <= 1;
	}

	public static int GetNumPawns() {
		return m_numPawns;
	}

	public static int GetNumLivePawns() {
		return m_livePawns;
	}

	public static Pawn GetFirstLivePawn() {
		return m_pawns.FirstOrDefault(pawn => pawn != null && !pawn.IsDead());
	}

	public static Pawn GetPawn(int index) {
		return m_pawns[index];
	}

	public static bool PlaceBombAt(int x, int y, Pawn owner) {
		// check that no bomb has already been placed here
		foreach (var other in m_bombs) {
			if (other.GetMapX() == x && other.GetMapY() == y)
				return false;
		}

		// create and configure new bomb
		var bomb = m_root.CreateBomb(x, y);
		bomb.SetOwner(owner);
		bomb.SetRange(owner.BombRange);

		m_bombs.Add(bomb);
		BombPlaced?.Invoke(bomb, owner);
		return true;
	}

	public static void OnBombDetonated(Bomb bomb) {
		BombDetonated?.Invoke(bomb, bomb.GetOwner());
	}

	public static void UnregisterBomb(Bomb bomb) {
		Debug.Assert(m_bombs.Contains(bomb));
		m_bombs.Remove(bomb);
	}

	public static Vector3 MapToWorld(MapPoint point) {
		return MapToWorld(point.x, point.y);
	}

	public static Vector3 MapToWorld(int x, int y) {
		return new Vector3(x - Map.MAP_SIZE / 2, 0, y - Map.MAP_SIZE / 2);
	}

	public static void WorldToMap(Vector3 world, out int x, out int y) {
		x = Mathf.RoundToInt(world.x + Map.MAP_SIZE / 2f);
		y = Mathf.RoundToInt(world.z + Map.MAP_SIZE / 2f);
	}

	public static void CardinalToDelta(Cardinal dir, out int dx, out int dy) {
		switch (dir) {
			case Cardinal.North:
				dx = 0;
				dy = 1;
				break;
			case Cardinal.South:
				dx = 0;
				dy = -1;
				break;
			case Cardinal.East:
				dx = 1;
				dy = 0;
				break;
			case Cardinal.West:
				dx = -1;
				dy = 0;
				break;
			default:
				throw new ArgumentException(nameof(dir));
		}
	}

	public static MapPoint CardinalToDelta(Cardinal dir) {
		int dx, dy;
		CardinalToDelta(dir, out dx, out dy);
		return new MapPoint(dx, dy);
	}

	public static Quaternion CardinalToRotation(Cardinal dir) {
		switch (dir) {
			case Cardinal.North:
				return Quaternion.Euler(0, 0, 0);
			case Cardinal.East:
				return Quaternion.Euler(0, 90, 0);
			case Cardinal.South:
				return Quaternion.Euler(0, 180, 0);
			case Cardinal.West:
				return Quaternion.Euler(0, 270, 0);
			default:
				throw new ArgumentException(nameof(dir));
		}
	}

	public static float ComputeDangerLevel(MapPoint point) {
		foreach (var bomb in m_bombs) {
			if (bomb.GetRange().Contains(point))
				return bomb.GetFuseFactor();
		}

		return 0f;
	}

	public static Bomb GetFirstBombInRange(MapPoint point) {
		foreach (var bomb in m_bombs) {
			if (bomb.GetRange().Contains(point))
				return bomb;
		}

		return null;
	}

}
