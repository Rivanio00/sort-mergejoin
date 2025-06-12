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
    private int ios_sort_tabelaE { get; set; }
    private int ios_sort_tabelaD { get; set; }
    private int ios_merge_tabela { get; set; }
    private int num_pag_geradas { get; set; }
    private Tabela tabela_mergeada { get; set; }

    public Operador(Tabela Tabela1, Tabela Tabela2, String field1, String field2)
    {
        TabelaE = Tabela1;
        _fieldE = field1;
        TabelaD = Tabela2;
        _fieldD = field2;
        ios_sort_tabelaE = 0;
        ios_sort_tabelaD = 0;
        ios_merge_tabela = 0;
        num_pag_geradas = 0;
        tabela_mergeada = new Tabela($"{Tabela1}_{Tabela2}_{field1}_{field2}", Tabela1.QntCols+Tabela2.QntCols);
    }

    public void Executar()
    {
        ios_sort_tabelaE = TabelaE.SortTable(_fieldE);
        ios_sort_tabelaD = TabelaD.SortTable(_fieldD);
        ios_merge_tabela = Merge_Tabelas();
    }
    public int NumPagsGeradas()
    {
        return tabela_mergeada.QntPags;
    }
    public int NumIOExecutados()
    {
        return ios_sort_tabelaD + ios_sort_tabelaE + ios_merge_tabela;
    }
    public int NumTuplasGeradas()
    {
        int qntPags = tabela_mergeada.QntPags;
        Pagina ultimaPagina = Arquivos.ReadTxtPage($"disk/merge_{TabelaE}_{TabelaD}/pag-{qntPags - 1}");
        int qntTuplasUltimaPag = ultimaPagina.GetNumTuplas();
        int qntTuplas = (tabela_mergeada.QntPags - 1) * 10 + qntTuplasUltimaPag;
        return qntTuplas;
    }
    public void SalvarTuplasGeradas(String csvName)
    {
        //salvar a tabela final em um arquivo csv
    }

    public void addIO()
    {
        ios_merge_tabela += 1;
    }

    public int Merge_Tabelas()
    {
        Pagina paginaE = Arquivos.ReadTxtPage($"disk/{TabelaE}/{TabelaE}_{_fieldE}_intercalado_final/pag-0");
        addIO();
        Pagina paginaD = Arquivos.ReadTxtPage($"disk/{TabelaD}/{TabelaD}_{_fieldD}_intercalado_final/pag-0");
        addIO();
        int indexE = 0;
        int indexD = 0;
        Pagina paginaSaida = new Pagina();

        var schemaE = Schemas.Tabelas[TabelaE.getNomeTabela()];
        string[] colunasE = schemaE.colunas;
        int indexColunaE = Array.IndexOf(colunasE, _fieldE);

        var schemaD = Schemas.Tabelas[TabelaD.getNomeTabela()];
        string[] colunasD = schemaD.colunas;
        int indexColunaD = Array.IndexOf(colunasD, _fieldD);
        tabela_mergeada.QntPags = 1;

        while (paginaE != null && paginaD != null)
        {
            Tupla tupleE = paginaE.GetTuple(indexE % 10);
            Tupla tupleD = paginaD.GetTuple(indexD % 10);
            if (tupleE.Cols[indexColunaE] == tupleD.Cols[indexColunaD])
            {
                // Junta os valores das duas tuplas em um novo array de strings
                string linhaJoin = string.Join(",", tupleE.Cols.Concat(tupleD.Cols));

                // Cria a nova tupla a partir da linha concatenada
                Tupla novaTupla = new Tupla(linhaJoin);

                // Adiciona na página de saída
                if (!paginaSaida.AddTuple(novaTupla))
                {
                    String caminhoOutput = $"disk/merge_{TabelaE}_{TabelaD}/pag-{tabela_mergeada.QntPags - 1}";
                    Arquivos.WriteTxtPage(paginaSaida, caminhoOutput);
                    addIO();
                    paginaSaida = new Pagina();
                    paginaSaida.AddTuple(novaTupla);
                    tabela_mergeada.QntPags += 1;
                }
                // Ando com a página da direita
                indexD += 1;
                if (indexD > (TabelaD.QntPags * 10) - 1)
                {
                    int pagAtual = (indexD + 9) / 10;
                    paginaD = Arquivos.ReadTxtPage($"disk/{TabelaD}/{TabelaD}_{_fieldD}_intercalado_final/pag-{pagAtual}");
                }
                if ((indexD % 10) > paginaD.GetNumTuplas())
                {

                }
                //paginaE = Arquivos.ReadTxtPage($"disk/{TabelaE}/{TabelaE}_{_fieldE}_intercalado_final/pag-0");
            }
        }
        return 1;
    }
}