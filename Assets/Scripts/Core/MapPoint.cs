using System;
using UnityEngine;

public struct MapPoint : IEquatable<MapPoint>, IComparable<MapPoint> {

	public int x;
	public int y;

	public MapPoint(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public bool Equals(MapPoint other) {
		return this.x == other.x && this.y == other.y;
	}

	public int CompareTo(MapPoint other) {
		return this.Equals(other)
			? 0
			: Mathf.Clamp(this.x - other.x + this.y - other.y, -1, 1);
	}

	public static bool operator ==(MapPoint lhs, MapPoint rhs) {
		return lhs.Equals(rhs);
	}

	public static bool operator !=(MapPoint lhs, MapPoint rhs) {
		return !(lhs == rhs);
	}

	public static MapPoint operator +(MapPoint lhs, MapPoint rhs) {
		return new MapPoint(lhs.x + rhs.x, lhs.y + rhs.y);
	}

	public static MapPoint operator -(MapPoint lhs, MapPoint rhs) {
		return new MapPoint(lhs.x - rhs.x, lhs.y - rhs.y);
	}

	public static MapPoint operator *(MapPoint lhs, int scalar) {
		return new MapPoint(lhs.x * scalar, lhs.y * scalar);
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		return obj is MapPoint && Equals((MapPoint)obj);
	}

	public override int GetHashCode() {
		unchecked {
			return (x * 397) ^ y;
		}
	}

	public override string ToString() {
		return $"{x}, {y}";
	}

	public int Manhattan(MapPoint other) {
		return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
	}

}
