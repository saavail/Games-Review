namespace UISystem
{
    public interface IBackendWaiter
    {
        public void Show();
        public void ShowException();
        public void Hide();
        public void ForceHide();
    }
}