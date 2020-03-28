namespace NDExApi.model
{
    public class SimplePathQuery : SimpleQuery
    {
        public int searchDepth = 1;
        public int edgeLimit = 0;
        public bool errorWhenLimitIsOver = false;
        public bool directOnly = false;
    }
}