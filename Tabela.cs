using System.Collections.Generic;
using System.Runtime.InteropServices;

class Tabela
{
    private List<Pagina> _pags;
    public int QntCols { get; private set; }
    public int QntPags => _pags.Count;
    private String _csvPath;

    public Tabela(String csvPath)
    {
        _csvPath = csvPath;
    }

    public void AddPage(Pagina page)
    {
        _pags.Add(page);
    }

    public Pagina GetPage(int index)
    {
        if (index < 0 || index >= _pags.Count)
            throw new IndexOutOfRangeException();
        return _pags[index];
    }

    public void CarregarDados()
    {
        //
    }
}