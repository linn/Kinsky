using System;

namespace Linn.Topology
{
    public sealed class ModelSourceAuxiliary : ModelSource
    {
        public ModelSourceAuxiliary(Source aSource)
        {
            iSource = aSource;
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get
            {
                return (iSource.FullName);
            }
        }

        public override Source Source
        {
            get
            {
                return iSource;
            }
        }

        private Source iSource;
    }

} // Linn.Topology
