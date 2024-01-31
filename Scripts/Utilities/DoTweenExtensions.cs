using DG.Tweening;

namespace Utilities
{
    public static class DoTweenExtensions
    {
        public static Tweener Wait(float duration)
            => DOTween.To(_ => { }, float.MinValue, float.MaxValue, duration);
    }
}