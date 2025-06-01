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

    public String[] GetMetadata(String tableName)
    {
        Directory.CreateDirectory($"disk/{tableName}");
        String metaPath = $"disk/{tableName}/meta-{tableName}.txt";
        String[] lines = File.Exists(metaPath) ? File.ReadAllLines(metaPath) : [""];
        if (lines.Length == 0 || lines[0] == "")
        {
            Arquivos.WriteTxtLine(metaPath, "0", false);
            return File.ReadAllLines(metaPath);
        }
        return lines;
    }

    public void SetMetadata(String tableName, int qntTuplas = -1) // 0 = qntTuplas
    {
        String[] fields = GetMetadata(tableName);
        if (qntTuplas != -1)
        {
            fields[0] = qntTuplas.ToString();
        }
        Arquivos.WriteTxtLine($"disk/{tableName}/meta-{tableName}.txt", fields[0], false);
    }

    public void CarregarDados()
    {
        String tableName = _csvPath.Split('.')[0];
        int qntTuplas = int.Parse(GetMetadata(tableName)[0]);
        String[] lines = Arquivos.ReadCsvLines(_csvPath);
        int i = qntTuplas+1;
        for (; i < lines.Length; i++) // Pois a primeira linha é a estrutura da tupla
        {
            Arquivos.WriteTxtLine($"disk/{tableName}/pag-{tableName}-{QntPags}.txt", lines[i]);
            if (i % 10 == 0) QntPags += 1;
        }
        SetMetadata(tableName, i-1);
    }
}