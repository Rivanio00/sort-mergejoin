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

            //tabela que receberá as páginas orenadas
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

            //ler grupos de tamanho 4 e ordenar --- ceil(n/4) ordenações = também é o número de grupos ordenados(chamarei de bloco)
            //pulo de 4
            Console.WriteLine("-"+NomeTabela+"-");

            for (int i = 0; i < 4; i++)
            {
                AddPage(Arquivos.ReadTxtPage($"disk/{NomeTabela}/pag-{NomeTabela}-{i.ToString()}.txt"));
            }
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(GetPage(i).ToString());
            }

            //ler ao menos 3 páginas e manter uma livre no buffer como output --- 1 páina por bloco até ter os 3 blocos ordenados e faz isso com o resto dos blocos
            //pulo de 12

            //ler ao menos 3 páginas e manter uma livre no buffer como output --- repetir o processo até alcançar um único bloco ordenado
            //pulo de 36...

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