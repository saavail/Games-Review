namespace Core
{
    public interface IScoreChangeable
    {
        public void OnScoreChanged(int score, int maxScore);
    }
}