using Importer.Models;
using System.Text;
using System.Text.Json;

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

                if (!GetFile()) throw new FileNotFoundException("CSV file not found.");
                if (await IsValid() == false) throw new InvalidDataException("CSV file is invalid.");
                if (await Import() == false) throw new Exception("CSV file import failed.");

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

        private bool GetFile()
        {
            if (!Directory.Exists(directoryPath)) throw new DirectoryNotFoundException("Directory not found.");

            var files = Directory.GetFiles(directoryPath).Where(IsValidExtension).ToList();

            if (files.Count == 0) return false;
            if (files.Count > 1) throw new FileNotFoundException("Multiple CSV files found.");

            filePath = files[0];

            Logger.Instance.Log($"File found: {filePath}");

            return true;
        }

        private static bool IsValidExtension(string filePath) => Path.GetExtension(filePath).ToLower() == validExtension;

        private async Task<bool> IsValid()
        {
            return await Task.Run(() =>
            {
                Logger.Instance.Log("Validating CSV file...");

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

                        if (++linesRead % 150000 == 0) Logger.Instance.Log($"Validated {linesRead} of {linesCount} lines.");
                    }
                }

                Logger.Instance.Log("CSV file is valid.");

                return true;
            });
        }

        private bool IsValidHeader(string[] values)
        {
            if (values.Length != validColumnCount) return false;
            for (var col = 0; col < validColumnCount; col++)
            {
                if (values[col] != validHeaderValues[col]) return false;
            }
            return true;
        }

        private async Task<bool> Import()
        {
            return await Task.Run(async () =>
            {
                Logger.Instance.Log("Importing CSV file...");

                List<Sample> samples = [];

                using (var reader = new StreamReader(filePath))
                {
                    bool headerSkipped = false;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (!headerSkipped)
                        {
                            headerSkipped = true;
                            continue;
                        }

                        var values = line.Split(',');

                        var sample = new Sample
                        {
                            Parameter1 = double.Parse(values[1], System.Globalization.NumberStyles.AllowDecimalPoint),
                            Parameter2 = double.Parse(values[2], System.Globalization.NumberStyles.AllowDecimalPoint),
                            Parameter3 = double.Parse(values[3], System.Globalization.NumberStyles.AllowDecimalPoint),
                            Parameter4 = double.Parse(values[4], System.Globalization.NumberStyles.AllowDecimalPoint)
                        };

                        samples.Add(sample);
                    }
                }

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7217/Analysis/ImportSamples");
                var content = new StringContent(JsonSerializer.Serialize(samples), Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                Logger.Instance.Log("CSV file imported successfully.");

                return true;
            });
        }
    }
}
