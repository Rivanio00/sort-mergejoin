class Tuple
{
    private string[] values;

    public int Size { get; private set; }


    public Tuple(int size)
    {
        Size = size;
        values = new string[size];
    }

    // Indexador para acessar os valores como um array
    public string this[int index]
    {
        get
        {
            if (index < 0 || index >= Size)
                throw new IndexOutOfRangeException();
            return values[index];
        }
        set
        {
            if (index < 0 || index >= Size)
                throw new IndexOutOfRangeException();
            values[index] = value;
        }
    }

    // Método opcional para exibir a tupla como string
    public override string ToString()
    {
        return string.Join("|", values);
    }
}
