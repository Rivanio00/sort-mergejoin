using System;
using System.Reflection;
// VOCÊ DEVE INCLUIR SEUS NAMESPACES AQUI, CASO NECESSÁRIO!!!
namespace sort_mergejoin;
class Program
{
    private static void cleanDisk()
    {
        if (Directory.Exists("disk"))
        {
            Directory.Delete("disk", true);
        }
    }

    static void Main(string[] args)
    {
        cleanDisk();
        Tabela vinho = new Tabela("vinho.csv"); // cria estrutura necessária para a Tabela
        Tabela uva = new Tabela("uva.csv");
        Tabela pais = new Tabela("pais.csv");

        vinho.CarregarDados(); // lê os dados do csv e adiciona na estrutura da Tabela, caso necessário
        uva.CarregarDados();
        pais.CarregarDados();

        // BATERIA DE TESTES - APENAS COMPARAÇÕES QUE FAZEM SENTIDO
        Console.WriteLine("=== BATERIA DE TESTES - COMPARAÇÕES VÁLIDAS ===\n");

        Operador op;
        int contadorTestes = 1;

        // =========================================================================
        // TESTE 1: VINHO x UVA - Chave estrangeira válida
        // vinho.uva_id = uva.uva_id (JOIN natural por chave estrangeira)
        // =========================================================================
        Console.WriteLine("--- TESTE 1: JOIN Natural VINHO x UVA ---");
        op = new Operador(vinho, uva, "uva_id", "uva_id");
        Console.WriteLine($"Teste {contadorTestes++}: vinho.uva_id = uva.uva_id (Chave estrangeira)");
        op.Executar();
        Console.WriteLine($"#Pags: {op.NumPagsGeradas()} | #IOs: {op.NumIOExecutados()} | #Tups: {op.NumTuplasGeradas()}");
        op.SalvarTuplasGeradas($"join_vinho_uva_chave_estrangeira.csv");

        // =========================================================================
        // TESTE 2: VINHO x PAIS - Chave estrangeira válida  
        // vinho.pais_producao_id = pais.pais_id (JOIN natural por chave estrangeira)
        // =========================================================================
        Console.WriteLine("\n--- TESTE 2: JOIN Natural VINHO x PAIS ---");
        op = new Operador(vinho, pais, "pais_producao_id", "pais_id");
        Console.WriteLine($"Teste {contadorTestes++}: vinho.pais_producao_id = pais.pais_id (Chave estrangeira)");
        op.Executar();
        Console.WriteLine($"#Pags: {op.NumPagsGeradas()} | #IOs: {op.NumIOExecutados()} | #Tups: {op.NumTuplasGeradas()}");
        op.SalvarTuplasGeradas($"join_vinho_pais_chave_estrangeira.csv");

        // =========================================================================
        // TESTE 3: UVA x PAIS - Chave estrangeira válida
        // uva.pais_origem_id = pais.pais_id (JOIN natural por chave estrangeira)
        // =========================================================================
        Console.WriteLine("\n--- TESTE 3: JOIN Natural UVA x PAIS ---");
        op = new Operador(uva, pais, "pais_origem_id", "pais_id");
        Console.WriteLine($"Teste {contadorTestes++}: uva.pais_origem_id = pais.pais_id (Chave estrangeira)");
        op.Executar();
        Console.WriteLine($"#Pags: {op.NumPagsGeradas()} | #IOs: {op.NumIOExecutados()} | #Tups: {op.NumTuplasGeradas()}");
        op.SalvarTuplasGeradas($"join_uva_pais_chave_estrangeira.csv");

        // =========================================================================
        // TESTE 4: Comparação por ano (mesmo tipo de dado - inteiro)
        // vinho.ano_producao = uva.ano_colheita (Comparação temporal válida)
        // =========================================================================
        Console.WriteLine("\n--- TESTE 4: JOIN por Ano (Comparação Temporal) ---");
        op = new Operador(vinho, uva, "ano_producao", "ano_colheita");
        Console.WriteLine($"Teste {contadorTestes++}: vinho.ano_producao = uva.ano_colheita (Mesmo tipo: ano)");
        op.Executar();
        Console.WriteLine($"#Pags: {op.NumPagsGeradas()} | #IOs: {op.NumIOExecutados()} | #Tups: {op.NumTuplasGeradas()}");
        op.SalvarTuplasGeradas($"join_vinho_uva_por_ano.csv");

        Console.WriteLine($"\n=== BATERIA DE TESTES OTIMIZADA CONCLUÍDA ===");
        Console.WriteLine($"Total de testes executados: {contadorTestes - 1}");
        Console.WriteLine("Apenas comparações logicamente válidas foram mantidas:");
        Console.WriteLine("- 3 JOINs por chave estrangeira (relacionamentos naturais)");
        Console.WriteLine("- 1 JOIN por comparação de tipo compatível (anos)");

    }
}