using System.Threading;
using Cysharp.Threading.Tasks;
using _Project.Logic.Characters;

internal sealed class ChaseState : BaseState
{
    private readonly IChaseContext _chaseContext;

    public ChaseState(IChaseContext chaseContext) => 
        _chaseContext = chaseContext;

    protected override async UniTask EnterCore(CancellationToken cancellationToken)
    {
        if (_chaseContext.TargetDetector.IsTargetHit() is false)
        {
            DisposeToken();
            await StateChanger.Enter<PatrolState>();
            return;
        }

        _chaseContext.MoverX.MoveToTarget(_chaseContext.TargetDetector.GetTargetTransfrom(), cancellationToken).Forget();

        UniTask waitLost = UniTask.WaitUntil(() => _chaseContext.TargetDetector.IsTargetLost(), cancellationToken: cancellationToken);
        UniTask waitNear = UniTask.WaitUntil(() => _chaseContext.TargetDetector.IsTargetNear(), cancellationToken: cancellationToken);

        await UniTask.WhenAny(waitLost, waitNear);

        DisposeToken();

        if (_chaseContext.TargetDetector.IsTargetLost())
        {
            await StateChanger.Enter<PatrolState>();
        }
        else
        {
            _chaseContext.MoverX.Stop();
            await StateChanger.Enter<AttackState>();
        }
    }
}