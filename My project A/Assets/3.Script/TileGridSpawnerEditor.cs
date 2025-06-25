#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileGridSpawner))]
public class TileGridSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TileGridSpawner myScript = (TileGridSpawner)target;
        if (GUILayout.Button("Spawn Tiles"))
        {
            myScript.SpawnTiles();
        }
    }
}
#endif