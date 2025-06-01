Class Table
    num_columns = número de colunas da tabela
    pages = lista de páginas da tabela
    NumPaginas => pages.Count; -> número de páginas da tabela


Class Page
    num_tuple = número de tuplas na página(<=10)
    tuples = vetor de tuplas
    AddTuple() -> adiciona uma tupla à pagina, caso ainda tenha espaço


Class Tuple
    values = vetor com os valores da tupla
    size = tamanho do vetor(número de colunas)
    public string this[int index] -> Acessa os campos com o índice
                                     Tuple t1 = new Tuple(3);
                                     t1[0] = "1";
                                     t1[1] = "Cabernet";
                                     t1[2] = "2020";
    ToString() -> Printa a tupla