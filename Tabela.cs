using System.Collections.Generic;
using System.Runtime.InteropServices;

class Tabela
{
    private List<Pagina> _pags;
    public int QntCols { get; private set; }
    public int QntPags = 0;
    private String _csvPath;

    public Tabela(String csvPath)
    {
        _csvPath = csvPath;
        String[] lines = Arquivos.ReadCsvLines(csvPath);
        QntCols = Arquivos.ParseLine(lines[0]).Length;
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
        String tableName = _csvPath.Split('.')[0];
        Directory.CreateDirectory($"disk/{tableName}");
        String[] lines = Arquivos.ReadCsvLines(_csvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            Arquivos.WriteTxtLine($"disk/{tableName}/pag-{tableName}-{QntPags}.txt", lines[i]);
            if (i % 10 == 0) QntPags += 1;
        }
    }
}