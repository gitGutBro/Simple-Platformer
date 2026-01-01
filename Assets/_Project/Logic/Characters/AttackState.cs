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

    public override async UniTask Enter()
    {
        CancellationTokenSource = new CancellationTokenSource();

        try
        {
            while (_targetDetector.IsTargetNear() && CancellationTokenSource.IsCancellationRequested is false)
            {
                if (_onCooldown.Invoke() is false)
                    await _attack(CancellationTokenSource.Token);
                else
                    await UniTask.Yield(PlayerLoopTiming.Update, CancellationTokenSource.Token);

                if (_targetDetector.IsTargetNear() is false)
                {
                    if (_targetDetector.IsTargetHit())
                        await StateMachine.Enter<ChaseState>();
                    else
                        await StateMachine.Enter<PatrolState>();

                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        if (_targetDetector.IsTargetLost())
            await StateMachine.Enter<PatrolState>();
        else if (_targetDetector.IsTargetHit())
            await StateMachine.Enter<ChaseState>();
    }

    public override UniTask Exit()
    {
        DisposeToken();
        return UniTask.CompletedTask;
    }
}