using System.IO;
using System.Linq;

namespace MatchMaker.Reporting
{
    public class GumChallengeExporter : IExporter
    {
        public void Export(Summary summary, string folder)
        {
            var quizzers = summary.QuizzerSummaries.Select(x => new
            {
                x.Value.QuizzerId,
                Name = GetQuizzerName(summary, x.Value),
                x.Value.Place,
                x.Value.AverageScore,
                x.Value.AverageErrors,
                TeamId = GetTeamId(summary, x.Value)
            });

            var teams = quizzers.GroupBy(x => x.TeamId)
                .Select(x => x.OrderBy(y => y.Place).Last())
                .OrderByDescending(x => x.Place)
                .ToArray();

            var path = Path.Combine(folder, "gum.txt");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            for (var i = 0; i < teams.Length - 1; i += 2)
            {
                File.AppendAllText(Path.Combine(folder, "gum.txt"), $"{teams[i].Name} | {teams[i + 1].Name}\r\n");
            }
        }

        private static string GetQuizzerName(Summary summary, QuizzerSummary quizzer)
        {
            return summary.Result.Schedule.Quizzers
                .Where(x => x.Value.Id == quizzer.QuizzerId)
                .Select(x => $"{x.Value.FirstName} {x.Value.LastName}")
                .First();
        }

        private static int GetTeamId(Summary summary, QuizzerSummary quizzer)
        {
            return summary.Result.Schedule.Quizzers
                .Where(x => x.Value.Id == quizzer.QuizzerId)
                .Select(x => x.Value.TeamId)
                .First();
        }
    }
}