using System.Collections.Generic;

class Table
{
    public int num_columns { get; private set; }

    private List<Page> pages;

    public Table(int numcol)
    {
        num_columns = numcol;
        pages = new List<Page>();
    }

    public void AddPage(Page page)
    {
        pages.Add(page);
    }

    public Page GetPage(int index)
    {
        if (index < 0 || index >= pages.Count)
            throw new IndexOutOfRangeException();
        return pages[index];
    }

    public int  NumPaginas => pages.Count;
}