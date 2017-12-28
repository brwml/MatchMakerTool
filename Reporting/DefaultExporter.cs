using System.IO;
using Newtonsoft.Json;

namespace MatchMaker.Reporting
{
    public class DefaultExporter : IExporter
    {
        public void Export(Summary summary, string folder)
        {
            var path = Path.Combine(folder, summary.Name + ".summary");
            File.WriteAllText(path, JsonConvert.SerializeObject(summary, Formatting.Indented));
        }
    }
}