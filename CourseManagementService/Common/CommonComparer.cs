using System.Diagnostics.CodeAnalysis;

namespace CourseManagementService.Common
{
    public class CommonComparer<TObject, TKey>(Func<TObject, TKey> keySelector, IEqualityComparer<TKey> keyComparer = null, TKey defaultKey = default) : IEqualityComparer<TObject>
    {
        private readonly Func<TObject, TKey> _keySelector = keySelector;
        private readonly IEqualityComparer<TKey> _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        private readonly TKey _defaultKey = defaultKey;

        public bool Equals(TObject x, TObject y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (ReferenceEquals(x, y))
            {
                return true;
            }

            // Lấy giá trị key của x và y
            var keyX = _keySelector(x);
            var keyY = _keySelector(y);

            // Nếu cả 2 key là giá trị mặc định thì coi như không bằng nhau
            if (_keyComparer.Equals(keyX, _defaultKey) && _keyComparer.Equals(keyY, _defaultKey))
            {
                return false;
            }

            // Không dùng keyX.Equals(keyY) vì có thể sử dụng cách so sánh khác nhau
            // ví dụ key là string thì có thể so sánh case sensitive hoặc không
            // cụ thể StringComparison.OrdinalIgnoreCase.Equals(keyX, keyY)
            return _keyComparer.Equals(keyX, keyY);
        }

        public int GetHashCode([DisallowNull] TObject obj)
        {
            if (obj == null)
            {
                return 0;
            }

            var key = _keySelector(obj);
            return _keyComparer.Equals(key, _defaultKey) ? 0 : _keyComparer.GetHashCode(key);
        }
    }
}