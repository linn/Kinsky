using System;
using System.IO;
using System.Collections.Generic;

using Upnp;

namespace Linn.Kinsky
{
    public interface IMediaRetriever
    {
        DidlLite DragMedia { get; }
        DidlLite Media { get; }
    }

    public class MediaRetrieverException : ApplicationException
    {
        public MediaRetrieverException() { }
    }

    public class MediaRetrieverNoRetrieve : IMediaRetriever
    {
        public MediaRetrieverNoRetrieve(IList<upnpObject> aList)
        {
            iDidlLite = new DidlLite();
            iDidlLite.AddRange(aList);
        }

        public DidlLite DragMedia
        {
            get
            {
                return iDidlLite;
            }
        }

        public DidlLite Media
        {
            get
            {
                DidlLite didl = new DidlLite();

                // expand out playlist containers
                foreach (upnpObject o in iDidlLite)
                {
                    if (o is playlistContainer)
                    {
                        if (o.Res.Count == 1)
                        {
                            if (Path.GetExtension(o.Res[0].Uri) == Playlist.kPlaylistExtension)
                            {
                                Playlist p = new Playlist();
                                p.Load(o.Res[0].Uri);
                                didl.AddRange(p.Tracks);
                            }
                        }
                    }
                    else
                    {
                        didl.Add(o);
                    }
                }

                return didl;
            }
        }

        private DidlLite iDidlLite;
    }

    public class MediaRetriever : IMediaRetriever
    {
        public MediaRetriever(IContainer aContainer, IList<upnpObject> aList)
        {
            iContainer = aContainer;
            iDidlLite = new DidlLite();
            iDidlLite.AddRange(aList);
        }

        public DidlLite DragMedia
        {
            get
            {
                return iDidlLite;
            }
        }

        public DidlLite Media
        {
            get
            {
                DidlLite result = new DidlLite();

                try
                {
                    foreach (upnpObject o in iDidlLite)
                    {
                        if (o is item)
                        {
                            result.Add(o);
                        }
                        else
                        {
                            IContainer c = iContainer.ChildContainer(o as container);

                            result.AddRange(GetItems(c));
                        }
                    }
                }
                catch (MediaRetrieverException)
                {
                    result.Clear();
                }

                Trace.WriteLine(Trace.kKinsky, "MediaRetriever.Media: result.Count=" + result.Count);

                return result;
            }
        }

        private IList<upnpObject> GetItems(IContainer aContainer)
        {
            try
            {
                uint count = aContainer.Open();
    
                List<upnpObject> result = new List<upnpObject>();
                DidlLite didl = new DidlLite();
    
                uint index = 0;
                while (index < count)
                {
                    DidlLite d = aContainer.Items(index, kCountPerCall);
                    if (d.Count == 0)
                    {
                        UserLog.WriteLine("Error retrieving items from " + aContainer.Metadata.Title);
                        throw new MediaRetrieverException();
                    }
                    didl.AddRange(d);
    
                    index += (uint)d.Count;
                }
    
                foreach (upnpObject o in didl)
                {
                    if (o is container)
                    {
                        // hack for twonky servers - don't recurse into the "- ALL -" container - if someone changes its default name tough
                        if (o.Title != "- ALL -")
                        {
                            IContainer c = aContainer.ChildContainer(o as container);
                            result.AddRange(GetItems(c));
                        }
                    }
                    else
                    {
                        result.Add(o);
                    }
                }
    
                return result;
            }
            catch(HttpServerException) 
            {
                throw new MediaRetrieverException();
            }
        }

        private const uint kCountPerCall = 100;

        private IContainer iContainer;
        private DidlLite iDidlLite;
    }
}