using System;

// VOCÊ DEVE INCLUIR SEUS NAMESPACES AQUI, CASO NECESSÁRIO!!!

class Program
{
    static void Main(string[] args)
    {
        Tabela vinho = new Tabela("vinho.csv"); // cria estrutura necessária para a Tabela
        Tabela uva = new Tabela("uva.csv");
        Tabela pais = new Tabela("pais.csv");

        vinho.CarregarDados(); // lê os dados do csv e adiciona na estrutura da Tabela, caso necessário
        uva.CarregarDados();
        pais.CarregarDados();

        /* IMPLEMENTE O OperationERADOR E DEPOIS EXECUTE AQUI
        Operationerador operation = new Operationerador(vinho, uva, "vinho_id", "uva_id");
        significa: SELECT * FROM Vinho V, Uva U WHERE V.vinho_id = U.uva_id
        IMPORTANTE: isso é só um exemplo, podem ser Tabelas/colunas distintas.
        genericamente: Operationerador(Tabela_1, Tabela_2, col_tab_1, col_tab_2):
        significa: SELECT * FROM Tabela_1, Tabela_2 WHERE col_tab_1 = col_tab_2 */

        Operador op = new Operador(vinho, uva, "vinho_id", "uva_id");
        
        op.Executar(); // Realiza a Operationeração desejada

        Console.WriteLine($"#Pags: {op.NumPagsGeradas()}"); // Retorna a quantidade de páginas geradas pela Operationeração
        Console.WriteLine($"#IOs: {op.NumIOExecutados()}"); // Retorna a quantidade de IOs geradas pela Operationeração
        Console.WriteLine($"#Tups: {op.NumTuplasGeradas()}"); // Retorna a quantidade de tuplas geradas pela Operationeração

        op.SalvarTuplasGeradas("selecao_vinho_ano_colheita_1990.csv"); // Retorna as tuplas geradas pela Operationeração e salva em um csv
    }
}