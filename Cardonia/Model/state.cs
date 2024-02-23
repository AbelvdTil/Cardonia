using Cardonia.Model.Table;

namespace Cardonia.Model
{
    public sealed class State
    {
        private static readonly Lazy<State> lazy = new Lazy<State>(() => new State());

        public static State Instance { get { return lazy.Value; } }

        public TableManager TableManager { get; private set; }

        private State()
        {
            TableManager = new TableManager();
        }
    }
}
