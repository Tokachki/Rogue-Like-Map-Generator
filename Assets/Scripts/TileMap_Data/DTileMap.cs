using UnityEngine;
using System.Collections.Generic;

public class DTileMap {

	protected class DRoom {
		public int left;
		public int top;
		public int width;
		public int height;

		public bool isConnected = false;

		public int right {
			get { return left + width - 1; }
		}
		public int bottom {
			get { return top + height - 1; }
		}
		public int center_x {
			get { return left + width / 2; }
		}
		public int center_y {
			get { return top + height / 2; }
		}

		public bool CollidesWith(DRoom other) {
			if (left > other.right - 1) 
				return false;

			if (top > other.bottom - 1)
				return false;

			if (right < other.left + 1)
				return false;

			if (bottom < other.top + 1)
				return false;

			return true;
		}
	}

	int sizeX;
	int sizeY;

	int[,] mapData;

	List<DRoom> rooms;

	// 0 = Unknown
	// 1 = Floor
	// 2 = Wall
	// 3 = Stone

	public DTileMap(int sizeX, int sizeY) {
		DRoom r;
		this.sizeX = sizeX;
		this.sizeY = sizeY;

		mapData = new int[sizeX, sizeY];

		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				mapData [x, y] = 3;
			}
		}

		rooms = new List<DRoom> ();

		int maxFails = 10;

		// rooms.Count < # of rooms
		while (rooms.Count < 20) {
			int rsx = Random.Range (8, 16);		// Room Size x
			int rsy = Random.Range (8, 16);		// Room Size y

			r = new DRoom ();
			r.left = Random.Range (0, sizeX - rsx);
			r.top = Random.Range (0, sizeY - rsy);
			r.width = rsx;
			r.height = rsy;

			if (!RoomCollides (r)) {
				rooms.Add (r);
			} else {
				maxFails--;

				if (maxFails <= 0) {
					break;
				}
			}

			foreach (DRoom r2 in rooms) {
				MakeRoom (r2);
			}

			for (int i = 0; i < rooms.Count; i++) {
				if (!rooms[i].isConnected) {
					int j = Random.Range (1, rooms.Count);
					MakeCorridor(rooms[i], rooms[(i + j) % rooms.Count]);
				}
			}

			MakeWalls();
		}
	}

	bool RoomCollides(DRoom r) {
		foreach (DRoom r2 in rooms) {
			if (r.CollidesWith (r2)) {
				return true;
			}
		}

		return false;
	}

	public int GetTileAt(int x, int y) {
			return mapData[x,y];
	}

	void MakeRoom(DRoom r){
		for (int x = 0; x < r.width; x++) {
			for (int y = 0; y < r.height; y++) {
				if (x == 0 || x == r.width -1 || y == 0 || y == r.height - 1) {
					mapData [r.left + x, r.top + y] = 2;
				}
				else {
					mapData [r.left + x, r.top + y] = 1;
				}
			}
		}
	}

	void MakeCorridor(DRoom r1, DRoom r2) {
		int x = r1.center_x;
		int y = r1.center_y;

		while (x != r2.center_x) {
			mapData [x, y] = 1;

			x += x < r2.center_x ? 1 : -1;
		}

		while (y != r2.center_y) {
			mapData [x, y] = 1;

			y += y < r2.center_y ? 1 : -1;
		}

		r1.isConnected = true;
		r2.isConnected = true;
	}

	void MakeWalls() {
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				if (mapData[x, y] == 3 && HasAdjacentFloor(x,y)) {
					mapData[x, y] = 2;
				}
			}
		}
	}

	bool HasAdjacentFloor(int x, int y) {
		if (x > 0 && mapData [x - 1, y] == 1)
			return true;
		if (x < sizeX - 1 && mapData [x + 1, y] == 1)
			return true;
		if (y > 0 && mapData [x, y - 1] == 1)
			return true;
		if (y < sizeY - 1 && mapData [x, y + 1] == 1)
			return true;

		if (x > 0 && y > 0 && mapData [x - 1, y - 1] == 1)
			return true;
		if (x < sizeX - 1 && y  > 0 && mapData [x + 1, y - 1] == 1)
			return true;
		
		if (x > 0 && y < sizeY - 1 && mapData [x - 1, y + 1] == 1)
			return true;
		if (x > sizeX - 1 && y < sizeY - 1 && mapData [x + 1, y + 1] == 1)
			return true;

		return false;
	}
}
