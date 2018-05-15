using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace StreamsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var newsResults = BingNewsSearch("Diego Valeri");
            var sentiment = GetSentimentResponse(newsResults);

            string curDir = Directory.GetCurrentDirectory();
            DirectoryInfo dirInfo = new DirectoryInfo(curDir);
            var files = dirInfo.GetFiles("*.txt");

            var fileName = Path.Combine(dirInfo.FullName, "data.txt");
            var fileInfo = new FileInfo(fileName);
            if (fileInfo.Exists)
            {
                var encoding = Encoding.GetEncoding(1251); // EncodingName = "Cyrillic (Windows)"; HeaderName = "windows-1251", can be passed as name parameter; CodePage = 1251
                // GetEncoding("Unicode" | "utf-16" | 1200) -> EncodingName = "Unicode"; HeaderName = "utf-16"; CodePage = 1200

                using (var reader = new StreamReader(fileInfo.FullName, encoding)) // default Encoding = {System.Text.UTF8Encoding}; CodePage 65001; EncodingName = "Unicode (UTF-8)"; HeaderName = "utf-8"
                                                                                   // .Net Core default is ISO-8859-1 (code page 28591)
                {
                    var line = reader.ReadLine();
                    Console.WriteLine(line);
                }

            }

            char lowerH = '\u0068';
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(new char[] { lowerH, 'i' });
            string unicodeString = Encoding.Unicode.GetString(unicodeBytes);

            var resultsFileName = Path.Combine(dirInfo.FullName, "SoccerGameResults.csv");
            //var results = ReadFile(resultsFileName);
            //var lines = results.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var results = ReadGameResults(resultsFileName);

            fileName = Path.Combine(dirInfo.FullName, "players.json");
            var players = DeserializePlayers(fileName);
            var top10players = GetTopTenPlayers(players);
            fileName = Path.Combine(dirInfo.FullName, "top10players.json");
            SerializePlayers(top10players, fileName);
        }

        static string ReadFile(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        private static List<GameResult> ReadGameResults(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }

            var result = new List<GameResult>();
            using (var reader = new StreamReader(fileName))
            {
                var format = CultureInfo.InvariantCulture;// culture-insensitive; it is associated with the English language but not with any country/region
                bool isHeaderLine = true;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (isHeaderLine)
                    {
                        isHeaderLine = false;
                        continue;
                    }

                    string[] values = line.Split(',');
                    if (values.Length >= 8 && !string.IsNullOrEmpty(values[0]))
                    {
                        var resultObj = new GameResult
                        {
                            GameDate = DateTime.Parse(values[0], format),
                            TeamName = values[1],
                            HomeOrAway = (HomeOrAway)Enum.Parse(typeof(HomeOrAway), values[2]), // Enum.Parse() returns System.Object and must be cast
                            Goals = int.Parse(values[3]),
                            GoalAttempts = int.Parse(values[4]),
                            ShotsOnGoal = int.Parse(values[5]),
                            ShotsOffGoal = int.Parse(values[6]),
                            PosessionPercent = double.Parse(values[7])
                        };
                        result.Add(resultObj);
                    }
                }
            }

            return result;
        }

        private static List<Player> DeserializePlayers(string fileName)
        {
            //var players = new List<Player>();
            using (var reader = new StreamReader(fileName))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    var players = serializer.Deserialize<List<Player>>(jsonReader);
                    return players;
                }
            }
        }

        static void SerializePlayers(List<Player> players, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, players);
                }
            }
        }

        static List<Player> GetTopTenPlayers(List<Player> players)
        {
            Comparison<Player> playerComparison = delegate (Player x, Player y)
                {
                    return -x.PointsPerGame.CompareTo(y.PointsPerGame);// x.PointsPerGame < y.PointsPerGame ? 1 : x.PointsPerGame == y.PointsPerGame ? 0 : -1;
                };
            players.Sort(comparison: playerComparison);
            return players.Take(10).ToList();
        }

        static string GetWebPage(string pageUri)
        {
            using (var webClient = new WebClient())
            {
                //webClient.Headers.Add(HttpRequestHeader.Accept, ...);
                byte[] pageBytes = webClient.DownloadData(pageUri);
                using (var stream = new MemoryStream(pageBytes))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        const string accessKey = "2e99004754f94d6b8fd9cbfb8665a00b"; // Key 2:fef3dc518b114786b79aa758499dd4da
        const string uriBase = "https://api.cognitive.microsoft.com/bing/v7.0/news/search";
        static List<NewsResult> BingNewsSearch(string playerName)
        {
            var escaped = Uri.EscapeDataString(playerName);
            string url = string.Format("{0}?q={1}", uriBase, escaped); // &mkt=en-us
            var request = WebRequest.Create(url);
            request.Headers["Ocp-Apim-Subscription-Key"] = accessKey;
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    var newsSearch = DeserializeNews(stream);
                    return newsSearch.NewsResults;
                }
            }

            //// Create result object for return
            //var searchResult = new SearchResult()
            //{
            //    jsonResult = json,
            //    relevantHeaders = new Dictionary<String, String>()
            //};

            //// Extract Bing HTTP headers
            //foreach (String header in response.Headers)
            //{
            //    if (header.StartsWith("BingAPIs-") || header.StartsWith("X-MSEdge-"))
            //        searchResult.relevantHeaders[header] = response.Headers[header];
            //}
        }

        private static NewsSearch DeserializeNews(Stream newsStream)
        {
            using (var reader = new StreamReader(newsStream))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    var result = serializer.Deserialize<NewsSearch>(jsonReader);
                    return result;
                }
            }
        }

        const string textAnalyticsEndpoint = "https://northeurope.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        const string textAnalyticsKey = "ba133a4e86f545d5b7cb6389394738b6"; // key 2: 53934b48287d4a9bba9d08b52e06043e

        static SentimentResponse GetSentimentResponse(List<NewsResult> newsResults)
        {
            var sentimentRequest = new SentimentRequest
            {
                Documents = new List<Document>()
            };
            foreach (var newsResult in newsResults)
            {
                sentimentRequest.Documents.Add(new Document
                {
                    Id = newsResult.Headline,
                    Text = newsResult.Summary,
                    Language = "en"
                });
            }

            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Ocp-Apim-Subscription-Key", textAnalyticsKey);
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
                var requestJson = JsonConvert.SerializeObject(sentimentRequest);
                var requestBytes = Encoding.UTF8.GetBytes(requestJson);
                var responseBytes = webClient.UploadData(textAnalyticsEndpoint, requestBytes);
                var responseJson = Encoding.UTF8.GetString(responseBytes);
                var result = JsonConvert.DeserializeObject<SentimentResponse>(responseJson);
                return result;
            }
        }
    }
}
