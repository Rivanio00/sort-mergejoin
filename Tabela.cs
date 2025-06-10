using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
namespace sort_mergejoin;
    public class Tabela
    {
        private List<Pagina> _pags;
        public int QntCols { get; private set; }
        public int QntPags = 0;
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


        public void AddPage(Pagina page)
        {
            _pags.Add(page);
        }
        public static readonly Pagina PaginaVazia = new Pagina();

    public Tabela SortTable(String coluna)
    {
        // Criar nova tabela ordenada
        Tabela Tabela_Ordenada = new Tabela(NomeTabela + "_ordenada", QntCols);
        if (!Schemas.Tabelas.ContainsKey(NomeTabela))
            throw new Exception("Tabela não registrada no schema.");

        var schema = Schemas.Tabelas[NomeTabela];
        string[] colunas = schema.colunas;
        string[] tipos = schema.tipos;
        int index = Array.IndexOf(colunas, coluna);
        if (index == -1)
            throw new Exception("Coluna não encontrada.");

        string tipo = tipos[index];
        int totalPaginas = QntPags;
        int blocos = (int)Math.Ceiling(totalPaginas / 4.0);

        int contadorPagOrdenada = 0; // contador para nomear os arquivos da tabela ordenada

        string pastaOrdenada = $"disk/{Tabela_Ordenada.NomeTabela}";
        if (!Directory.Exists(pastaOrdenada))
        {
            Directory.CreateDirectory(pastaOrdenada);
        } 
        Console.WriteLine("blocos= "+blocos.ToString());
        Console.WriteLine("totalpags= " + totalPaginas.ToString());
        for (int b = 0; b < blocos; b++)
        {
            // 1. Subir até 4 páginas para a memória
            List<Tupla> todasAsTuplas = new List<Tupla>();
            for (int i = 0; i < 4; i++)
            {
                int paginaIndex = b * 4 + i;
                Console.WriteLine("paginddex= "+ paginaIndex.ToString());
                Console.WriteLine("total paginas= "+ totalPaginas.ToString());
                if (paginaIndex > (totalPaginas-1))
                {
                    break;
                }
                Console.WriteLine("paginddex= "+paginaIndex.ToString());
                Pagina pagina = Arquivos.ReadTxtPage($"disk/{NomeTabela}/pag-{NomeTabela}-{paginaIndex}.txt");
                for (int j = 0; j < pagina.qnt_tuplas_ocup; j++)
                {
                    todasAsTuplas.Add(pagina.GetTuple(j));
                }
            }

            // 2. Ordenar as tuplas do bloco
            if (tipo == "int")
            {
                todasAsTuplas.Sort((a, b) => int.Parse(a[index]).CompareTo(int.Parse(b[index])));
            }
            else
            {
                todasAsTuplas.Sort((a, b) => string.Compare(a[index], b[index], StringComparison.Ordinal));
            }

            // 3. Recriar páginas ordenadas e SALVAR no disco
            Pagina paginaAtual = new Pagina();
            foreach (var tupla in todasAsTuplas)
            {
                if (!paginaAtual.AddTuple(tupla))
                {
                    // Página cheia -> salvar no disco
                    Arquivos.WriteTxtPage(paginaAtual, $"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{contadorPagOrdenada}.txt");
                    contadorPagOrdenada++;
                    paginaAtual = new Pagina();
                    paginaAtual.AddTuple(tupla);
                }
            }
            // Salva a última página restante
            if (paginaAtual.qnt_tuplas_ocup > 0)
            {
                Arquivos.WriteTxtPage(paginaAtual, $"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{contadorPagOrdenada}.txt");
                contadorPagOrdenada++;
            }

            // Limpa buffer da lista de tuplas
            todasAsTuplas.Clear();
        }
        //intercalação
        //ler ao menos 3 páginas e manter uma livre no buffer como output --- 1 páina por bloco até ter os 3 blocos ordenados e faz isso com o resto dos blocosAdd commentMore actions
        //pulo de 12
        //ler ao menos 3 páginas e manter uma livre no buffer como output --- repetir o processo até alcançar um único bloco ordenado
        //pulo de 36...

         // ====== FASE 2: INTERCALAÇÃO DOS BLOCOS 2 a 2 ======
    int blocosOrdenados = (int)Math.Ceiling(contadorPagOrdenada / 4.0);
    int novoIndex = 0;

    string nomeFinal = Tabela_Ordenada.NomeTabela + "_intercalada_";
    Tabela tabelaFinal = new Tabela(nomeFinal, QntCols);
    string pastaFinal = $"disk/{nomeFinal+coluna}";
    if (!Directory.Exists(pastaFinal))
        Directory.CreateDirectory(pastaFinal);

    for (int b = 0; b < blocosOrdenados; b += 2)
    {
        int startA = b * 4;
        int endA = Math.Min(startA + 3, contadorPagOrdenada - 1);

        int startB = startA + 4;
        int endB = Math.Min(startB + 3, contadorPagOrdenada - 1);
        if (startB >= contadorPagOrdenada)
        {
            // Último bloco solitário (sem par pra intercalar), apenas copiar
            for (int p = startA; p <= endA; p++)
            {
                Pagina pag = Arquivos.ReadTxtPage($"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{p}.txt");
                Arquivos.WriteTxtPage(pag, $"disk/{nomeFinal+coluna}/pag-{nomeFinal}-{novoIndex}.txt");
                novoIndex++;
            }
            break;
        }

        int ptrA = 0, ptrB = 0;
        int pagAIndex = startA, pagBIndex = startB;

        Pagina? pagA = Arquivos.ReadTxtPage($"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{pagAIndex}.txt");
        Pagina? pagB = Arquivos.ReadTxtPage($"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{pagBIndex}.txt");
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
                Arquivos.WriteTxtPage(pagSaida, $"disk/{nomeFinal+coluna}/pag-{nomeFinal}-{novoIndex}.txt");
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
                    pagA = (pagAIndex <= endA) ? Arquivos.ReadTxtPage($"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{pagAIndex}.txt") : null;
                }
            }
            else if (menor == tuplaB)
            {
                ptrB++;
                if (ptrB >= pagB!.qnt_tuplas_ocup)
                {
                    ptrB = 0;
                    pagBIndex++;
                    pagB = (pagBIndex <= endB) ? Arquivos.ReadTxtPage($"disk/{Tabela_Ordenada.NomeTabela}/pag-{Tabela_Ordenada.NomeTabela}-{pagBIndex}.txt") : null;
                }
            }
        }

        if (pagSaida.qnt_tuplas_ocup > 0)
        {
            Arquivos.WriteTxtPage(pagSaida, $"disk/{nomeFinal+coluna}/pag-{nomeFinal}-{novoIndex}.txt");
            novoIndex++;
        }
    }

        return Tabela_Ordenada;
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
            Arquivos.WriteTxtLine($"disk/{NomeTabela}/pag-{NomeTabela}-{QntPags}.txt", lines[i]);
            if (i % 10 == 0) QntPags += 1;
        }QntPags++;

        SetMetadata(NomeTabela, i - 1);
        }
    }