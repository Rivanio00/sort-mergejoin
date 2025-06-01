class Arquivos
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
}