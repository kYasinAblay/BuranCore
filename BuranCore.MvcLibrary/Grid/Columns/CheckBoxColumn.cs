namespace Buran.Core.MvcLibrary.Grid.Columns
{
    public class CheckBoxColumn : DataColumn
    {
        public string CheckedField { get; set; }

        public CheckBoxColumn()
        {
            EditorType = ColumnTypes.CheckBox;
            Filterable = false;
            Sortable = false;
        }
    }
}
