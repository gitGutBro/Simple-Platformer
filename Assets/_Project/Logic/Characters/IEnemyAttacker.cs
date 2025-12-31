using Cysharp.Threading.Tasks;

namespace _Project.Logic.Characters
{
    internal interface IEnemyAttacker
    {
        void Init(IAttackAnimator animator);
        UniTask TryAttack();
    }
}