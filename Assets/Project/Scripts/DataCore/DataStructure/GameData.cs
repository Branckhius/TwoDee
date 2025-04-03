namespace Project.Scripts.DataCore.DataStructure
{
    public class GameData
    {
        public PlayerRelatedData playerRelatedData;
        
        public GameData()
        {
            playerRelatedData = new PlayerRelatedData();
        }

        public GameData(PlayerRelatedData playerRelatedData)
        {
            this.playerRelatedData = playerRelatedData;
        }
    }
}