
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;


namespace Linn
{
    [Flags]
    public enum EReleaseQuality
    {
        Developer = 0x01,
        Nightly = 0x02,
        Development = 0x04,
        Beta = 0x08,
        Stable = 0x10
    }

    public class ReleaseFeed
    {
        public ReleaseFeed(Stream aStream)
        {
            using (StreamReader reader = new StreamReader(aStream))
            {
                string json = reader.ReadToEnd();
                Json = JsonConvert.DeserializeObject<ReleaseFeedJson>(json);
            }
        }

        public IReleaseFeedJson Json { get; private set; }

        public IReleaseJson CheckForUpdate(string aCurrentVersion, EReleaseQuality aCurrentQuality, string aCurrentPlatform, string aCurrentPlatformVersion, EReleaseQuality aDesiredQuality)
        {
            // select all releases that
            // - are for this platform
            // - have a min platform version that is less than or equal to this platform version
            // - have a quality that matches the desired quality
            var releases = Json.Releases.Where(r => r.Platform == aCurrentPlatform)
                                        .Where(r => VersionSupport.ComparePartialVersions(aCurrentPlatformVersion, r.PlatformMinVersion) >= 0)
                                        .Where(r => (r.Quality() & aDesiredQuality) == r.Quality());

            // select the release with the highest version for each release quality
            // - group by quality
            // - order releases in each group by ascending version
            // - select the last release in each group
            releases = releases.GroupBy(r => r.Quality())
                               .Select(g => g.OrderBy(r => r.Version, new VersionComparer()).Last());

            // pick the release with the highest version number and the highest quality (priority to version number)
            var release = releases.OrderBy(r => r.Version, new VersionComparer())
                                  .ThenBy(r => r.Quality())
                                  .LastOrDefault();

            if (release == null)
            {
                return null;
            }

            // this version is a valid update if
            // - its version is greater than the current app version
            // - its version is the same as the current app version but its quality is greater than the current app quality
            // - its version is less than the current app version and the current app quality is not in the desired quality
            if (VersionSupport.ComparePartialVersions(release.Version, aCurrentVersion) > 0 ||
                (VersionSupport.ComparePartialVersions(release.Version, aCurrentVersion) == 0 && release.Quality() > aCurrentQuality) ||
                (VersionSupport.ComparePartialVersions(release.Version, aCurrentVersion) < 0 && (aCurrentQuality & aDesiredQuality) != aCurrentQuality))
            {
                return release;
            }

            return null;
        }
    }

    // interfaces & extensions for the json data

    public interface IReleaseFeedJson
    {
        string Version { get; }
        string Application { get; }
        string History { get; }
        IReleaseJson[] Releases { get; }
    }

    public interface IReleaseJson
    {
        string Date { get; }
        string Version { get; }
        string Quality { get; }
        string Platform { get; }
        string PlatformMinVersion { get; }
        string DownloadUri { get; }
        string UpdateUri { get; }
        string[] Notes { get; }
    }

    public static class ReleaseFeedJsonExtensions
    {
        public const string kDateFormat = "d MMM yyyy H:mm:ss";

        public static DateTime Date(this IReleaseJson aJson)
        {
            return DateTime.ParseExact(aJson.Date, kDateFormat, CultureInfo.InvariantCulture);
        }

        public static Uri History(this IReleaseFeedJson aJson)
        {
            return new Uri(aJson.History);
        }

        public static Uri DownloadUri(this IReleaseJson aJson)
        {
            return new Uri(aJson.DownloadUri);
        }

        public static Uri UpdateUri(this IReleaseJson aJson)
        {
            return new Uri(aJson.UpdateUri);
        }

        public static EReleaseQuality Quality(this IReleaseJson aJson)
        {
            switch (aJson.Quality)
            {
                case "developer":
                    return EReleaseQuality.Developer;
                case "nightly":
                    return EReleaseQuality.Nightly;
                case "development":
                    return EReleaseQuality.Development;
                case "beta":
                    return EReleaseQuality.Beta;
                case "stable":
                    return EReleaseQuality.Stable;
                default:
                    throw new NotImplementedException();
            }
        }
    }


    // classes for the JSON deserialisation

    public class ReleaseFeedJson : IReleaseFeedJson
    {
        public string Version { get; set; }
        public string Application { get; set; }
        public string History { get; set; }
        [JsonConverter(typeof(Converter<IReleaseJson[], ReleaseJson[]>))]
        public IReleaseJson[] Releases { get; set; }
    }

    public class ReleaseJson : IReleaseJson
    {
        public string Date { get; set; }
        public string Version { get; set; }
        public string Quality { get; set; }
        public string Platform { get; set; }
        public string PlatformMinVersion { get; set; }
        public string DownloadUri { get; set; }
        public string UpdateUri { get; set; }
        public string[] Notes { get; set; }
    }

    public class Converter<T, U> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(U));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}


