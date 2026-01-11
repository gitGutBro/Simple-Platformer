using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Project.Logic.Characters
{
    internal interface IEnemyAttacker
    {
        void Init(IAttackAnimator animator, Func<bool> grounded = null);
        UniTask TryAttack(CancellationToken cancellationToken = default);
    }
}