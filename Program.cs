using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FTPServer 
{
    class Program {
        static void Main(string[] args)
        {
            StartFTPServer();
        }

        static void StartFTPServer()
        {
            try
            {
                int ftpPort = 21;
                TcpListener ftpListener = new TcpListener(IPAddress.Any, ftpPort);
                ftpListener.Start();
                Console.WriteLine("Servidor FTP iniciado. Esperando conexiones...");

                while (true)
                {
                    TcpClient client = ftpListener.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado.");

                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream);
                    StreamReader reader = new StreamReader(stream);

                    writer.WriteLine("220 Servidor FTP listo.");
                    writer.Flush();

                    bool exitRequested = false;
                    while (!exitRequested)
                    {
                        string request = reader.ReadLine();
                        Console.WriteLine($"Cliente: {request}");

                        if (request.StartsWith("USER"))
                        {
                            writer.WriteLine("331 Usuario OK, contraseña requerida.");
                            writer.Flush();
                        }
                        else if (request.StartsWith("PASS"))
                        {
                            writer.WriteLine("230 Usuario autenticado.");
                            writer.Flush();
                        }
                        else if (request.StartsWith("LIST"))
                        {
                            string files = string.Join(Environment.NewLine, Directory.GetFiles("."));
                            writer.WriteLine($"150 Iniciando lista de archivos:{Environment.NewLine}{files}");
                            writer.Flush();
                            writer.WriteLine("226 Lista de archivos enviada correctamente.");
                            writer.Flush();
                        }
                        else if (request.StartsWith("QUIT"))
                        {
                            writer.WriteLine("221 Servidor cerrando conexión.");
                            writer.Flush();
                            exitRequested = true;
                        }
                        else
                        {
                            writer.WriteLine("500 Comando no reconocido.");
                            writer.Flush();
                        }
                    }

                    client.Close();
                    Console.WriteLine("Cliente desconectado.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }   
}