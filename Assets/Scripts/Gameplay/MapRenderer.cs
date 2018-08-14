using System;
using UnityEngine;

public class MapRenderer : MonoBehaviour {

	public GameObject TilePrefab;

	private TileObject[,] m_grid;

	private void Awake() {
		m_grid = new TileObject[Map.MAP_SIZE, Map.MAP_SIZE];
	}

	private void Start() {
		Map.TileRemoved += Map_OnTileRemoved;

		var container = new GameObject("Map Container");
		for (int x = 0; x < Map.MAP_SIZE; x++) {
			for (int y = 0; y < Map.MAP_SIZE; y++) {
				var gobj = Instantiate(TilePrefab, GameController.MapToWorld(x, y), Quaternion.identity, container.transform);
				var tile = gobj.GetComponent<TileObject>();
				tile.SetTile(Map.GetAt(x, y));
				m_grid[x, y] = tile;
			}
		}
	}

	private void Map_OnTileRemoved(MapPoint point) {
		m_grid[point.x, point.y].Break();
		m_grid[point.x, point.y] = null;
	}

}
