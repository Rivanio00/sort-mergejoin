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
        // Escolhendo a tabela menor como externa
        int min = Math.Min(Tabela1.QntPags, Tabela2.QntPags);
        if (Tabela1.QntPags == min)
        {
            TabelaE = Tabela1;
            _fieldE = field1;
            TabelaD = Tabela2;
            _fieldD = field2;
        }
        else
        {
            TabelaE = Tabela2;
            _fieldE = field2;
            TabelaD = Tabela1;
            _fieldD = field1;
        }

        ios_sort_tabelaE = 0;
        ios_sort_tabelaD = 0;
        ios_merge_tabela = 0;
        num_pag_geradas = 0;
        tabela_mergeada = new Tabela($"{Tabela1}_{Tabela2}_{field1}_{field2}", Tabela1.QntCols + Tabela2.QntCols);
    }

    public void Executar()
    {
        ios_sort_tabelaE = TabelaE.SortTable(_fieldE);
        ios_sort_tabelaD = TabelaD.SortTable(_fieldD);
        Merge_Tabelas();
    }
    public int NumPagsGeradas()
    {
        num_pag_geradas = tabela_mergeada.QntPags;
        return num_pag_geradas;
    }

    public int NumIOExecutados()
    {
        return ios_sort_tabelaD + ios_sort_tabelaE + ios_merge_tabela;
    }
    public int NumTuplasGeradas()
    {
        int qntPags = tabela_mergeada.QntPags;
        Pagina ultimaPagina = Arquivos.ReadTxtPage($"disk/merge_{TabelaE.getNomeTabela()}_{TabelaD.getNomeTabela()}/pag-{qntPags - 1}.txt");
        int qntTuplasUltimaPag = ultimaPagina.GetNumTuplas();
        int qntTuplas = (tabela_mergeada.QntPags - 1) * 10 + qntTuplasUltimaPag;
        return qntTuplas;
    }
    public void SalvarTuplasGeradas(String csvName)
    {
        Pagina page;
        Arquivos.DeleteFileIfExists(csvName);
        Arquivos.WriteTxtLine($"disk/{csvName}",$"{TabelaE.getColunas()},{TabelaD.getColunas()}");
        for (int i = 0; i < num_pag_geradas; i++)
        {
            String pagePath = $"disk/merge_{TabelaE.getNomeTabela()}_{TabelaD.getNomeTabela()}/pag-{i}.txt";
            page = Arquivos.ReadTxtPage(pagePath);
            for (int j = 0; j < page.GetNumTuplas(); j++)
            {
                Arquivos.WriteTxtLine($"disk/{csvName}", page.GetTuple(j).ToString());
            }
        }
    }

    public void addIO()
    {
        ios_merge_tabela += 1;
    }

    public void Merge_Tabelas()
{
    Pagina paginaE = Arquivos.ReadTxtPage($"disk/{TabelaE.getNomeTabela()}/{TabelaE.getNomeTabela()}_{_fieldE}_intercalado_final/pag-0.txt");
    addIO();
    Pagina paginaD = Arquivos.ReadTxtPage($"disk/{TabelaD.getNomeTabela()}/{TabelaD.getNomeTabela()}_{_fieldD}_intercalado_final/pag-0.txt");
    addIO();

    int indexE = 0;
    int indexD = 0;
    int pagAtualE = 0;
    int pagAtualD = 0;
    tabela_mergeada.QntPags = 1;

    Pagina paginaSaida = new Pagina();

    var schemaE = Schemas.Tabelas[TabelaE.getNomeTabela()];
    var schemaD = Schemas.Tabelas[TabelaD.getNomeTabela()];
    string[] colunasE = schemaE.colunas;
    string[] colunasD = schemaD.colunas;
    string[] tiposE = schemaE.tipos;

    int indexColunaE = Array.IndexOf(colunasE, _fieldE);
    int indexColunaD = Array.IndexOf(colunasD, _fieldD);
    string tipoColuna = tiposE[indexColunaE]; // Assumimos que os tipos são compatíveis
    int position_pag = -1;   //salva a primeira pagina que tem o valor de coluna D == E, para subir de volta caso necessario 
    string pastaBase = $"disk/merge_{TabelaE.getNomeTabela()}_{TabelaD.getNomeTabela()}";
    if (!Directory.Exists(pastaBase))
        Directory.CreateDirectory(pastaBase);

    while (paginaE != null && paginaD != null)
    {
        if (indexD >= paginaD.GetNumTuplas())
        {   
            indexD = 0;
            pagAtualD++;
            if (pagAtualD >= TabelaD.QntPags)
                break;

            paginaD = Arquivos.ReadTxtPage($"disk/{TabelaD.getNomeTabela()}/{TabelaD.getNomeTabela()}_{_fieldD}_intercalado_final/pag-{pagAtualD}.txt");
            addIO();
        }

            if (indexE >= paginaE.GetNumTuplas())
            {
                indexE = 0;
                pagAtualE++;
                if (pagAtualE >= TabelaE.QntPags)
                    break;

                paginaE = Arquivos.ReadTxtPage($"disk/{TabelaE.getNomeTabela()}/{TabelaE.getNomeTabela()}_{_fieldE}_intercalado_final/pag-{pagAtualE}.txt");
                addIO();
                indexD = 0;
                if (position_pag != -1)
                {
                    paginaD = Arquivos.ReadTxtPage($"disk/{TabelaD.getNomeTabela()}/{TabelaD.getNomeTabela()}_{_fieldD}_intercalado_final/pag-{position_pag}.txt");
                    addIO();
                    pagAtualD = position_pag;
                }
                position_pag = -1;
        }

        Tupla tuplaE = paginaE.GetTuple(indexE);
        Tupla tuplaD = paginaD.GetTuple(indexD);

        string valorE = tuplaE.Cols[indexColunaE];
        string valorD = tuplaD.Cols[indexColunaD];

        int comparacao = CompararValores(valorE, valorD, tipoColuna);

            if (comparacao == 0)
            {
                if (position_pag == -1) { position_pag = pagAtualD; }
                string linhaJoin = string.Join(",", tuplaE.Cols.Concat(tuplaD.Cols));
                Tupla novaTupla = new Tupla(linhaJoin);

                if (!paginaSaida.AddTuple(novaTupla))
                {
                    string caminhoOutput = $"disk/merge_{TabelaE.getNomeTabela()}_{TabelaD.getNomeTabela()}/pag-{tabela_mergeada.QntPags - 1}.txt";
                    Arquivos.WriteTxtPage(paginaSaida, caminhoOutput);
                    addIO();
                    paginaSaida = new Pagina();
                    tabela_mergeada.QntPags++;
                    paginaSaida.AddTuple(novaTupla);
                }

                indexD++;
            }
            else if (comparacao < 0) // e<d
            {
                indexE++;
                if (position_pag != -1)
                    {
                        paginaD = Arquivos.ReadTxtPage($"disk/{TabelaD.getNomeTabela()}/{TabelaD.getNomeTabela()}_{_fieldD}_intercalado_final/pag-{position_pag}.txt");
                        addIO();
                        pagAtualD = position_pag;
                        indexD = 0;
                        position_pag = -1;
                    }
            }
            else //e>d
            {
                indexD++;
            }
    }

    // Salva última página se tiver algo
    if (paginaSaida.GetNumTuplas() > 0)
    {
        string caminhoOutput = $"disk/merge_{TabelaE.getNomeTabela()}_{TabelaD.getNomeTabela()}/pag-{tabela_mergeada.QntPags - 1}.txt";
        Arquivos.WriteTxtPage(paginaSaida, caminhoOutput);
        addIO();
    }
}

    public static int CompararValores(string valorE, string valorD, string tipoColuna)
{
    if (tipoColuna == "int")
    {
        int intE = int.Parse(valorE);
        int intD = int.Parse(valorD);
        return intE.CompareTo(intD);
    }
    else if (tipoColuna == "string")
    {
        return string.Compare(valorE, valorD, StringComparison.Ordinal);
    }
    else
    {
        throw new Exception($"Tipo de dado não suportado: {tipoColuna}");
    }
}
}