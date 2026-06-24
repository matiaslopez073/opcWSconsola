using Sharp7;
using System;
using System.Diagnostics;
using WS_Protocol.Client;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            uint WStag;
            string IpHeuft, IpPLC;
            int port, DbPLC;
            S7Connection PLC = new S7Connection();
            int auxiliar, posicion;

            // Ruta del la aplicacion HEUFT server
             string rutaExe = @"C:\HEUFT\concentrator\HEUFTDataconcentrator.exe";
           // string rutaExe = "notepad.exe";

            try
            {

                // Iniciar el proceso
                // Process.Start(rutaExe);

                if (!System.IO.File.Exists(rutaExe))
                {
                    Console.WriteLine("El archivo no existe.");
                }
                else
                {
                    Process.Start(rutaExe);
                }


                var application = new ProcessStartInfo
                {
                    FileName = rutaExe,
                    WorkingDirectory = @"C:\HEUFT\concentrator",
                    Arguments = "-start",
                  //  Verb = "runas", // ejecutar como admin
                    UseShellExecute = true
                };

                Process.Start(application);


                System.Threading.Thread.Sleep(5000);
                Console.WriteLine("Aplicación HEUFT Data Concentrador iniciada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al iniciar HEUFT Data Concentrador:");
                Console.WriteLine(ex.Message);
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Andina Weihenstepahn Protocol < --- > Siemens S71500");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            //  Console.WriteLine("Ingresar la IP del heuft: ");
            //  IpHeuft = Console.ReadLine();
            IpHeuft = "172.16.210.10";

              port = 50000;
            IpPLC = "181.142.60.183";
            DbPLC = 410;
            System.Threading.Thread.Sleep(2000);

            do
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                var WSclient = new WS_TcpClient(IpHeuft, port) { SendAutomaticHeartBeat = false };
                try
                {

                    //var WSclient = new WS_TcpClient("192.168.0.101", 5000);
                    
/*
                    Console.Write("IP: ");
                    Console.Write(WSclient.HostName);
                    Console.Write("  Puerto:");
                    Console.WriteLine(WSclient.Port);
                    Console.WriteLine();

                    Console.Write("Conectando con HEUFT...          ");
*/
                    WSclient.Connect();

                    /*
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Conexión a HEUFT exitosa");
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Conectando con PLC...          ");
                    */
                    try
                    {
                        PLC.Conect(IpPLC, 0, 1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    PLC.DBnumber = DbPLC;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Conexión con PLC y con HEUFT exitosa");
                    Console.ForegroundColor = ConsoleColor.White;

                   


                    try
                    {
                        

                        //botellas procesadas
                        WStag = 50005;
                        posicion = 0;
                        Console.Write("LeyendoTagID= " + Convert.ToString(WStag) + " : ");
                        auxiliar = WSclient.ReadSingleValueAsInt(WStag);
                        PLC.WriteData(auxiliar, posicion*4);
                        Console.WriteLine(auxiliar);

                        //botellas buenas
                        WStag = 50006;
                        posicion = 1;
                        Console.Write("LeyendoTagID= " + Convert.ToString(WStag) + " : ");
                        auxiliar = WSclient.ReadSingleValueAsInt(WStag);
                        PLC.WriteData(auxiliar, posicion * 4);
                        Console.WriteLine(auxiliar);

                        //botellas rechazadas
                        WStag = 50011;
                        posicion = 2;
                        Console.Write("LeyendoTagID= " + Convert.ToString(WStag) + " : ");
                        auxiliar = WSclient.ReadSingleValueAsInt(WStag);
                        PLC.WriteData(auxiliar, posicion * 4);
                        Console.WriteLine(auxiliar);

                        //numero de programa
                        WStag = 30003;
                        posicion = 3;
                        Console.Write("LeyendoTagID= " + Convert.ToString(WStag) + " : ");
                        auxiliar = WSclient.ReadSingleValueAsInt(WStag);
                        PLC.WriteData(auxiliar, posicion * 4);
                        Console.WriteLine(auxiliar);

                    }
                    catch (Exception ex) {
                        Console.WriteLine();
                    }
                    

                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Falló la conexión");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ResetColor();
                }
                finally
                {
                    WSclient.Disconnect();
                    System.Threading.Thread.Sleep(2000);
                }
                
            }while (true);
        }
    }

    public class S7Connection
    {
        private S7Client Client = new S7Client();
        private Byte[] Buffer = new Byte[4];
        private int DB;

        public int DBnumber { get
            {
                return DB;
            } set {
                DB = value;
                    }
            }
 

        public string Conect(string CPUip,int CPUrack,int CPUslot)
        {
                    
            int Result = Client.ConnectTo(CPUip, CPUrack, CPUslot);
            return Client.ErrorText(Result);
        }

        public string WriteData (int tag, int offset)
        {

            Buffer[3] = (byte)(tag & 0xFF);
            Buffer[2] = (byte)((tag >> 8) & 0xFF);
            Buffer[1] = (byte)((tag >> 16) & 0xFF);
            Buffer[0] = (byte)((tag >> 24) & 0xFF);
            
            //Client.AsDBWrite(this.DB, offset,4, Buffer);
            Client.DBWrite(410, offset, Buffer.Length, Buffer);
           // Console.WriteLine(Buffer[0].ToString());
            return "OK";
        }

        public string Disconnect()
        {
            Client.Disconnect();
            return "OK";
        }


    }
}

