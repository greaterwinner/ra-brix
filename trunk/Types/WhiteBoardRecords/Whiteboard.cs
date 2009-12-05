/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using Ra.Brix.Data;
using System.Collections.Generic;

namespace WhiteBoardRecords
{
    [ActiveType]
    public class Whiteboard : ActiveType<Whiteboard>
    {
        [ActiveType]
        public class Column : ActiveType<Column>
        {
            public Column()
            {
                ShowInSummary = true;
            }

            [ActiveField]
            public string Caption { get; set; }

            [ActiveField]
            public string Type { get; set; }

            [ActiveField]
            public int Position { get; set; }

            [ActiveField]
            public bool ShowInSummary { get; set; }
        }

        [ActiveType]
        public class Row : ActiveType<Row>
        {
            public Row()
            {
                Cells = new List<Cell>();
            }

            [ActiveField]
            public List<Cell> Cells { get; set; }
        }

        [ActiveType]
        public class Cell : ActiveType<Cell>
        {
            [ActiveField]
            public string Value { get; set; }

            [ActiveField(IsOwner = false)]
            public Column Column { get; set; }
        }

        public Whiteboard()
        {
            Columns = new List<Column>();
            Rows = new List<Row>();
            PageSize = -1;
        }

        [ActiveField]
        public string Name { get; set; }

        [ActiveField]
        public bool EnableFiltering { get; set; }

        [ActiveField]
        public bool EnableHeaders { get; set; }

        [ActiveField]
        public bool EnableDeletion { get; set; }

        [ActiveField]
        public int PageSize { get; set; }

        [ActiveField]
        public List<Column> Columns { get; set; }

        [ActiveField]
        public List<Row> Rows { get; set; }

        public void AddRow()
        {
            Row row = new Row();
            foreach (Column idx in Columns)
            {
                Cell cell = new Cell();
                cell.Column = idx;
                cell.Value = "";
                row.Cells.Add(cell);
            }
            Rows.Add(row);
            Save();
        }

        public override void Save()
        {
            // Organizing the rows to make sure they've got the right columns and such...
            foreach (Row idxRow in Rows)
            {
                // Removing cells which have had their columns deleted
                idxRow.Cells.RemoveAll(
                    delegate(Cell idxCell)
                    {
                        return !Columns.Exists(
                            delegate(Column idxCol)
                            {
                                return idxCell.Column.ID == idxCol.ID;
                            });
                    });

                // Adding cells which have had a new column created but cell is not created...
                foreach (Column idxColumn in Columns)
                {
                    int columnId = idxColumn.ID;
                    if (!idxRow.Cells.Exists(
                        delegate(Cell idxCur)
                        {
                            return columnId == idxCur.Column.ID;
                        }))
                    {
                        Cell tmp = new Cell();
                        tmp.Column = idxColumn;
                        idxRow.Cells.Add(tmp);
                    }
                }
            }
            base.Save();
        }
    }
}






