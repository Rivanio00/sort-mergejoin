class Tupla(string[] cols)
{
    public string[] Cols { get; private set; } = cols;

    public string this[int index]
    {
        get { return Cols[index]; }
        set { Cols[index] = value; }
    }

    public override string ToString()
    {
        return string.Join(",", Cols);
    }
}
