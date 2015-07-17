using System;
using System.Xml;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace Linn
{
    public class VersionSupport
    {

        public static int CompareVersions(string aVersion, string aCompareVersion)
        {
            uint family = FamilyNumber(aVersion);
            uint family2 = FamilyNumber(aCompareVersion);
            if (family != family2)
            {
                return family < family2 ? -1 : 1;
            }
            else
            {
                uint major = Release(aVersion);
                uint major2 = Release(aCompareVersion);
                if (major < major2)
                {
                    return -1;
                }
                else if (major > major2)
                {
                    return 1;
                }
            }
            return Build(aVersion).CompareTo(Build(aCompareVersion));
        }

        public static int ComparePartialVersions(string aVersionA, string aVersionB)
        {
            IEnumerable<string> numbersA = aVersionA.Split('.');
            IEnumerable<string> numbersB = aVersionB.Split('.');

            if (numbersA.Count() > numbersB.Count())
            {
                numbersB = numbersB.Concat(Enumerable.Repeat("0", numbersA.Count() - numbersB.Count()));
            }
            else if (numbersB.Count() > numbersA.Count())
            {
                numbersA = numbersA.Concat(Enumerable.Repeat("0", numbersB.Count() - numbersA.Count()));
            }

            IEnumerable<int> intsA = numbersA.Select(s => int.Parse(s, CultureInfo.InvariantCulture));
            IEnumerable<int> intsB = numbersB.Select(s => int.Parse(s, CultureInfo.InvariantCulture));

            int numInts = intsA.Count();

            for (int i = 0; i < numInts; i++)
            {
                int a = intsA.ElementAt(i);
                int b = intsB.ElementAt(i);
                if (a > b)
                {
                    return 1;
                }
                else if (a < b)
                {
                    return -1;
                }
            }

            return 0;
        }

        public static int CompareProxyVersions(string aVersion, string aCompareVersion) {
            string currVersion = aCompareVersion.ToLower(CultureInfo.InvariantCulture);
            if (currVersion.StartsWith("sp")) {
                currVersion = currVersion.Remove(0, currVersion.IndexOf("v") + 1);
            }
            else if (currVersion.StartsWith("s")) {
                currVersion = currVersion.Remove(0, 1);
            }
            string latestVersion = aVersion.ToLower(CultureInfo.InvariantCulture);
            if (latestVersion.StartsWith("sp")) {
                latestVersion = latestVersion.Remove(0, currVersion.IndexOf("v") + 1);
            }
            else if (latestVersion.StartsWith("s")) {
                latestVersion = latestVersion.Remove(0, 1);
            }
            try {
                uint current = uint.Parse(currVersion, System.Globalization.CultureInfo.InvariantCulture);
                uint latest = uint.Parse(latestVersion, System.Globalization.CultureInfo.InvariantCulture);
                if (current < latest) {
                    return 1;
                }
            }
            catch(FormatException) { }
            return -1;
        }

        public static VersionSupport Instance
        {
            get
            {
                if (iInstance == null)
                {
                    iInstance = new VersionSupport();
                }
                return iInstance;
            }
        }

        public static string Family(string aSoftwareVersion)
        {
            string[] split = aSoftwareVersion.Split('.');
            if (split.Length > 0)
            {
                int index = int.Parse(split[0], System.Globalization.CultureInfo.InvariantCulture) - 1;
                if (index >= 0 && index < VersionSupport.Instance.kFamilyNames.Length)
                {
                    return VersionSupport.Instance.kFamilyNames[index];
                }
            }
            return kFamilyUnknown;
        }

        public static uint FamilyNumber(string aSoftwareVersion)
        {
            string[] split = aSoftwareVersion.Split('.');
            if (split.Length > 0)
            {
                return uint.Parse(split[0], System.Globalization.CultureInfo.InvariantCulture);
            }
            return 0;
        }

        public static uint Release(string aSoftwareVersion)
        {
            string[] split = aSoftwareVersion.Split('.');
            if (split.Length > 1)
            {
                return uint.Parse(split[1], System.Globalization.CultureInfo.InvariantCulture);
            }
            return 0;
        }

        public static uint Build(string aSoftwareVersion)
        {
            string[] split = aSoftwareVersion.Split('.');
            if (split.Length > 2)
            {
                return uint.Parse(split[2], System.Globalization.CultureInfo.InvariantCulture);
            }
            return 0;
        }

        public static string SoftwareVersionPretty(string aSoftwareVersion, bool aIncludeFull)
        {
            if (aSoftwareVersion != null && aSoftwareVersion.Contains(".") && !aSoftwareVersion.Contains("-"))
            {
                string result = Family(aSoftwareVersion) + " " + Release(aSoftwareVersion);
                if (Release(aSoftwareVersion) == 0)
                {
                    result = Family(aSoftwareVersion) + " Nightly Build (" + aSoftwareVersion + ")";
                }
                else if (aIncludeFull)
                {
                    result += " (" + aSoftwareVersion + ")";
                }
                return result;
            }
            else
            {
                return aSoftwareVersion;
            }
        }

        // new software naming convention: Family.Version.BuildNumber (3.2.37 = Cara 2 build 37)
        public readonly string[] kFamilyNames = new string[] { "Auskerry", "Bute", "Cara", "Davaar", "Eriska" };
        public const string kFamilyUnknown = "Unknown";

        static private VersionSupport iInstance = null;
    }


    public class VersionComparer : IComparer<string>
    {
        public int Compare(string aVersionA, string aVersionB)
        {
            return VersionSupport.ComparePartialVersions(aVersionA, aVersionB);
        }
    }

} // Linn