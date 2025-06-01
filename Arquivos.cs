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

    public static void WriteTxtLine(String txtPath, String line)
    {
        using (StreamWriter writer = new StreamWriter(txtPath, append: true))
        {
            writer.WriteLine(line);
        }
    }
}