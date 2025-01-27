using Project.Scripts.DataCore.DataStructure;

namespace Project.Scripts.DataCore
{
    public interface ILocalDataStorage
    {
        void Store(GameData gameData);
        void Clear();
        bool Has();
        public GameData Fetch();
    }
}