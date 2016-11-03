using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour {

	public int columns = 100;
	public int rows = 50;
	public float tileSize = 1.0f;
	public int numberOfRooms = 10;

	public Texture2D terrainTiles;
	public int tileResolution;

	// Use this for initialization
	void Start () {
		BuildMesh();
	}

	Color[][] ChopUpTiles() {
		int numberOfTilesPerRow = terrainTiles.width / tileResolution;
		int numberOfRows = terrainTiles.height / tileResolution;

		Color[][] tiles = new Color[numberOfTilesPerRow * numberOfRows][];

		for(int y = 0; y<numberOfRows; y++) {
			for(int x = 0; x<numberOfTilesPerRow; x++) {
				tiles [y * numberOfTilesPerRow + x] = terrainTiles.GetPixels (x * tileResolution, y * tileResolution, tileResolution, tileResolution);
			}
		}

		return tiles;
	}

	void BuildTexture() {
		DTileMap map = new DTileMap(columns, rows, numberOfRooms);

		int textureWidth = columns * tileResolution;
		int textureHeight = rows * tileResolution;
		Texture2D texture = new Texture2D(textureWidth, textureHeight);

		Color[][] tiles = ChopUpTiles();

		for(int y = 0; y < rows; y++) {
			for(int x = 0; x < columns; x++) {
				Color[] pixels = tiles [map.GetTileAt (x, y)];
				texture.SetPixels(x*tileResolution, y*tileResolution, tileResolution, tileResolution, pixels);
			}
		}

		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();

		MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
		mesh_renderer.sharedMaterials[0].mainTexture = texture;

		Debug.Log("Texture Done!");
	}

	public void BuildMesh() {
		int numberOfTiles = columns * rows;
		int numberOfTriangles = numberOfTiles * 2;

		int vsize_x = columns + 1;
		int vsize_z = rows + 1;
		int numberOfVertices = vsize_x * vsize_z;

		// Generate the mesh data
		Vector3[] vertices = new Vector3[ numberOfVertices ];
		Vector3[] normals = new Vector3[numberOfVertices];
		Vector2[] uv = new Vector2[numberOfVertices];

		int[] triangles = new int[ numberOfTriangles * 3 ];

		int x, z;
		for(z = 0; z < vsize_z; z++) {
			for(x =0 ; x < vsize_x; x++) {
				vertices [z * vsize_x + x] = new Vector3 (x * tileSize, 0, -z * tileSize);
				normals [z * vsize_x + x] = Vector3.up;
				uv [z * vsize_x + x] = new Vector2 ((float)x / columns, 1f - (float)z / rows);
			}
		}
		Debug.Log("Vertices Done!");

		for (z = 0; z < rows; z++) {
			for(x = 0; x < columns; x++) {
				int squareIndex = z * columns + x;
				int triangleOffset = squareIndex * 6;
				triangles[triangleOffset + 0] = z * vsize_x + x + 		    0;
				triangles[triangleOffset + 2] = z * vsize_x + x + vsize_x + 0;
				triangles[triangleOffset + 1] = z * vsize_x + x + vsize_x + 1;

				triangles[triangleOffset + 3] = z * vsize_x + x + 		    0;
				triangles[triangleOffset + 5] = z * vsize_x + x + vsize_x + 1;
				triangles[triangleOffset + 4] = z * vsize_x + x + 		    1;
			}
		}

		Debug.Log("Triangles Done!");

		// Create a new Mesh and populate with the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;

		// Assign our mesh to our filter/renderer/collider
		MeshFilter mesh_filter = GetComponent<MeshFilter>();
		MeshCollider mesh_collider = GetComponent<MeshCollider>();

		mesh_filter.mesh = mesh;
		mesh_collider.sharedMesh = mesh;
		Debug.Log("Mesh Done!");

		BuildTexture();
	}
}
