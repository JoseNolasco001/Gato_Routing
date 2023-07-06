using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gato.core
{
    public interface IPlayerMove
    {
        event EventHandler<PlayerArgs> changeAmount;
        int[] GetGato();
        void movimiento(int posicion, int turno);
        int RegistrarUsuario(int id, string name);
        int GetTurno();
        int Winner(int turno);
    }
}
