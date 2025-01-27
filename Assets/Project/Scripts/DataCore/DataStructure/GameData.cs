namespace Project.Scripts.DataCore.DataStructure
{
    public class GameData
    {
        public PlayerRelatedData playerRelatedData;
        
        public GameData()
        {
        }

        public GameData(PlayerRelatedData playerRelatedData)
        {
            this.playerRelatedData = playerRelatedData;
        }
    }
}