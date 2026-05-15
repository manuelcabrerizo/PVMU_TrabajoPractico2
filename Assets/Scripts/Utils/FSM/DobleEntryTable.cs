using System;

public class DobleEntryTable<RV, CV, TV>
{
    private RV[] rows;
    private CV[] columns;
    private TV[,] table;

    public DobleEntryTable(RV[] rows, CV[] cols)
    { 
        this.rows = rows;
        this.columns = cols;
        table = new TV[rows.Length, cols.Length];
    }

    public TV this[RV row, CV col]
    {
        get 
        {
            int rowIndex = Array.IndexOf(rows, row);
            int colIndex = Array.IndexOf(columns, col);
            return table[rowIndex, colIndex];
        }
        set
        {
            int rowIndex = Array.IndexOf(rows, row);
            int colIndex = Array.IndexOf(columns, col);
            table[rowIndex, colIndex] = value;
        }
    }
}