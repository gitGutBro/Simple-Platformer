using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using _Project.Logic.Characters;

internal sealed class PatrolState : BaseState
{
    private readonly Func<bool> _targetHit;
    private readonly Func<CancellationToken, UniTaskVoid> _patrolling;

    public PatrolState(Func<bool> targetHit, Func<CancellationToken, UniTaskVoid> patrolling)
    {
        _targetHit = targetHit;
        _patrolling = patrolling;
    }

    protected override async UniTask EnterCore(CancellationToken cancellationToken)
    {
        _patrolling(cancellationToken).Forget();

        await UniTask.WaitUntil(_targetHit, cancellationToken: cancellationToken);

        await StateChanger.Enter<ChaseState>();
    }
}