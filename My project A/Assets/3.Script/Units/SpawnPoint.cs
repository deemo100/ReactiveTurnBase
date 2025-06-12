using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SpawnPoint : MonoBehaviour
{
    [Tooltip("이 포인트에서 소환할 프리팹")]
    public GameObject prefab;

    [Tooltip("이 프리팹으로 InitStat 에 넘길 statId")]
    public int statId = 1;
}