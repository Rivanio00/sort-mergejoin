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

        int totalPaginas = QntPags; // <- Garanta que isso representa o total de páginas da tabela
        int blocos = (int)Math.Ceiling(totalPaginas / 4.0);

        int contadorPagOrdenada = 0; // contador para nomear os arquivos da tabela ordenada
        
        string pastaOrdenada = $"disk/{Tabela_Ordenada.NomeTabela}";
        if (!Directory.Exists(pastaOrdenada))
        {
            Directory.CreateDirectory(pastaOrdenada);
        }
        for (int b = 0; b < blocos; b++)
        {
            // 1. Subir até 4 páginas para a memória
            List<Tupla> todasAsTuplas = new List<Tupla>();
            for (int i = 0; i < 4; i++)
            {
                int paginaIndex = b * 4 + i;
                if (paginaIndex >= totalPaginas)
                    break;

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
            return Tabela_Ordenada;
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
            QntPags = (qntTuplas + 9) / 10;
            String[] lines = Arquivos.ReadCsvLines(_csvPath);
            int i = qntTuplas + 1;
            for (; i < lines.Length; i++) // Pois a primeira linha é a estrutura da tupla
            {
                Arquivos.WriteTxtLine($"disk/{NomeTabela}/pag-{NomeTabela}-{QntPags}.txt", lines[i]);
                if (i % 10 == 0) QntPags += 1;
            }
            SetMetadata(NomeTabela, i - 1);
        }
    }