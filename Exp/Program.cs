using System.Diagnostics;

namespace SomeCode
{
    public class Program
    {
        public static void Main()
        {
            Stopwatch sw = new();
            sw.Start();

            string fileText = string.Empty;
            string filePath = @"C:\Projects\Content.txt";
            try
            {
                using StreamReader sr = new(filePath);
                fileText = sr.ReadToEnd();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error occurred");
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            Console.WriteLine($"File data: {fileText}");

            List<string> tripletList = new();
            fileText.Split(new char[] { ' ', ',', '.', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(el => el.Length > 2)
                .Select(x => x.ToLower()).ToList()
                .ForEach(x =>
                {
                    for (int i = 0; i < x.Length - 2; i++)
                        tripletList.Add(x.Substring(i, 3));
                });

            Thread subThread = new(
                new ParameterizedThreadStart(arg => GetData(arg as List<string>)));

            subThread.Start(tripletList);
            subThread.Join();

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Console.WriteLine($"RunTime: {ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");
            Console.Read();
        }

        public static void GetData(List<string> dataList)
        {
            var data = dataList
                .GroupBy(x => x)
                .Select(g => new { g.Key, Value = g.Count() })
                .OrderByDescending(x => x.Value);

            Console.WriteLine("Result: \n" + string.Join("\n", data.Take(10).Select(x => $"Key: {x.Key}, Count: {x.Value}")));
        }
    }
}