using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace LWFlo.States
{
    public class GameStateGameplayOrResume : GameStateBase<GameStateGameplayOrResume.Context>
    {
        public class Context {}
        
        //por sa spawnez ui-ul
        protected override UniTask OnRun(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}