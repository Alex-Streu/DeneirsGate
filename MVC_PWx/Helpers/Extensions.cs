

namespace System.Collections.Generic
{
    public static class ListExtensions
    {
        public static void Move<T>(this List<T> list, T item, int index)
        {
            list.Remove(item);
            list.Insert(index, item);
        }

        public static void MoveToTop<T>(this List<T> list, T item)
        {
            list.Move(item, 0);
        }
    }
}

namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }
    }
}