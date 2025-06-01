/*
        operation.Executar(); // Realiza a Operationeração desejada

        Console.WriteLine($"#Pags: {operation.NumPagsGeradas()}"); // Retorna a quantidade de páginas geradas pela Operationeração
        Console.WriteLine($"#IOs: {operation.NumIOExecutados()}"); // Retorna a quantidade de IOs geradas pela Operationeração
        Console.WriteLine($"#Tups: {operation.NumTuplasGeradas()}"); // Retorna a quantidade de tuplas geradas pela Operationeração
        */

class Operador
{
    public Operador(Tabela Tabela1, Tabela Tabela2, String field1, String field2) { }

    public void Executar() { }
    public int NumPagsGeradas() { return 0; }
    public int NumIOExecutados() { return 0; }
    public int NumTuplasGeradas() { return 0; }
    public void SalvarTuplasGeradas(String csvName) {}
}