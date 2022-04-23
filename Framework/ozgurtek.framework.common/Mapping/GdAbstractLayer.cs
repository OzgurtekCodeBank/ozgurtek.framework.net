using ozgurtek.framework.core.Mapping;

namespace ozgurtek.framework.common.Mapping
{
    public abstract class GdAbstractLayer : IGdLayer
    {
        protected string _name;
        protected IGdRenderer _renderer;

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual IGdRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
