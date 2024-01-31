namespace Games
{
    public interface ITableMapEvents
    {
        public void OnPush(int x, int y, int value);
        public void OnMove(int x1, int y1, int x2, int y2);
        public void OnMerge(int x1, int y1, int x2, int y2, int value);
        public void OnDelete(int x, int y);
    }
}