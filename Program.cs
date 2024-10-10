using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MemoriaProceso
{
    class Program
    {
        static void Main(string[] args)
        {
            // Creado por: Jared Daniel Salazar Sanchez

            // Define variables to track the peak memory usage of the process.
            long peakPagedMem = 0,
                 peakworkingset = 0,
                 peakVirtualem = 0;

            // Inicio del proceso.
            Process myProcess = Process.Start("NotePad.exe");
            try
            {
                // Espera un momento para asegurarse de que el proceso de Notepad esté completamente iniciado.
                System.Threading.Thread.Sleep(1000);

                // Hilo para capturar la entrada del usuario.
                Thread inputThread = new Thread(() =>
                {
                    while (!myProcess.HasExited)
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                        {
                            try
                            {
                                // Verifica si el proceso sigue en ejecución antes de refrescar.
                                if (!myProcess.HasExited)
                                {
                                    // Refresh los valores del proceso actual.
                                    myProcess.Refresh();
                                    Console.Clear();

                                    // Mostrar el nombre del creador.
                                    Console.WriteLine("Creado por: Jared Daniel Salazar Sanchez");
                                    Console.WriteLine();

                                    // Obtener todos los procesos de Notepad.
                                    var notepadProcesses = Process.GetProcessesByName("notepad");

                                    // Crear una tabla para mostrar la información.
                                    Console.WriteLine("+{0,-10}+{1,-20}+{2,-20}+{3,-10}+{4,-15}+{5,-20}+{6,-20}+{7,-25}+{8,-30}+{9,-20}+{10,-20}+",
                                        new string('-', 10), new string('-', 20), new string('-', 20), new string('-', 10), new string('-', 15), new string('-', 20), new string('-', 20), new string('-', 25), new string('-', 30), new string('-', 20), new string('-', 20));
                                    Console.WriteLine("|{0,-10}|{1,-20}|{2,-20}|{3,-10}|{4,-15}|{5,-20}|{6,-20}|{7,-25}|{8,-30}|{9,-20}|{10,-20}|",
                                        "ID", "Nombre", "Memoria Física (MB)", "Prioridad", "Clase Prioridad", "Tiempo CPU", "Tiempo CPU Kernel", "Tiempo Total CPU", "Memoria Virtual (GB)", "Memoria Página (MB)", "Estado", "Inicio", "Ejecutable", "Subprocesos", "Tiempo Ejecución");
                                    Console.WriteLine("+{0,-10}+{1,-20}+{2,-20}+{3,-10}+{4,-15}+{5,-20}+{6,-20}+{7,-25}+{8,-30}+{9,-20}+{10,-20}+",
                                        new string('-', 10), new string('-', 20), new string('-', 20), new string('-', 10), new string('-', 15), new string('-', 20), new string('-', 20), new string('-', 25), new string('-', 30), new string('-', 20), new string('-', 20));

                                    foreach (var process in notepadProcesses)
                                    {
                                        try
                                        {
                                            string fileName = "No disponible";
                                            try
                                            {
                                                fileName = process.MainModule.FileName;
                                            }
                                            catch { }

                                            string priorityClass = "No disponible";
                                            try
                                            {
                                                priorityClass = process.PriorityClass.ToString();
                                            }
                                            catch { }

                                            // Convertir la hora de inicio del proceso a la hora de México.
                                            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                                            DateTime startTimeInMexico = TimeZoneInfo.ConvertTime(process.StartTime, mexicoTimeZone);

                                            Console.WriteLine("|{0,-10}|{1,-20}|{2,-20}|{3,-10}|{4,-15}|{5,-20}|{6,-20}|{7,-25}|{8,-30}|{9,-20}|{10,-20}|",
                                                process.Id,
                                                process.ProcessName + (process.Id == myProcess.Id ? " (Nuevo)" : ""),
                                                process.WorkingSet64 / 1048576,
                                                process.BasePriority,
                                                priorityClass,
                                                process.UserProcessorTime,
                                                process.PrivilegedProcessorTime,
                                                process.TotalProcessorTime,
                                                process.VirtualMemorySize64 / 1073741824,
                                                process.PagedMemorySize64 / 1048576,
                                                process.Responding ? "Running" : "Not Responding",
                                                startTimeInMexico.ToString("HH:mm"),
                                                fileName,
                                                process.Threads.Count,
                                                DateTime.Now - process.StartTime
                                            );
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error al obtener información del proceso: {ex.Message}");
                                        }
                                    }

                                    Console.WriteLine("+{0,-10}+{1,-20}+{2,-20}+{3,-10}+{4,-15}+{5,-20}+{6,-20}+{7,-25}+{8,-30}+{9,-20}+{10,-20}+",
                                        new string('-', 10), new string('-', 20), new string('-', 20), new string('-', 10), new string('-', 15), new string('-', 20), new string('-', 20), new string('-', 25), new string('-', 30), new string('-', 20), new string('-', 20));
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error al actualizar la información del proceso: {ex.Message}");
                            }
                        }
                    }
                });

                // Inicia el hilo de entrada del usuario.
                inputThread.Start();

                // Espera a que el proceso de Notepad termine.
                myProcess.WaitForExit();

                Console.WriteLine();
                Console.WriteLine($"Fin del Proceso: {myProcess.ExitCode}");
                // Display peak memory statistics for the process.
                Console.WriteLine($"Memoria Física Máxima del Proceso desde el inicio: {peakworkingset / 1048576} MB");
                Console.WriteLine($"Memoria al archivo de paginación desde el inicio: {peakPagedMem / 1048576} MB");
                Console.WriteLine($"Memoria Virtual asignada desde el inicio: {peakVirtualem / 1073741824} GB");
            }
            finally
            {
                // Cierra el proceso si aún está en ejecución.
                if (!myProcess.HasExited)
                {
                    myProcess.Kill();
                    Console.WriteLine("El proceso Notepad ha sido cerrado.");
                }
                myProcess.Dispose();
            }

            // Pausa al final para que el usuario pueda revisar la información.
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
