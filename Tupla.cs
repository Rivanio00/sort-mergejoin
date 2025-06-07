namespace sort_mergejoin;
    public class Tupla
    {
        public string[] Cols { get; private set; }

        public string this[int index]
        {
            get { return Cols[index]; }
            set { Cols[index] = value; }
        }

        public Tupla(String linha)
        {
            Cols = linha.Split(",");
        }

        public override string ToString()
        {
            return string.Join(",", Cols);
        }
    }