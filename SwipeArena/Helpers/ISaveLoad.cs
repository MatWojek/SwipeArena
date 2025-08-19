using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwipeArena.Helpers
{
    /// <summary>
    /// Interfejs do zapisu postępu w grze
    /// </summary>
    public interface ISaveLoad
    {
        void Load();
        void Save();
    }
}
