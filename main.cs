using System;
using System.Reflection;
// VOCÊ DEVE INCLUIR SEUS NAMESPACES AQUI, CASO NECESSÁRIO!!!
namespace sort_mergejoin;
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

        // IMPLEMENTE O OPERADOR E DEPOIS EXECUTE AQUI
        Operador op = new Operador(pais,uva,"pais_id" , "pais_origem_id");

        /*pais.SortTable("nome");
        pais.SortTable("sigla");

        uva.SortTable("ano_colheita");
        uva.SortTable("nome");
        uva.SortTable("tipo");
        uva.SortTable("pais_origem_id");

        vinho.SortTable("rotulo");
        vinho.SortTable("ano_producao");
        vinho.SortTable("uva_id");
        vinho.SortTable("pais_producao_id");*/

        //significa: SELECT * FROM Vinho V, Uva U WHERE V.vinho_id = U.uva_id
        //IMPORTANTE: isso é só um exemplo, podem ser Tabelas/colunas distintas.

        //genericamente: Operador(Tabela_1, Tabela_2, col_tab_1, col_tab_2):
        //significa: SELECT * FROM Tabela_1, Tabela_2 WHERE col_tab_1 = col_tab_2

        op.Executar(); // Realiza a Operação desejada

        //Console.WriteLine($"#Pags: {op.NumPagsGeradas()}"); // Retorna a quantidade de páginas geradas pela Operação
        Console.WriteLine($"#IOs: {op.NumIOExecutados()}"); // Retorna a quantidade de IOs geradas pela Operação
        //Console.WriteLine($"#Tups: {op.NumTuplasGeradas()}"); // Retorna a quantidade de tuplas geradas pela Operação

        //op.SalvarTuplasGeradas("selecao_vinho_ano_colheita_1990.csv"); // Retorna as tuplas geradas pela Operação e salva em um csv*/
    }
}