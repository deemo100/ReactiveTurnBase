using UnityEngine;
using System;

/// <summary>
/// 유닛 프리팹을 받아서 IInitializableUnit 을 초기화하고 반환하는 팩토리.
/// </summary>
public class UnitFactory : MonoBehaviour
{
    [Header("기존 제네릭용 (필요 없으면 지워도 됩니다)")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    /// <summary>
    /// SpawnPoint 방식으로 사용할 때 호출할 오버로드.
    /// prefab, statId, position, (optional) rotation 을 직접 지정합니다.
    /// </summary>
    public MonoBehaviour Create(
        GameObject prefab,
        int statId,
        Vector3 position,
        Quaternion rotation = default
    )
    {
        if (prefab == null)
        {
            Debug.LogError("UnitFactory.Create: prefab이 null입니다.");
            return null;
        }

        if (rotation == default) rotation = Quaternion.identity;

        var go = Instantiate(prefab, position, rotation);
        var init = go.GetComponent<IInitializableUnit>();
        if (init == null)
        {
            Debug.LogError($"{prefab.name}에 IInitializableUnit 구현체가 없습니다.");
            Destroy(go);
            return null;
        }

        init.InitStat(statId);
        return init as MonoBehaviour;
    }

    /// <summary>
    /// 기존 방식 호환용 제네릭 Create.
    /// playerPrefab 또는 enemyPrefab 슬롯에서 꺼내 씁니다.
    /// (SpawnPoint 흐름으로 전환했다면 호출할 일이 없을 수도 있습니다.)
    /// </summary>
    public T Create<T>(int statId, Vector3 position)
        where T : MonoBehaviour, IInitializableUnit
    {
        GameObject prefab = null;
        Quaternion rotation = Quaternion.identity;

        if (typeof(T) == typeof(PlayerUnit))
        {
            prefab = playerPrefab;
            rotation = Quaternion.Euler(0,180,0); // 예: 플레이어만 뒤집어서 소환
        }
        else if (typeof(T) == typeof(EnemyUnit))
        {
            prefab = enemyPrefab;
            rotation = Quaternion.identity;
        }
        else
        {
            throw new System.Exception($"지원하지 않는 타입: {typeof(T).Name}");
        }

        if (prefab == null)
        {
            Debug.LogError($"UnitFactory.Create<{typeof(T).Name}>: prefab 슬롯이 비어있습니다.");
            return null;
        }

        var go = Instantiate(prefab, position, rotation);
        var comp = go.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogError($"{prefab.name}에 {typeof(T).Name} 컴포넌트가 없습니다.");
            Destroy(go);
            return null;
        }

        comp.InitStat(statId);
        return comp;
    }
}
