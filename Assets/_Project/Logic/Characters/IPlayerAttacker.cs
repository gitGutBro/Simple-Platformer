namespace _Project.Logic.Characters
{
    internal interface IPlayerAttacker
    {
        public void Init(IAttackAnimator animator);
        public void OnAttackPressed();
    }
}