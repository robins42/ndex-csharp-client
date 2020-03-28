namespace NDExApi.model
{
    public class SimpleQuery
    {
        private string _internalSearchString;
        
        public string searchString
        {
            get { return _internalSearchString; }
            set { _internalSearchString = value ?? ""; }
        }
    }
}