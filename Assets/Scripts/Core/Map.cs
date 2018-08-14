using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Map {

	public const int MAP_SIZE = 24;
	public const int NUM_CRATES = 110;

	public static event Action<MapPoint> TileRemoved;

	private static MapTile[,] m_grid;

	private static readonly MapPoint[] m_spawns4p = {
		new MapPoint(6, 16),
		new MapPoint(6, 8),
		new MapPoint(18, 16),
		new MapPoint(18, 8)
	};

	private static readonly MapPoint[] m_spawns2p = {
		new MapPoint(12, 20),
		new MapPoint(12, 4)
	};

	public static void Initialize() {
		TileRemoved = null;

		m_grid = new MapTile[MAP_SIZE, MAP_SIZE];
		for (int x = 0; x < MAP_SIZE; x++) {
			for (int y = 0; y < MAP_SIZE; y++) {
				// circular arena, tiles outside become empty
				var dist = Vector2.Distance(new Vector2(x - MAP_SIZE / 2f, y - MAP_SIZE / 2f), Vector2.zero);
				if (dist >= MAP_SIZE / 2f) {
					m_grid[x, y] = MapTile.Void;
					continue;
				}

				// floor as baseline
				m_grid[x, y] = MapTile.Floor;

				// dotted with pillars
				if (x % 2 == 1 && y % 2 == 1)
					m_grid[x, y] = MapTile.Wall;
			}
		}

		for (int i = 0; i < NUM_CRATES; i++) {
			// find a random position that isn't yet occupied by anything
			MapPoint pos;
			do {
				pos.x = Random.Range(0, MAP_SIZE);
				pos.y = Random.Range(0, MAP_SIZE);
			} while (GetAt(pos.x, pos.y) != MapTile.Floor || GetIsNearSpawn(pos));

			// put a crate there
			m_grid[pos.x, pos.y] = MapTile.Crate;
		}
	}

	public static bool IsValid(MapPoint point) {
		return IsValid(point.x, point.y);
	}

	public static bool IsValid(int x, int y) {
		return x >= 0 && y >= 0 && x < MAP_SIZE && y < MAP_SIZE;
	}

	public static MapTile GetAt(MapPoint point) {
		return GetAt(point.x, point.y);
	}

	public static MapTile GetAt(int x, int y) {
		if (!IsValid(x, y)) throw new ArgumentOutOfRangeException();
		return m_grid[x, y];
	}

	private static MapPoint[] GetSpawnPointArray() {
		return GameController.GetNumPawns() <= 2 ? m_spawns2p : m_spawns4p;
	}

	public static MapPoint GetSpawnPoint(int pawnIndex) {
		return GetSpawnPointArray()[pawnIndex];
	}

	public static bool GetIsNearSpawn(MapPoint point) {
		return GetSpawnPointArray().Any(spawn => {
			// true if the point exactly matches a spawn
			if (point == spawn) return true;
			// true if manhattan distance towards spawn is 1
			var manhattan = Mathf.Abs(point.x - spawn.x) + Mathf.Abs(point.y - spawn.y);
			return manhattan <= 1;
		});
	}

	public static bool IsPassable(MapPoint point) {
		return IsPassable(point.x, point.y);
	}

	public static bool IsPassable(int x, int y) {
		// outside map is just impassable, no need to throw errors
		if (!IsValid(x, y)) return false;

		return GetAt(x, y) < MapTile.Wall && GetAt(x, y) != MapTile.Void;
	}

	public static int GetPathCost(MapPoint point) {
		int cost = 0;
		switch (GetAt(point.x, point.y)) {
			case MapTile.Void:
				cost = 999;
				break;
			case MapTile.Empty:
				cost = 8;
				break;
			case MapTile.Floor:
				cost = 1;
				break;
		}

		// heavily penalize tiles that are in immediate blast zones
		var danger = GameController.ComputeDangerLevel(point);
		if (danger > 0f) {
			//Debug.LogWarning($"PATHFIND: {point} is {danger * 100f:F0}% demonic Kappa (-{(int)(danger * 50f)} points)");
			cost += (int)(Mathf.Max(0.5f, danger) * 60f);
		}

		return cost;
	}

	public static void SetAt(int x, int y, MapTile tile) {
		m_grid[x, y] = tile;
	}

	public static void BreakAt(int x, int y) {
		if (m_grid[x, y] == MapTile.Empty) return;

		m_grid[x, y] = MapTile.Empty;
		TileRemoved?.Invoke(new MapPoint(x, y));
	}

}
