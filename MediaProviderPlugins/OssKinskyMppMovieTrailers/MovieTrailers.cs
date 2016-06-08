using System;
using System.Reflection;
using System.IO;

using Linn;
using Linn.Kinsky;

using Upnp;

[assembly: ContentDirectoryFactoryType("OssKinskyMppMovieTrailers.MediaProviderMovieTrailersFactory")]

namespace OssKinskyMppMovieTrailers
{
    public class MediaProviderMovieTrailersFactory : IContentDirectoryFactory
    {
        public IContentDirectory Create(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            return new ContentDirectoryMovieTrailers(aDataPath, aSupport);
        }
    }

    public class ContentDirectoryMovieTrailers : IContentDirectory
    {
        public ContentDirectoryMovieTrailers(string aDataPath, IContentDirectorySupportV2 aSupport)
        {
            string installPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            iWebFetcher = new WebFetcher(aDataPath);

            iPodcast = new Podcast(iWebFetcher, installPath, aSupport);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public string Name
        {
            get
            {
                return "Movie Trailers";
            }
        }

        public string Company
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length != 0)
                {
                    return ((AssemblyCompanyAttribute)attributes[0]).Company;
                }
                else
                {
                    return "";
                }
            }
        }

        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public IContainer Root
        {
            get
            {
                return iPodcast;
            }
        }

        public IOptionPage OptionPage
        {
            get
            {
                return null;
            }
        }

        private WebFetcher iWebFetcher;
        private Podcast iPodcast;
    }
}
