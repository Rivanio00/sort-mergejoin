/*
        operation.Executar(); // Realiza a Operationeração desejada

        Console.WriteLine($"#Pags: {operation.NumPagsGeradas()}"); // Retorna a quantidade de páginas geradas pela Operationeração
        Console.WriteLine($"#IOs: {operation.NumIOExecutados()}"); // Retorna a quantidade de IOs geradas pela Operationeração
        Console.WriteLine($"#Tups: {operation.NumTuplasGeradas()}"); // Retorna a quantidade de tuplas geradas pela Operationeração
        */
        using sort_mergejoin;
public class Operador
{
    private Tabela TabelaE { get; set; }
    private Tabela TabelaD { get; set; }
    private String _fieldE;
    private String _fieldD;
    public Operador(Tabela Tabela1, Tabela Tabela2, String field1, String field2)
    {
        TabelaE = Tabela1;
        _fieldE = field1;
        TabelaD = Tabela2;
        _fieldD = field2;
    }

    public void Executar()
    {
        //ordenar as duas tabelas
        Tabela TabelaE_ordenada = TabelaE.SortTable(_fieldE);
        Tabela TabelaD_ordenada = TabelaD.SortTable(_fieldD);
        //mergear as tabelas ordenadas
        //OBS=contar a quantidade de IOs durante a operação
    }
    public int NumPagsGeradas()
    {
        //retorna o número de páginas da tabela final
        //basta aessar os metadados dessa tabela
        return 0;
    }
    public int NumIOExecutados()
    {
        //retorna o número de IOs feitas durante a operação
        //durante a execução do método .Executar() iremos computar o número de acessos ao disco, podemos salvar isso num atributo da classe Operador
        //basta retornar o valor desse atributo
        return 0;
    }
    public int NumTuplasGeradas()
    {
        //retorna a quantidade de tuplas da tabela final
        //basta acessar os metadados dessa tabela
        return 0;
    }
    public void SalvarTuplasGeradas(String csvName)
    {
        //salvar a tabela final em um arquivo csv
    }
}