namespace sort_mergejoin;

public class Arquivos
{
    public static String[] ReadCsvLines(String csvPath)
    {
        return File.ReadAllLines(csvPath);
    }

    public static String[] ParseLine(String line) // Extrai dados separados por vírgulas
    {
        return line.Split(',');
    }

    public static void WriteTxtLine(String txtPath, String line, bool canAppend = true)
    {
        using (StreamWriter writer = new StreamWriter(txtPath, append: canAppend))
        {
            writer.WriteLine(line);
        }
    }

    public static Pagina ReadTxtPage(string txtPath)
    {
        Pagina new_page = new Pagina();

        using (StreamReader reader = new StreamReader(txtPath))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                Tupla tupla = new Tupla(line);
                new_page.AddTuple(tupla);
            }
        }

        return new_page;
    }

    public static void WriteTxtPage(Pagina nova_pagina, string txtPath)
    {
        DeleteFileIfExists(txtPath);
        for (int i = 0; i < nova_pagina.GetNumTuplas(); i++)
        {
            Arquivos.WriteTxtLine(txtPath, nova_pagina.GetTuple(i).ToString());
        }
    }
    
    public static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}