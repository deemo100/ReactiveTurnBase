using UnityEngine;
using System;

/// <summary>
/// 유닛 프리팹을 받아서 IInitializableUnit 을 초기화하고 반환하는 팩토리.
/// </summary>
public class UnitFactory : MonoBehaviour
{
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

        if (rotation == default)
            rotation = Quaternion.identity;

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
}