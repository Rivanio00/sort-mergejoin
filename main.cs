using System;

// VOCÊ DEVE INCLUIR SEUS NAMESPACES AQUI, CASO NECESSÁRIO!!!

class Program
{
    static void Main(string[] args)
    {
        Table vinho = new Table("vinho.csv"); // cria estrutura necessária para a Table
        Table uva = new Table("uva.csv");
        Table pais = new Table("pais.csv");

        vinho.CarregarDados(); // lê os dados do csv e adiciona na estrutura da Table, caso necessário
        uva.CarregarDados();
        pais.CarregarDados();

        // IMPLEMENTE O OperationERADOR E DEPOIS EXECUTE AQUI
        // Operationerador operation = new Operationerador(vinho, uva, "vinho_id", "uva_id");
        //// significa: SELECT * FROM Vinho V, Uva U WHERE V.vinho_id = U.uva_id
        //// IMPORTANTE: isso é só um exemplo, podem ser Tables/colunas distintas.
        //// genericamente: Operationerador(Table_1, Table_2, col_tab_1, col_tab_2):
        //// significa: SELECT * FROM Table_1, Table_2 WHERE col_tab_1 = col_tab_2

        operation.Executar(); // Realiza a Operationeração desejada

        Console.WriteLine($"#Pags: {operation.NumPagsGeradas()}"); // Retorna a quantidade de páginas geradas pela Operationeração
        Console.WriteLine($"#IOs: {operation.NumIOExecutados()}"); // Retorna a quantidade de IOs geradas pela Operationeração
        Console.WriteLine($"#Tups: {operation.NumTuplasGeradas()}"); // Retorna a quantidade de tuplas geradas pela Operationeração

        operation.SalvarTuplasGeradas("selecao_vinho_ano_colheita_1990.csv"); // Retorna as tuplas geradas pela Operationeração e salva em um csv
    }
}