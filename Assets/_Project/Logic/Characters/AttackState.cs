using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Project.Logic.Characters;

internal sealed class AttackState : BaseState
{
    private readonly TargetDetector _targetDetector;
    private readonly Func<bool> _onCooldown;
    private readonly Func<CancellationToken, UniTask> _attack;

    public AttackState(Func<CancellationToken, UniTask> attack, Func<bool> onCooldown, TargetDetector targetDetector)
    {
        _attack = attack;
        _onCooldown = onCooldown;
        _targetDetector = targetDetector;
    }

    protected override async UniTask EnterCore(CancellationToken cancellationToken)
    {
        try
        {
            while (_targetDetector.IsTargetNear() && cancellationToken.IsCancellationRequested is false)
            {
                if (_onCooldown.Invoke() is false)
                    await _attack(cancellationToken);
                else
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

                if (_targetDetector.IsTargetNear() is false)
                {
                    if (_targetDetector.IsTargetHit())
                        await StateChanger.Enter<ChaseState>();
                    else
                        await StateChanger.Enter<PatrolState>();

                    return;
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        if (_targetDetector.IsTargetLost())
            await StateChanger.Enter<PatrolState>();
        else if (_targetDetector.IsTargetHit())
            await StateChanger.Enter<ChaseState>();
    }
}