namespace Buran.Core.MvcLibrary.Grid
{
    public class DataGridCommand
    {
        public DataGridCommand()
        {
            Ajax = false;
        }
        public bool Ajax { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Confirm { get; set; }
        public string Css { get; set; }
        public string Target { get; set; }
    }
}
