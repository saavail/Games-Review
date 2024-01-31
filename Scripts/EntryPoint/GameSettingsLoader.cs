namespace EntryPoint
{
    public class GameSettingsLoader : AsyncInitializableAndLoad<GameSettingsData>
    {
        protected override string LoadPath => nameof(GameSettingsData);

        public GameSettingsLoader(IResourceLoader resourceLoader)
            : base(resourceLoader) { }
    }
}