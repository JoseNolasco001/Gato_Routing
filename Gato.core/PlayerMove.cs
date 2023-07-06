using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace Gato.core
{
    public class PlayerMove : MarshalByRefObject, IPlayerMove
    {
        private TcpServerChannel serverChannel;
        private ObjRef internalRef;
        private bool serverActive = false;
        private static int tcpPort = 9096;
        private static string serverUri = "ManagerAccount";
        private int [] Gato = new int[9];
        public event EventHandler<PlayerArgs> changeAmount;
        private PlayerArgs[] users = new PlayerArgs[2];
        private int numUsuarios = 0; 
        private int turno = 0;

        public void Initialize()
        {
            if (serverActive)
                return;

            Hashtable props = new Hashtable();
            props["port"] = tcpPort;
            props["name"] = serverUri;

            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            serverChannel = new TcpServerChannel(props, serverProv);

            try
            {
                ChannelServices.RegisterChannel(serverChannel, false);
                internalRef = RemotingServices.Marshal(this, props["name"].ToString());
                serverActive = true;
                Console.WriteLine("Manager initialized...");
                Console.WriteLine($"In tcp://localhost:{tcpPort}/{serverUri}");
                InicializarGato();
            }
            catch (RemotingException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error could not start the server {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error could not start the server {ex.Message}");
            }
        }

        public void Uninitialize()
        {
            if (!serverActive) return;

            RemotingServices.Unmarshal(internalRef);

            try
            {
                ChannelServices.UnregisterChannel(serverChannel);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error to uninitialize {ex.Message}");
            }
        }

        public void InicializarGato()
        {
            for(int i = 0; i < 9; i++)
            {
                Gato[i] = -1;
            }
        }

        public int[] GetGato()
        {
            return Gato;
        }

        public void movimiento(int posicion, int turno)
        {
            Gato[posicion] = turno;
            changeAmount?.Invoke(this, new PlayerArgs() {Name = users[turno].Name, posicion = posicion, turno = turno });
            this.turno++;
            this.turno = this.turno % 2;
        }

        public int RegistrarUsuario(int id, string name)
        {
            if(numUsuarios<2) {
                users[numUsuarios] = new PlayerArgs() { Id = id, Name = name, posicion = -1, turno = numUsuarios };
                changeAmount?.Invoke(this, users[numUsuarios]);
                Console.WriteLine($"Se conecto el usuario {name} con turno {numUsuarios}");
                return numUsuarios++;
            }
            return -1;
        }

        public int GetTurno()
        {
            return this.turno;
        }

        public int Winner(int turno)
        {
            if (Gato[0] == turno && Gato[1] == turno && Gato[2] == turno)
            {
                return turno;
            }
            else if (Gato[3] == turno && Gato[4] == turno && Gato[5] == turno)
            {
                return turno;
            }
            else if (Gato[6] == turno && Gato[7] == turno && Gato[8] == turno)
            {
                return turno;
            }
            else if (Gato[0] == turno && Gato[3] == turno && Gato[6] == turno)
            {
                return turno;
            }
            else if (Gato[1] == turno && Gato[4] == turno && Gato[7] == turno)
            {
                return turno;
            }
            else if (Gato[2] == turno && Gato[5] == turno && Gato[8] == turno)
            {
                return turno;
            }
            else if (Gato[0] == turno && Gato[4] == turno && Gato[8] == turno)
            {
                return turno;
            }
            else if (Gato[2] == turno && Gato[4] == turno && Gato[6] == turno)
            {
                return turno;
            }
            else
            {
                return -1;
            }
        }
    }
}
