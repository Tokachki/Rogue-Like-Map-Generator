using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TGMapInspector : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		if (GUILayout.Button("Generate")) {
			TileMap tileMap = target as TileMap;
			tileMap.BuildMesh ();
		}
	}
}
