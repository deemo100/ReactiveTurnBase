using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Input;
using UnityEngine.InputSystem; // 반드시 필요!

public class InputServiceNew : MonoBehaviour
{
    private List<Vector3> clickPositions = new List<Vector3>();
    
    private List<PlayerUnit> _players;
    private PlayerUnit _selectedUnit;
    private UniTaskCompletionSource<PlayerUnit> _selectTcs;
    private UniTaskCompletionSource<PlayerAction> _actionTcs;

    // (UI 버튼/기타 모드 전환용 변수 - 필요시 확장)
    private bool _skillMode;
    private SkillData _selectedSkill;

    public void SetPlayerUnits(List<PlayerUnit> players)
    {
        Debug.Log("[InputServiceNew] SetPlayerUnits 호출");
        _players = players;
        foreach (var unit in players)
        {
            unit.ResetTurn();
        }
        _selectedUnit = null;
    }

    public async UniTask<PlayerUnit> WaitForUnitSelect()
    {
        Debug.Log("[InputServiceNew] WaitForUnitSelect 진입, 선택 대기 중...");
        _selectedUnit = null;
        _selectTcs = new UniTaskCompletionSource<PlayerUnit>();
        // 대기: Raycast에서 TrySetResult로 해제
        return await _selectTcs.Task;
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screenPos = Mouse.current.position.ReadValue();

            // 3D Ray 생성 (카메라에서 마우스 위치로)
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // RaycastHit 결과
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 hitPos = hit.point;
                clickPositions.Add(hitPos);
                if (clickPositions.Count > 20)
                    clickPositions.RemoveAt(0);

                // PlayerUnit 감지 (3D Collider가 붙어있어야 함!)
                var unit = hit.collider.GetComponent<PlayerUnit>();
                if (unit != null && !unit.HasActedThisTurn)
                {
                    if (_selectedUnit != null)
                        _selectedUnit.SetSelected(false);

                    _selectedUnit = unit;
                    unit.SetSelected(true);

                    Debug.Log($"[선택] {unit.UnitName}이 선택됨 (3D Raycast)");
                }
                else
                {
                    Debug.Log("[Raycast] 클릭한 오브젝트: " + hit.collider.name);
                }
            }
        }
    }

    public async UniTask<PlayerAction> WaitForPlayerAction(PlayerUnit p)
    {
        Debug.Log($"[InputServiceNew] WaitForPlayerAction 진입: {p.UnitName}");
        SetActivePlayer(p);
        _actionTcs = new UniTaskCompletionSource<PlayerAction>();
        // 실제 행동 입력(스킬/공격/타겟) 로직은 추가 구현 필요
        // (예: OnTrigger, 버튼 이벤트 등에서 TrySetResult 호출)
        return await _actionTcs.Task;
    }

    public void SetActivePlayer(PlayerUnit p)
    {
        Debug.Log($"[InputServiceNew] SetActivePlayer 호출: {p.UnitName}");
        _selectedUnit = p;
        // 공격/스킬 모드 초기화 등
    }

    public void MarkUnitActed(PlayerUnit unit)
    {
        Debug.Log($"[InputServiceNew] MarkUnitActed 호출: {unit.UnitName}");
        unit.MarkActed();
        if (_selectedUnit == unit)
        {
            Debug.Log($"[InputServiceNew] MarkUnitActed: 현재 선택 유닛 해제 {_selectedUnit.UnitName}");
            _selectedUnit = null;
        }
    }

    public bool AllPlayerActed()
    {
        bool result = _players != null && _players.All(p => p.HasActedThisTurn || p.IsDead);
        Debug.Log($"[InputServiceNew] AllPlayerActed? : {result}");
        return result;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var pos in clickPositions)
        {
            Gizmos.DrawSphere(pos, 0.25f); // 클릭 위치에 구
            Gizmos.DrawLine(pos + Vector3.left * 0.4f, pos + Vector3.right * 0.4f);
            Gizmos.DrawLine(pos + Vector3.up * 0.4f, pos + Vector3.down * 0.4f);
        }
    }
    
}
