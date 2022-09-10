
using ABI_RC.VideoPlayer;
using ABI_RC.VideoPlayer.Scripts;
using HarmonyLib;
using MelonLoader;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CVRBilibiliVideoResolver
{
    public static class BuildInfo
    {
        public const string Name = "CVRBilibiliVideoResolver"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Resolve video link from bilibili.com"; // Description for the Mod.  (Set as null if none)
        public const string Author = "YueLuo"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class CVRBilibiliVideoResolver : MelonMod
    {
        private static MelonLogger.Instance logger;

        public override void OnApplicationStart() // Runs after Game Initialization.
        {
            logger = LoggerInstance;

            AddHostToUrlWhiteList();

            HarmonyInstance.Patch(typeof(YoutubeDl).GetMethod(nameof(YoutubeDl.GetVideoMetaDataAsync)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(CVRBilibiliVideoResolver),
                    nameof(BeforeGetVideoMetaDataAsync))));

        }

        public static void AddHostToUrlWhiteList()
        {
            UrlWhitelist.AddToWhitelist("*.bilibili.com");
            // short link
            UrlWhitelist.AddToWhitelist("*.b23.tv");
        }

        public static string ResolveVideo(string url)
        {
            try
            {
                var source = GetWebPageSource(url);

                if (!string.IsNullOrWhiteSpace(source))
                {
                    // logger.Msg(source);

                    const string pattern = @"{""p"":(?:\d+),(?:.*?)""aid"":(\d+),""bvid"":""(?:.*?)"",""cid"":(\d+)";
                    var match = Regex.Match(source, pattern, RegexOptions.Multiline);
                    if (match.Success)
                    {
                        var aid = match.Groups[1].Value;
                        var cid = match.Groups[2].Value;

                        if (!string.IsNullOrWhiteSpace(cid)) return GetVideoUrl(aid, cid);

                        logger.Error("video info missing important parameter cid!");
                        return null;

                    }
                    else
                    {
                        logger.Error("failed to fetch video info!");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("error occurred when resolve video url!", e);
            }

            return null;
        }

        private static string GetWebPageSource(string url)
        {
            using var client = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
            using var response = client.GetAsync(url).Result;
            using var content = response.Content;
            var source = content.ReadAsStringAsync().Result;
            return source;
        }

        private static string GetVideoUrl(string aid, string cid)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("GET"),
                $"http://api.bilibili.com/x/player/playurl?avid={aid}&cid={cid}&qn=64&fnval=1&fnver=0&fourk=1&platform=html5&high_quality=1");
            var response = httpClient.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            var jobject = JObject.Parse(responseBody);
            var videoUrl = jobject["data"]?["durl"]?[0]?["url"]?.ToString();

            logger.Msg($"resolve successful! url: {videoUrl}");

            return videoUrl;
        }

        public static bool BeforeGetVideoMetaDataAsync(ref Task<YoutubeDl.ProcessResult> __result, ref string youtubeUrl,
            ref string parameter)
        {
            if (!youtubeUrl.Contains("bilibili.com") && !youtubeUrl.Contains("b23.tv")) return true;

            var tcs = new TaskCompletionSource<YoutubeDl.ProcessResult>();
            var result = default(YoutubeDl.ProcessResult);

            var url = ResolveVideo(youtubeUrl);
            if (url != null)
            {
                var meta = new YoutubeDlVideoMetaData
                {
                    Url = url
                };

                result.Completed = true;
                result.ExitCode = 0;
                result.Output = meta;
            }
            else
            {
                result.Completed = true;
                result.ExitCode = -1;
                result.Error = "Failed to resolve! Check console for further info!";
            }

            tcs.TrySetResult(result);
            __result = tcs.Task;
            return false;

        }
    }

}