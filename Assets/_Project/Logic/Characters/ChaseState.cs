using System.Threading;
using Cysharp.Threading.Tasks;
using _Project.Logic.Characters;
using _Project.Logic.Infrastructure.StateMachine;

internal sealed class ChaseState : BaseState
{
    private readonly IChaseContext _chaseContext;

    public ChaseState(IChaseContext chaseContext) => 
        _chaseContext = chaseContext;

    public override async UniTask Enter()
    {
        CancellationTokenSource = new CancellationTokenSource();

        if (_chaseContext.TargetDetector.IsTargetHit() is false)
        {
            DisposeToken();
            await StateMachine.Enter<PatrolState>();
            return;
        }

        _chaseContext.MoverX.MoveToTarget(_chaseContext.TargetDetector.GetTargetTransfrom(), CancellationTokenSource.Token).Forget();

        UniTask waitLost = UniTask.WaitUntil(() => _chaseContext.TargetDetector.IsTargetLost(), cancellationToken: CancellationTokenSource.Token);
        UniTask waitNear = UniTask.WaitUntil(() => _chaseContext.TargetDetector.IsTargetNear(), cancellationToken: CancellationTokenSource.Token);

        await UniTask.WhenAny(waitLost, waitNear);

        DisposeToken();

        if (_chaseContext.TargetDetector.IsTargetLost())
        {
            await StateMachine.Enter<PatrolState>();
        }
        else
        {
            _chaseContext.MoverX.Stop();
            await StateMachine.Enter<AttackState>();
        }
    }
}