using System.Threading;
using Cysharp.Threading.Tasks;

namespace Project.Scripts.Game.GameManager
{
    public interface IGameManager
    {
        UniTask StartWithState<TState, TContext>(TContext ctx, CancellationToken cancellationToken)
            where TContext : class
            where TState : GameStateBase<TContext>;
    }
}