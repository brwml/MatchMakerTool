namespace MatchMaker.Reporting
{
    public interface IExporter
    {
        void Export(Summary summary, string folder);
    }
}