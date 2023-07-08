using Unity.Burst;

[BurstCompile]
public unsafe struct CustomFixedList16<T> where T : unmanaged
{
    private readonly T m_Element0;
    private readonly T m_Element1;
    private readonly T m_Element2;
    private readonly T m_Element3;
    private readonly T m_Element4;
    private readonly T m_Element5;
    private readonly T m_Element6;
    private readonly T m_Element7;
    private readonly T m_Element8;
    private readonly T m_Element9;
    private readonly T m_Element10;
    private readonly T m_Element11;
    private readonly T m_Element12;
    private readonly T m_Element13;
    private readonly T m_Element14;
    private readonly T m_Element15;
    

    public ref T this[int index]
    {
        get
        {
            return ref GetElement(index);
        }
    }

    private ref T GetElement(int index)
    {
        fixed (T* elements = &this.m_Element0)
        {
            return ref elements[index];
        }
    }

    private void SetElement(int index, T value)
    {
        fixed (T* elements = &this.m_Element0)
        {
            elements[index] = value;
        }
    }

    public int Count { get; private set; }

    public const int Capacity = 16;

    public void Add(T item)
    {
        SetElement(this.Count, item);
        this.Count++;
    }

    public void Clear()
    {
        for (int i = 0; i < this.Count; ++i)
        {
            SetElement(i, default);
        }
        this.Count = 0;
    }

    public void Insert(int index, T value)
    {
        if (this.Count >= Capacity)
        {
            return;
        }
        for (int i = this.Count; i > index; --i)
        {
            SetElement(i, GetElement(i - 1));
        }
        SetElement(index, value);
        this.Count++;
    }

    public void RemoveAt(int index)
    {
        if (this.Count == 0)
        {
            return;
        }
        for (int i = index; i < this.Count - 1; ++i)
        {
            SetElement(i, GetElement(i + 1));
        }
        this.Count--;
    }

    public void RemoveRange(int index, int count)
    {

        if (count < 0)
        {
            return;
        }

        int indexAfter = index + count;
        int indexEndCopy = indexAfter + count;
        if (indexEndCopy >= this.Count)
        {
            indexEndCopy = this.Count;
        }

        int numCopies = indexEndCopy - indexAfter;
        for (int i = 0; i < numCopies; ++i)
        {
            SetElement(index + i, GetElement(index + count + i));
        }

        for (int i = indexAfter; i < this.Count - 1; ++i)
        {
            SetElement(i, GetElement(i + 1));
        }

        this.Count -= count;
    }
}
