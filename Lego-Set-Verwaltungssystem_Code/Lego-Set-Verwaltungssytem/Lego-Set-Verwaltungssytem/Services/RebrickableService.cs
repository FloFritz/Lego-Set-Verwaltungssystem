using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Lego_Set_Verwaltungssytem.Models;
using System.IO;


// Enthält alle Methoden für den Zugriff auf die Rebrickable API
namespace Lego_Set_Verwaltungssytem.Services
{
    public static class RebrickableService
    {
        // API-Zugangsdaten
        // Statt static readonly nur so:
        private static string _apiKey;

        public static string ApiKey
        {
            get
            {
                if (_apiKey == null)
                    _apiKey = LadeApiKey();
                return _apiKey;
            }
            set
            {
                _apiKey = value;
            }
        }

        private static string LadeApiKey()
        {
            string path = "api.txt";
            return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
        }

        // HttpClient wird einmalig erzeugt (Header dynamisch gesetzt)
        private static readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://rebrickable.com/api/v3/lego/")
        };

        // Mapping von Theme-IDs zu Namen
        private static Dictionary<int, string> themeMapping = new Dictionary<int, string>();

        // Suche nach Sets über die Freitextsuche
        public static async Task<List<LegoSet>> SucheSetsAsync(string suchbegriff, string suchtyp)
        {
            var sets = new List<LegoSet>();

            // API-Key Header setzen
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", ApiKey);

            if (themeMapping.Count == 0)
            {
                await LadeThemesAsync();
            }

            string query = $"sets/?search={suchbegriff}";
            HttpResponseMessage response = await client.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonSerializer.Deserialize<RebrickableApiResult>(jsonString);

                if (apiResult != null && apiResult.results != null)
                {
                    foreach (var apiSet in apiResult.results)
                    {
                        string themenName = themeMapping.ContainsKey(apiSet.theme_id) ? themeMapping[apiSet.theme_id] : "Unbekanntes Thema";

                        sets.Add(new LegoSet
                        {
                            Nummer = apiSet.set_num,
                            Name = apiSet.name,
                            Thema = themenName,
                            Jahr = apiSet.year,
                            PreisUVP = 0.0
                        });
                    }
                }
            }
            else
            {
                var fehler = await response.Content.ReadAsStringAsync();
                throw new Exception($"API Anfrage fehlgeschlagen. Code: {response.StatusCode}, Inhalt: {fehler}");
            }

            return sets;
        }

        // Lädt den UVP-Preis für ein bestimmtes Set
        public static async Task<double> LadePreisVonSetAsync(string setNummer)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", ApiKey);

            string query = $"sets/{setNummer}/";
            HttpResponseMessage response = await client.GetAsync(query);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var detail = JsonSerializer.Deserialize<RebrickableSetDetails>(jsonString);

                if (detail != null && detail.set_num != null)
                {
                    return detail.set_price_usd ?? 0.0;
                }
            }

            return 0.0;
        }

        // Modellklasse für Set-Details
        public class RebrickableSetDetails
        {
            public string set_num { get; set; }
            public double? set_price_usd { get; set; }
        }

        // Lädt alle Themenamen (mehrseitig) und gibt sortierte Liste zurück
        public static async Task<List<string>> LadeAlleThemenNamenAsync()
        {
            List<string> themenNamen = new List<string>();
            int page = 1;
            int pageSize = 1000;

            while (true)
            {
                string url = $"themes/?page={page}&page_size={pageSize}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", ApiKey);
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    break;

                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonSerializer.Deserialize<RebrickableThemesResult>(jsonString);

                if (apiResult?.results == null || apiResult.results.Count == 0)
                    break;

                foreach (var theme in apiResult.results)
                {
                    themenNamen.Add(theme.name);
                }

                if (apiResult.results.Count < pageSize)
                    break;

                page++;
            }

            return themenNamen.Distinct().OrderBy(t => t).ToList();
        }

        // Lädt alle Theme-IDs und speichert sie im Dictionary
        private static async Task LadeThemesAsync()
        {
            themeMapping.Clear();

            int page = 1;
            int pageSize = 1000;

            while (true)
            {
                string url = $"themes/?page={page}&page_size={pageSize}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", ApiKey);
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    break;

                var jsonString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonSerializer.Deserialize<RebrickableThemesResult>(jsonString);

                if (apiResult?.results == null || apiResult.results.Count == 0)
                    break;

                foreach (var theme in apiResult.results)
                {
                    themeMapping[theme.id] = theme.name;
                }

                if (apiResult.results.Count < pageSize)
                    break;

                page++;
            }
        }

        // Lädt alle Sets zu einer bestimmten Theme-ID
        public static async Task<List<LegoSet>> SucheAlleSetsNachThemaAsync(string themaName)
        {
            var sets = new List<LegoSet>();
            int page = 1;

            if (themeMapping.Count == 0)
            {
                await LadeThemesAsync();
            }

            int? themaId = await HoleThemaIdAsync(themaName);

            if (themaId == null)
            {
                throw new Exception("Thema wurde nicht gefunden.");
            }

            while (true)
            {
                string query = $"sets/?theme_id={themaId}&page={page}&page_size=100";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", ApiKey);
                HttpResponseMessage response = await client.GetAsync(query);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    break;
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var apiResult = JsonSerializer.Deserialize<RebrickableApiResult>(jsonString);

                    if (apiResult != null && apiResult.results != null && apiResult.results.Count > 0)
                    {
                        foreach (var apiSet in apiResult.results)
                        {
                            string themenName = themeMapping.ContainsKey(apiSet.theme_id) ? themeMapping[apiSet.theme_id] : "Unbekanntes Thema";

                            sets.Add(new LegoSet
                            {
                                Nummer = apiSet.set_num,
                                Name = apiSet.name,
                                Thema = themenName,
                                Jahr = apiSet.year,
                                PreisUVP = 0.0
                            });
                        }
                        page++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var fehler = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Anfrage fehlgeschlagen. Code: {response.StatusCode}, Inhalt: {fehler}");
                }
            }

            return sets;
        }

        // Sucht die ID eines Themas anhand des Namens
        public static async Task<int?> HoleThemaIdAsync(string themaName)
        {
            if (themeMapping.Count == 0)
            {
                await LadeThemesAsync();
            }

            foreach (var entry in themeMapping)
            {
                if (entry.Value.Equals(themaName, StringComparison.OrdinalIgnoreCase))
                {
                    return entry.Key;
                }
            }

            return null;
        }
    }

    // Datenmodell für Setliste aus API
    public class RebrickableApiResult
    {
        public List<RebrickableSet> results { get; set; }
    }

    // Datenmodell für einzelne Sets aus API
    public class RebrickableSet
    {
        public string set_num { get; set; }
        public string name { get; set; }
        public int year { get; set; }
        public int theme_id { get; set; }
        public bool moc { get; set; }
    }

    // Datenmodell für Themenliste aus API
    public class RebrickableThemesResult
    {
        public List<RebrickableTheme> results { get; set; }
    }

    // Datenmodell für einzelnes Theme
    public class RebrickableTheme
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? parent_id { get; set; }
    }
}
