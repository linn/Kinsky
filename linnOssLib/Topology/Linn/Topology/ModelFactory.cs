
using System;
using System.Collections.Generic;


namespace Linn.Topology
{
    public interface IModel
    {
        string DeviceXml { get; }
    }

    public interface IModelFactory
    {
        void ClearCache();
        IModelVolumeControl CreateModelVolumeControl(IPreamp aPreamp);
        IModelSourceReceiver CreateModelSourceReceiver(ISource aSource);
        IModelSourceDiscPlayer CreateModelSourceDiscPlayer(ISource aSource);
        IModelSourceMediaRenderer CreateModelSourceMediaRenderer(ISource aSource);
        IModelSourceRadio CreateModelSourceRadio(ISource aSource);
        IModelInfo CreateModelInfo(ISource aSource);
        IModelTime CreateModelTime(ISource aSource);
    }

    public class ModelFactory : IModelFactory
    {
        public const string kSourceUpnpAv = "UpnpAv";
        private Dictionary<string, ModelSource> iModelSourceCache;
        private Dictionary<string, IModelInfo> iModelInfoCache;
        private Dictionary<string, IModelTime> iModelTimeCache;

        public ModelFactory()
        {
            ClearCache();
        }

        private string Id(Source aSource)
        {
            return aSource.Device.Udn + "," + aSource.Device.Location + "," + aSource.Type;
        }

        private T CheckModelCache<T>(Dictionary<string, T> aCache, Source aSource) where T : class, IModel
        {
            T result = null;
            if (aCache.TryGetValue(Id(aSource), out result))
            {
                if (result.DeviceXml != aSource.Device.DeviceXml)
                {
                    aCache.Remove(Id(aSource));
                    result = null;
                }
            }
            return result;
        }

        private void AddToCache<T>(Dictionary<string, T> aCache, T aModel, Source aSource) where T : IModel
        {
            if (aCache.ContainsKey(Id(aSource)))
            {
                aCache[Id(aSource)] = aModel;
            }
            else
            {
                aCache.Add(Id(aSource), aModel);
            }
        }


        public void ClearCache()
        {
            iModelSourceCache = new Dictionary<string, ModelSource>();
            iModelInfoCache = new Dictionary<string, IModelInfo>();
            iModelTimeCache = new Dictionary<string, IModelTime>();
        }

        public IModelVolumeControl CreateModelVolumeControl(IPreamp aPreamp)
        {
            if (aPreamp != null && aPreamp is Preamp)
            {
                if (aPreamp.Type == "Preamp")
                {
                    return (new ModelVolumeControlPreamp(aPreamp as Preamp));
                }
                if (aPreamp.Type == "UpnpAv")
                {
                    return (new ModelVolumeControlUpnpAv(aPreamp as Preamp));
                }
            }

            return (null);
        }

        public IModelSourceReceiver CreateModelSourceReceiver(ISource aSource)
        {
            if (aSource != null && aSource is Source)
            {
                Source source = aSource as Source;
                ModelSourceReceiver result = CheckModelCache(iModelSourceCache, source) as ModelSourceReceiver;
                if (result == null)
                {
                    result = new ModelSourceReceiver(source);
                    AddToCache(iModelSourceCache, result, source);
                }
                return result;
            }
            return null;
        }

        public IModelSourceDiscPlayer CreateModelSourceDiscPlayer(ISource aSource)
        {
            if (aSource != null && aSource is Source)
            {
                Source source = aSource as Source;
                ModelSourceDiscPlayer result = CheckModelCache(iModelSourceCache, source) as ModelSourceDiscPlayer;
                if (result == null)
                {
                    result = new ModelSourceDiscPlayerSdp(source);
                    AddToCache(iModelSourceCache, result, source);
                }
                return result;
            }
            return null;
        }

        public IModelSourceMediaRenderer CreateModelSourceMediaRenderer(ISource aSource)
        {
            if (aSource != null && aSource is Source)
            {
                Source source = aSource as Source;
                ModelSourceMediaRenderer result = CheckModelCache(iModelSourceCache, source) as ModelSourceMediaRenderer;
                if (result == null)
                {
                    if (aSource.Type == kSourceUpnpAv)
                    {
                        result = new ModelSourceMediaRendererUpnpAv(source);
                    }
                    else
                    {
                        result = new ModelSourceMediaRendererDs(source);
                    }
                    AddToCache(iModelSourceCache, result, source);
                }
                return result;
            }
            return null;
        }
        public IModelSourceRadio CreateModelSourceRadio(ISource aSource)
        {
            if (aSource != null && aSource is Source)
            {
                Source source = aSource as Source;
                ModelSourceRadio result = CheckModelCache(iModelSourceCache, source) as ModelSourceRadio;
                if (result == null)
                {
                    result = new ModelSourceRadio(source);
                    AddToCache(iModelSourceCache, result, source);
                }
                return result;
            }
            return null;
        }

        public IModelInfo CreateModelInfo(ISource aSource)
        {            
            if (aSource != null && aSource is Source)
            {
                Source source = aSource as Source;
                if (aSource.Type == kSourceUpnpAv)
                {
                    ModelSourceMediaRendererUpnpAv result = CheckModelCache(iModelSourceCache, source) as ModelSourceMediaRendererUpnpAv;
                    if (result == null)
                    {
                        result = new ModelSourceMediaRendererUpnpAv(source);
                        AddToCache(iModelSourceCache, (result as ModelSource), source);
                    }
                    return result;
                }
                else
                {
                    IModelInfo result = CheckModelCache(iModelInfoCache, source);
                    if (result == null)
                    {
                        result = new ModelInfo(source);
                        AddToCache(iModelInfoCache, result, source);
                    }
                    return result;
                }
            }
            return null;
        }

        public IModelTime CreateModelTime(ISource aSource)
        {
            if (aSource != null && aSource is Source)
            {
                Source source = aSource as Source;
                if (aSource.Type == kSourceUpnpAv)
                {
                    ModelSourceMediaRendererUpnpAv result = CheckModelCache(iModelSourceCache, source) as ModelSourceMediaRendererUpnpAv;
                    if (result == null)
                    {
                        result = new ModelSourceMediaRendererUpnpAv(source);
                        AddToCache(iModelSourceCache, (result as ModelSource), source);
                    }
                    return result;
                }
                else
                {
                    IModelTime result = CheckModelCache(iModelTimeCache, source);
                    if (result == null)
                    {
                        result = new ModelTime(source);
                        AddToCache(iModelTimeCache, result, source);
                    }
                    return result;
                }
            }
            return null;
        }
    }

}
