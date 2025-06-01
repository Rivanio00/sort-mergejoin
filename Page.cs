class Page
{
    public int num_tuple { private set; get; }

    private Tuple[] tuples = new Tuple[10];

    public Page(int num)
    {
        num_tuple = 0;
    }


    public Tuple GetTuple(int index)
    {
        if (index < 0 || index >= num_tuple)
            throw new IndexOutOfRangeException();
        return tuples[index];
    }

    public void SetTuple(int index, Tuple tuple)
    {
        if (index < 0 || index >= 10)
            throw new IndexOutOfRangeException();
        if (index < num_tuple)
            tuples[index] = tuple;
    }

    public bool AddTuple(Tuple tuple)
    {
        if (num_tuple >= 10)
            return false; // Página cheia

        tuples[num_tuple] = tuple;
        num_tuple++;
        return true;
    }
}