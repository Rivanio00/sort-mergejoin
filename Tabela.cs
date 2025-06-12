using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
namespace sort_mergejoin;
    public class Tabela
    {
        private List<Pagina> _pags;
        public int QntCols { get; private set; }
        public int QntPags = 0;
        private int qntIos {get; set;}
        private String _csvPath;

        private String NomeTabela { get; set; }
        public Tabela(String csvPath)
        {
            _pags = new List<Pagina>();
            _csvPath = csvPath;
            String[] lines = Arquivos.ReadCsvLines(csvPath);
            QntCols = Arquivos.ParseLine(lines[0]).Length;
            NomeTabela = _csvPath.Split('.')[0];
        }

        public Tabela(String nometabela, int qnt_colunas)
        {
            NomeTabela = nometabela;
            QntCols = qnt_colunas;
            _pags = new List<Pagina>();
            _csvPath = NomeTabela + ".txt";
        }

        public void addIO() 
        {
            qntIos += 1;
        }
        
        public String getNomeTabela()
        {
            return NomeTabela;
        }
        
        public void AddPage(Pagina page)
    {
        _pags.Add(page);
    }
        public static readonly Pagina PaginaVazia = new Pagina();

    public int SortTable(string coluna)
    {
        // Criar nova tabela ordenada
        Tabela Tabela_Ordenada = new Tabela(NomeTabela + "_ordenada", QntCols);
        if (!Schemas.Tabelas.ContainsKey(NomeTabela))
            throw new Exception("Tabela não registrada no schema.");
        Tabela_Ordenada.NomeTabela = Tabela_Ordenada.NomeTabela + "_" + coluna;

        var schema = Schemas.Tabelas[NomeTabela];
        string[] colunas = schema.colunas;
        string[] tipos = schema.tipos;
        int index = Array.IndexOf(colunas, coluna);
        if (index == -1)
            throw new Exception("Coluna não encontrada.");

        string tipo = tipos[index];
        int totalPaginas = QntPags;
        int blocos = (int)Math.Ceiling(totalPaginas / 4.0);
        int contadorPagOrdenada = 0;

        string pastaBase = $"disk/{NomeTabela}/{Tabela_Ordenada.NomeTabela}";
        if (!Directory.Exists(pastaBase))
            Directory.CreateDirectory(pastaBase);

        // ====== FASE 1: ordenar cada bloco de até 4 páginas individualmente ======
        for (int b = 0; b < blocos; b++)
        {
            List<Tupla> todasAsTuplas = new List<Tupla>();

            for (int i = 0; i < 4; i++)
            {
                int paginaIndex = b * 4 + i;
                if (paginaIndex >= totalPaginas)
                    break;

                Pagina pagina = Arquivos.ReadTxtPage($"disk/{NomeTabela}/pag-{paginaIndex}.txt");
                addIO();
                for (int j = 0; j < pagina.qnt_tuplas_ocup; j++)
                    todasAsTuplas.Add(pagina.GetTuple(j));
            }

            if (tipo == "int")
                todasAsTuplas.Sort((a, b) => int.Parse(a[index]).CompareTo(int.Parse(b[index])));
            else
                todasAsTuplas.Sort((a, b) => string.Compare(a[index], b[index], StringComparison.Ordinal));

            Pagina paginaAtual = new Pagina();
            foreach (var tupla in todasAsTuplas)
            {
                if (!paginaAtual.AddTuple(tupla))
                {
                    Arquivos.WriteTxtPage(paginaAtual, $"{pastaBase}/pag-{contadorPagOrdenada}.txt");
                    addIO();
                    contadorPagOrdenada++;
                    paginaAtual = new Pagina();
                    paginaAtual.AddTuple(tupla);
                }
            }

            if (paginaAtual.qnt_tuplas_ocup > 0)
            {
                Arquivos.WriteTxtPage(paginaAtual, $"{pastaBase}/pag-{contadorPagOrdenada}.txt");
                addIO();
                contadorPagOrdenada++;
            }
        }

        // ====== FASE 2: INTERCALAÇÃO DOS BLOCOS 2 A 2 ======
        int tamanhoBloco = 4;
        int rodada = 0;
        string nomeBase = Tabela_Ordenada.NomeTabela;
        string pastaEntrada = pastaBase;

        while (tamanhoBloco < contadorPagOrdenada)
        {
            string nomeSaida = $"{nomeBase}_intercalado_{rodada}";
            string pastaSaida = $"disk/{NomeTabela}/{nomeSaida}";
            if (!Directory.Exists(pastaSaida)) Directory.CreateDirectory(pastaSaida);

            int novoIndex = 0;
            for (int b = 0; b < contadorPagOrdenada; b += tamanhoBloco * 2)
            {
                int startA = b;
                int endA = Math.Min(startA + tamanhoBloco - 1, contadorPagOrdenada - 1);

                int startB = endA + 1;
                int endB = Math.Min(startB + tamanhoBloco - 1, contadorPagOrdenada - 1);

                if (startB >= contadorPagOrdenada)
                {
                    for (int p = startA; p <= endA; p++)
                    {
                        Pagina pag = Arquivos.ReadTxtPage($"{pastaEntrada}/pag-{p}.txt");
                        addIO();
                        Arquivos.WriteTxtPage(pag, $"{pastaSaida}/pag-{novoIndex}.txt");
                        addIO();
                        novoIndex++;
                    }
                    break;
                }

                int ptrA = 0, ptrB = 0;
                int pagAIndex = startA, pagBIndex = startB;

                Pagina? pagA = Arquivos.ReadTxtPage($"{pastaEntrada}/pag-{pagAIndex}.txt");
                addIO();
                Pagina? pagB = Arquivos.ReadTxtPage($"{pastaEntrada}/pag-{pagBIndex}.txt");
                addIO();
                Pagina pagSaida = new Pagina();

                while (pagA != null || pagB != null)
                {
                    Tupla? tuplaA = (pagA != null && ptrA < pagA.qnt_tuplas_ocup) ? pagA.GetTuple(ptrA) : null;
                    Tupla? tuplaB = (pagB != null && ptrB < pagB.qnt_tuplas_ocup) ? pagB.GetTuple(ptrB) : null;
                    Tupla? menor = null;

                    if (tuplaA != null && tuplaB != null)
                        menor = (Comparar(tuplaA, tuplaB, index, tipo) <= 0) ? tuplaA : tuplaB;
                    else if (tuplaA != null)
                        menor = tuplaA;
                    else if (tuplaB != null)
                        menor = tuplaB;
                    else
                        break;

                    if (!pagSaida.AddTuple(menor))
                    {
                        Arquivos.WriteTxtPage(pagSaida, $"{pastaSaida}/pag-{novoIndex}.txt");
                        addIO();
                        novoIndex++;
                        pagSaida = new Pagina();
                        pagSaida.AddTuple(menor);
                    }

                    if (menor == tuplaA)
                    {
                        ptrA++;
                        if (ptrA >= pagA!.qnt_tuplas_ocup)
                        {
                            ptrA = 0;
                            pagAIndex++;
                            pagA = (pagAIndex <= endA) ? Arquivos.ReadTxtPage($"{pastaEntrada}/pag-{pagAIndex}.txt") : null;
                            addIO();
                        }
                    }
                    else if (menor == tuplaB)
                    {
                        ptrB++;
                        if (ptrB >= pagB!.qnt_tuplas_ocup)
                        {
                            ptrB = 0;
                            pagBIndex++;
                            pagB = (pagBIndex <= endB) ? Arquivos.ReadTxtPage($"{pastaEntrada}/pag-{pagBIndex}.txt") : null;
                            addIO();
                        }
                    }
                }

                if (pagSaida.qnt_tuplas_ocup > 0)
                {
                    Arquivos.WriteTxtPage(pagSaida, $"{pastaSaida}/pag-{novoIndex}.txt");
                    addIO();
                    novoIndex++;
                }
            }

            // Preparar próxima rodada
            tamanhoBloco *= 2;
            contadorPagOrdenada = novoIndex;
            pastaEntrada = pastaSaida;
            nomeBase = nomeSaida;
            rodada++;
        }

        // ====== FASE FINAL: CRIAR A TABELA FINAL COM DADOS ORDENADOS ======
        Tabela tabelaFinal = new Tabela($"{NomeTabela}_{coluna}_intercalado_final", QntCols);
        tabelaFinal.NomeTabela = tabelaFinal.NomeTabela;
        tabelaFinal.QntPags = contadorPagOrdenada;

        string pastaFinal = $"disk/{NomeTabela}/{tabelaFinal.NomeTabela}";
        // Deleta a pasta final se já existir, para evitar conflito no Move
        if (Directory.Exists(pastaFinal))
            Directory.Delete(pastaFinal, true);

        // Move a pasta da última rodada de intercalação para o nome final
        Directory.Move(pastaEntrada, pastaFinal);
        return qntIos;
    }


        public int Comparar(Tupla a, Tupla b, int index, string tipo)
        {
            if (tipo == "int")
                return int.Parse(a[index]).CompareTo(int.Parse(b[index]));
            else
                return string.Compare(a[index], b[index], StringComparison.Ordinal);
        }
        public Pagina GetPage(int index)
        {
            if (index < 0 || index >= _pags.Count)
                throw new IndexOutOfRangeException();
            return _pags[index];
        }

        public String[] GetMetadata(String tableName)
        {
            Directory.CreateDirectory($"disk/{tableName}");
            String metaPath = $"disk/{tableName}/meta-{tableName}.txt";
            String[] lines = File.Exists(metaPath) ? File.ReadAllLines(metaPath) : [""];
            if (lines.Length == 0 || lines[0] == "")
            {
                Arquivos.WriteTxtLine(metaPath, "0", false);
                return File.ReadAllLines(metaPath);
            }
            return lines;
        }

        public void SetMetadata(String tableName, int qntTuplas = -1) // 0 = qntTuplas
        {
            String[] fields = GetMetadata(tableName);
            if (qntTuplas != -1)
            {
                fields[0] = qntTuplas.ToString();
            }
            Arquivos.WriteTxtLine($"disk/{tableName}/meta-{tableName}.txt", fields[0], false);
        }

    public void CarregarDados()
    {
        int qntTuplas = int.Parse(GetMetadata(NomeTabela)[0]);
        QntPags = 0;
        String[] lines = Arquivos.ReadCsvLines(_csvPath);
        int i = qntTuplas + 1;
        for (; i < lines.Length; i++) // Pois a primeira linha é a estrutura da tupla
        {
            Arquivos.WriteTxtLine($"disk/{NomeTabela}/pag-{QntPags}.txt", lines[i]);
            if (i % 10 == 0) QntPags += 1;
        }
        if ((lines.Length-1) % 10 > 0) { QntPags++;}

        SetMetadata(NomeTabela, i - 1);
        }
    }