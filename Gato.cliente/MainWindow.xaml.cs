using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gato.core;
using System.Runtime.Remoting.Channels.Tcp;

namespace Gato.cliente
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static IPlayerMove accountMove;
        static TcpChannel channel;
        static RemoteEvent<PlayerArgs> changeAmount;
        static BinaryClientFormatterSinkProvider clientProv;
        static BinaryServerFormatterSinkProvider serverProv;
        private static string serverUri = "tcp://localhost:9096/ManagerAccount";
        private static bool connected = false;
        private Random _random = new Random();
        private static MainWindow instance = new MainWindow();
        private int turno = 0;
        private static bool endGame=true;

        public static MainWindow Instance { get { return instance; } }

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }

        private void Button_Click_Connect(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameUser.Text)) 
            {
                MessageBox.Show($"Ingrese un nombre en la caja de texto", "Error");
                return;
            }
            clientProv = new BinaryClientFormatterSinkProvider();
            serverProv = new BinaryServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            //Manejo una clase que encapsula un evento para poder integrar logs de disparo o error
            changeAmount = new RemoteEvent<PlayerArgs>();
            changeAmount.eventToHandle += new EventHandler<PlayerArgs>(changeAmountEvent);

            Hashtable props = new Hashtable();
            props["name"] = "remotingClient";
            props["port"] = 0;


            channel = new TcpChannel(props, clientProv, serverProv);
            ChannelServices.RegisterChannel(channel);

            RemotingConfiguration.RegisterWellKnownClientType(new WellKnownClientTypeEntry(typeof(IPlayerMove), serverUri));

            if (connected)
                return;

            try
            {
                accountMove = (IPlayerMove)Activator.GetObject(typeof(IPlayerMove), serverUri);
                //Para encapsular el evento en una clase para manejar logs
                accountMove.changeAmount += new EventHandler<PlayerArgs>(changeAmount.Notify);
                //accountMove.changeAmount += changeAmountEvent;
                turno = accountMove.RegistrarUsuario(_random.Next(), NameUser.Text);

                connected = true;
                endGame = false;
                labelConect.Content = "Conectado";
                accountMove.movimiento(-1,turno);
            }
            catch (Exception ex)
            {
                connected = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Could not connect {ex.Message}");
            }

            Console.ReadLine();
            Disconnect();
        }

        private void Button_Click_Disconnect(object sender, RoutedEventArgs e)
        {
            if (!Disconnect())
            {
                connected = false;
                labelConect.Content = "Desconectado";
            }
        }

        private static bool Disconnect()
        {
            if (!connected) return true;
            accountMove.changeAmount -= new EventHandler<PlayerArgs>(changeAmount.Notify);
            ChannelServices.UnregisterChannel(channel);
            return false;
        }

        private static void changeAmountEvent(object sender, PlayerArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            instance.Draw(e.posicion, e.turno == 0 ? Brushes.Red : Brushes.Blue);
            int ganador = accountMove.Winner(e.turno);
            
            if(ganador != -1)
            {
                endGame = true;
                MessageBox.Show($"El ganador es {e.Name}", "Error");
                Disconnect();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (endGame)
                return;
            Button btn = (Button)sender;
            string contenido = btn.Name;
            int posicion = Int32.Parse(contenido.Substring(3));

            if (accountMove.GetTurno() == turno)
            {
                if (ValidarTiro(posicion))
                {
                    accountMove.movimiento(posicion, turno);
                }
                else
                {
                    MessageBox.Show($"Error: La casilla {posicion} ya fue seleccionada", "Error");
                }
            }
            else
            {
                MessageBox.Show("Error: No es tu turno", "Error");
            }
        }

        private void Draw(int posicion, SolidColorBrush color)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                switch (posicion)
                {
                    case 0:
                        btn0.Background = color;
                        break;
                    case 1:
                        btn1.Background = color;
                        break;
                    case 2:
                        btn2.Background = color;
                        break;
                    case 3:
                        btn3.Background = color;
                        break;
                    case 4:
                        btn4.Background = color;
                        break;
                    case 5:
                        btn5.Background = color;
                        break;
                    case 6:
                        btn6.Background = color;
                        break;
                    case 7:
                        btn7.Background = color;
                        break;
                    case 8:
                        btn8.Background = color;
                        break;
                    default: break;
                }
            }));
        }

        private bool ValidarTiro(int posicion)
        {
            int[] mov = accountMove.GetGato();

            if (mov[posicion] != -1) 
                return false;
            return true;
        }
    }
}
