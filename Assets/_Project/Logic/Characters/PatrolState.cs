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

    public async override UniTask Enter()
    {
        CancellationTokenSource = new CancellationTokenSource();

        _patrolling(CancellationTokenSource.Token).Forget();

        await UniTask.WaitUntil(_targetHit, cancellationToken: CancellationTokenSource.Token);

        await StateMachine.Enter<ChaseState>();
    }

    public override UniTask Exit()
    {
        DisposeToken();
        return UniTask.CompletedTask;
    }
}