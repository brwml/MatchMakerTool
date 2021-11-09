@echo off

dotnet test --collect:"XPlat Code Coverage" --results-directory .\coverage

reportgenerator -reports:".\coverage\*\coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
