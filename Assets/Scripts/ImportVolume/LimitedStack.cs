namespace ImportVolume
{
    public class LimitedStack<T>
    {
        private readonly T[] items;
        private int top = 0;

        public LimitedStack(int capacity)
        {
            items = new T[capacity];
        }

        public void Push(T item)
        {
            items[top] = item;
            top = (top + 1) % items.Length;
        }

        public T Pop()
        {
            top = (items.Length + top - 1) % items.Length;
            return items[top];
        }

        public int Count() {
            return items.Length;
        }
    }
}