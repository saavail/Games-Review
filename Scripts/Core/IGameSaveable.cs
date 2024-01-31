namespace Core
{
    public interface IGameSaveable
    {
        public void OnSave(string prefixKey);
    }
}