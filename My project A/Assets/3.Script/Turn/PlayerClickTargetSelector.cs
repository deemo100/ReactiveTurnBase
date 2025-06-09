using System.Collections.Generic;
using System.Threading.Tasks; 
using UnityEngine;

public interface ITargetSelector
{
    Task<Unit> SelectTargetAsync(IEnumerable<Unit> candidates);
}

public class PlayerClickTargetSelector : MonoBehaviour, ITargetSelector
{
    private TaskCompletionSource<Unit> _tcs;
    public Task<Unit> SelectTargetAsync(IEnumerable<Unit> candidates)
    {
        _tcs = new TaskCompletionSource<Unit>();
        Unit.OnUnitClicked += OnClick;
        return _tcs.Task;
    }

    private void OnClick(Unit u)
    {
        if (_tcs != null)
        {
            _tcs.TrySetResult(u);
            Unit.OnUnitClicked -= OnClick;
            _tcs = null;
        }
    }
}