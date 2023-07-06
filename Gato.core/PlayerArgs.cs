using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gato.core
{
    [Serializable]
    public class PlayerArgs : EventArgs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int turno { get; set; }
        public int posicion { get; set; }
    }
}
