class Pagina
{
    public int qnt_tuplas_ocup { private set; get; }
    private Tupla[] _tuples;

    public Pagina()
    {
        qnt_tuplas_ocup = 0;
        _tuples = new Tupla[10];
    }


    public Tupla GetTuple(int index)
    {
        if (index < 0 || index >= qnt_tuplas_ocup)
            throw new IndexOutOfRangeException();
        return _tuples[index];
    }

    public void SetTuple(int index, Tupla tuple)
    {
        if (index < 0 || index >= 10)
            throw new IndexOutOfRangeException();
        if (index < qnt_tuplas_ocup)
            _tuples[index] = tuple;
    }

    public bool AddTuple(Tupla tuple)
    {
        if (qnt_tuplas_ocup >= 10)
            return false; // Página cheia

        _tuples[qnt_tuplas_ocup] = tuple;
        qnt_tuplas_ocup++;
        return true;
    }
}