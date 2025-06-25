using UnityEngine;

public class TileGridSpawner : MonoBehaviour
{
    [Header("여러 종류 타일 프리팹 (배열로 할당)")]
    public GameObject[] tilePrefabs;  // Inspector에 원하는 프리팹 여러 개 등록
    public int width = 10;
    public int height = 10;
    public float tileSize = 1.0f;     // 각 타일의 간격

    [ContextMenu("Spawn Tiles")]
    public void SpawnTiles()
    {
        // 이미 배치된 기존 타일 전부 삭제 (클린업)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // 그리드에 타일 자동 배치
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // 여러 종류 프리팹 중 랜덤 선택
                int idx = Random.Range(0, tilePrefabs.Length);
                GameObject prefab = tilePrefabs[idx];

                Vector3 pos = new Vector3(x * tileSize, 0, z * tileSize);
                Instantiate(prefab, pos, Quaternion.identity, this.transform);
            }
        }
    }
}