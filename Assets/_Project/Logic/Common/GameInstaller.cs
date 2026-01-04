using UnityEngine;
using Zenject;
using _Project.Logic.Characters;
using _Project.Logic.InputSystem;

internal class GameInstaller : MonoInstaller
{
    [SerializeField] private Player _playerInScene;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameplayNewInputSystem>().AsSingle().NonLazy();
        Container.Bind<Wallet>().AsTransient();
    }
}