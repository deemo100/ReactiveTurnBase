using UnityEngine;
using System;

public class UnitFactory : MonoBehaviour
{
    public Unit Create(
        GameObject prefab,
        int statId,
        Vector3 position,
        TeamType team,                      // ⭐ 추가
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

        var unit = go.GetComponent<Unit>();
        if (unit == null)
        {
            Debug.LogError($"{prefab.name}에 Unit 컴포넌트가 없습니다.");
            Destroy(go);
            return null;
        }

        if (!DataManager.Instance.UnitStatTable.TryGetValue(statId, out var stat))
        {
            Debug.LogError($"UnitStatTable에 statId={statId}가 없습니다.");
            Destroy(go);
            return null;
        }

        unit.Init(stat, team); // ⭐ 2개 파라미터로 호출
        return unit;
    }
}