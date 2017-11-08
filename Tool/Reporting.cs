﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MatchMaker.Reporting;

namespace MatchMaker.Tool
{
    internal static class Reporting
    {
        public static void Process(ReportingOptions options)
        {
            if (options == null)
            {
                return;
            }

            var schedule = LoadScheduleFromFolder(options.SourceFolder);
            var result = LoadResultsFromFolder(options.SourceFolder, schedule);
            var summary = Summary.FromResult(result, LoadRankingPolicies(options.RankingProcedure));

            Parallel.ForEach(GetExports(options.OutputFormat), exporter =>
            {
                exporter.Export(summary, options.OutputFolder);
            });

            if (options.NumberOfAlternateTeams > 0)
            {
                TournamentExporter.Create(summary, options.NumberOfTournamentTeams, options.NumberOfAlternateTeams, options.OutputFolder);
            }
        }

        private static IEnumerable<FileInfo> FindResultFiles(string sourceFolder)
        {
            return Directory
                .EnumerateFiles(sourceFolder, "*.results.xml", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x));
        }

        private static FileInfo FindScheduleFile(string folder)
        {
            return Directory
                .EnumerateFiles(folder, "*.schedule.xml", SearchOption.TopDirectoryOnly)
                .Select(x => new FileInfo(x))
                .FirstOrDefault();
        }

        private static IEnumerable<IExporter> GetExports(OutputFormat format)
        {
            var list = new List<IExporter>();

            if (format.HasFlag(OutputFormat.Excel))
            {
                list.Add(new ExcelExporter());
            }

            if (format.HasFlag(OutputFormat.Html))
            {
                list.Add(new HtmlExporter());
            }

            if (format.HasFlag(OutputFormat.Pdf))
            {
                list.Add(new PdfExporter());
            }

            return list;
        }

        private static string GetScheduleName(string name)
        {
            var ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(name.Substring(0, name.IndexOf(".schedule", StringComparison.OrdinalIgnoreCase)));
        }

        private static TeamRankingPolicy[] LoadRankingPolicies(string procedure)
        {
            return procedure.ToUpperInvariant().Select(TeamRankingPolicyFromChar).ToArray();
        }

        private static Result LoadResultsFromFiles(IEnumerable<FileInfo> files, Schedule schedule)
        {
            return Result.FromXml(files.Select(x => LoadXml(x)), schedule);
        }

        private static Result LoadResultsFromFolder(string sourceFolder, Schedule schedule)
        {
            return LoadResultsFromFiles(FindResultFiles(sourceFolder), schedule);
        }

        private static Schedule LoadScheduleFromFile(FileInfo file)
        {
            return Schedule.FromXml(LoadXml(file), GetScheduleName(file.Name));
        }

        private static Schedule LoadScheduleFromFolder(string folder)
        {
            return LoadScheduleFromFile(FindScheduleFile(folder));
        }

        private static XDocument LoadXml(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return XDocument.Load(stream);
            }
        }

        private static TeamRankingPolicy TeamRankingPolicyFromChar(char c)
        {
            switch (c)
            {
                case 'W':
                    return new WinPercentageTeamRankingPolicy();

                case 'S':
                    return new ScoreTeamRankingPolicy();

                case 'E':
                    return new ErrorTeamRankingPolicy();

                case 'H':
                    return new HeadToHeadTeamRankingPolicy();

                case 'L':
                    return new LossCountTeamRankingPolicy();
            }

            return new NullTeamRankingPolicy();
        }
    }
}