using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal static class AIPathfinder {

	private class AStarNode {
		public MapPoint position;
		public AStarNode parent;
		public int g;
		public int h;

		public AStarNode(MapPoint pos, MapPoint goal, AStarNode parent) {
			this.position = pos;
			this.parent = parent;
			this.g = parent?.g + Map.GetPathCost(pos) ?? 0;
			this.h = Mathf.Abs(pos.x - goal.x) + Mathf.Abs(pos.y - goal.y);
		}

		public int f() {
			return g + h;
		}
	}

	private static readonly List<AStarNode> open = new List<AStarNode>();
	private static readonly List<AStarNode> closed = new List<AStarNode>();

	public static List<MapPoint> Pathfind(MapPoint from, MapPoint to) {
		if (from == to) return new List<MapPoint> {to};

		closed.Clear();
		open.Clear();
		open.Add(new AStarNode(from, to, null));

		while (true) {
			// no route exists
			if (open.Count == 0) return null;

			// sort the open list
			open.Sort((lhs, rhs) => {
				if (lhs.f() == rhs.f())
					return 0;
				return lhs.f() > rhs.f() ? 1 : -1;
			});

			// get next node
			var current = open[0];
			if (current.position == to) {
				// goal reached
				var path = new List<MapPoint>();
				do {
					path.Add(current.position);
					current = current.parent;
#if UNITY_EDITOR
					if (current == null)
						Debug.LogError("invalid route? current == null");
#endif
				} while (current.position != from);

				path.Reverse();
				return path;
			}

			// requeue this current node to the closed list, and iterate over its neighbors
			open.RemoveAt(0);
			closed.Add(current);
			EnqueueNeighbors(current.position, to, current);
		}
	}

	private static AStarNode FindNodeByPoint(IEnumerable<AStarNode> list, MapPoint point) {
		return list.FirstOrDefault(node => node.position == point);
	}

	private static void EnqueueNeighbors(MapPoint point, MapPoint goal, AStarNode parent) {
		for (Cardinal dir = Cardinal.North; dir <= Cardinal.West; dir++) {
			int dx, dy;
			GameController.CardinalToDelta(dir, out dx, out dy);

			EnqueuePoint(new MapPoint(point.x + dx, point.y + dy), goal, parent);
		}
	}

	private static void EnqueuePoint(MapPoint point, MapPoint goal, AStarNode parent) {
		// cannot traverse impassable tiles
		if (!Map.IsPassable(point)) return;

		// if node is closed, exit
		if (FindNodeByPoint(closed, point) != null) return;

		// if node is listed in open list, reconsider its cost
		var node = new AStarNode(point, goal, parent);
		var listed = FindNodeByPoint(open, point);
		if (listed != null) {
			if (node.f() < listed.f()) {
				// this new path has a better cost than the previously known cost, so update this node
				listed.g = node.g;
				listed.h = node.h;
				listed.parent = node.parent;
			}

			return;
		}

		// not yet known, so add to open list
		open.Add(node);
	}

}
