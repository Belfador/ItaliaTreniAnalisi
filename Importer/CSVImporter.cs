using Importer.Models;
using Importer.Services;
using System.Globalization;

namespace Importer
{
    internal class CSVImporter
    {
        private static CSVImporter? instance;
        private string directoryPath;
        private string filePath;

        private const string validExtension = ".csv";
        private const int validColumnCount = 5;
        private static readonly string[] validHeaderValues = { "mm", "p1", "p2", "p3", "p4" };

        public static CSVImporter Instance => instance ??= new CSVImporter();

        private CSVImporter()
        {
            directoryPath = Directory.GetCurrentDirectory();
            filePath = string.Empty;
        }

        public async Task Start()
        {
            try
            {
                Logger.Instance.Log("Application started.");

                GetFile();
                if (!await IsValid()) throw new InvalidDataException("CSV file is invalid.");
                await Import();

                Logger.Instance.Log("Application finished.");
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.Message);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private void GetFile()
        {
            var files = Directory.GetFiles(directoryPath).Where(IsValidExtension);
            filePath = files.Single();

            Logger.Instance.Log($"File found: {filePath}");
        }

        private static bool IsValidExtension(string filePath) => Path.GetExtension(filePath).ToLower() == validExtension;

        private async Task<bool> IsValid()
        {
            return await Task.Run(() =>
            {
                int linesRead = 0;
                int linesCount = File.ReadLines(filePath).Count();

                using (var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        if (values.Length != validColumnCount) return false;

                        if (linesRead == 0 && !IsValidHeader(values)) return false;
                        else if (linesRead > 0)
                        {
                            if (!int.TryParse(values[0], out _)) return false;
                            foreach (var value in values.Skip(1))
                            {
                                if (!double.TryParse(value, out _)) return false;
                            }
                        }

                        if (++linesRead % 300000 == 0)
                        {
                            Logger.Instance.Log($"Validating CSV file... {linesRead/(float)linesCount * 100:0.00}%");
                        }
                    }
                }

                Logger.Instance.Log("CSV file is valid.");

                return true;
            });
        }

        private static bool IsValidHeader(string[] values)
        {
            if (values.Length != validColumnCount) return false;
            for (var col = 0; col < validColumnCount; col++)
            {
                if (values[col] != validHeaderValues[col]) return false;
            }
            return true;
        }

        private async Task Import()
        {
            await Task.Run(async () =>
            {
                Logger.Instance.Log("Importing CSV file...");

                var samples = GetSamples();
                await SampleService.Instance.ImportSamplesAsync(samples);

                Logger.Instance.Log("CSV file imported successfully.");
            });
        }

        private IEnumerable<Sample> GetSamples()
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var values = line.Split(',');

                    if (values[0] == validHeaderValues[0]) continue; // Skip the header

                    yield return new()
                    {
                        Parameter1 = double.Parse(values[1], CultureInfo.InvariantCulture),
                        Parameter2 = double.Parse(values[2], CultureInfo.InvariantCulture),
                        Parameter3 = double.Parse(values[3], CultureInfo.InvariantCulture),
                        Parameter4 = double.Parse(values[4], CultureInfo.InvariantCulture)
                    };
                }
            }
        }
    }
}
