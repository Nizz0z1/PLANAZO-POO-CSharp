using Planazo.Modelos;
using Planazo.Servicios;

namespace Planazo;

internal static class Program
{
    private static readonly GestorReservas Gestor = new();

    private static void Main(string[] args)
    {
        if (args.Contains("--pruebas", StringComparer.OrdinalIgnoreCase))
        {
            Environment.ExitCode = Pruebas.PruebasEscenarios.Ejecutar();
            return;
        }
        CargarEventosIniciales();
        bool continuar = true;

        while (continuar)
        {
            MostrarMenu();
            switch (LeerEntero("Seleccione una opción: "))
            {
                case 1: VerEventosDisponibles(); break;
                case 2: ConsultarEvento(); break;
                case 3: ReservarEntradas(); break;
                case 4: VerReservas(); break;
                case 5: CancelarReserva(); break;
                case 6: VerResumen(); break;
                case 0: continuar = false; Console.WriteLine("Gracias por usar PLANAZO."); break;
                default: Console.WriteLine("Opción inválida. Seleccione una opción del menú."); break;
            }

            if (continuar && !Console.IsInputRedirected) Console.WriteLine("\nPresione una tecla para continuar...");
            if (continuar && !Console.IsInputRedirected) Console.ReadKey(intercept: true);
            if (!Console.IsInputRedirected) Console.Clear();
        }
    }

    private static void CargarEventosIniciales()
    {
        Gestor.RegistrarEvento(new Cine(1, "La ciudad de las estrellas", new DateTime(2026, 11, 12, 18, 0, 0), 18000m, 40));
        Gestor.RegistrarEvento(new Cine(2, "Viaje al centro", new DateTime(2026, 11, 13, 20, 0, 0), 22000m, 35));
        Gestor.RegistrarEvento(new Concierto(3, "Noche de Rock", new DateTime(2026, 11, 20, 19, 0, 0), 50000m, 100));
        Gestor.RegistrarEvento(new Concierto(4, "Jazz en vivo", new DateTime(2026, 11, 21, 20, 0, 0), 65000m, 80));
        Gestor.RegistrarEvento(new Taller(5, "Fotografía para todos", new DateTime(2026, 11, 24, 9, 0, 0), 40000m, 20));
        Gestor.RegistrarEvento(new Taller(6, "Introducción a la cerámica", new DateTime(2026, 11, 25, 14, 0, 0), 55000m, 18));
        Gestor.RegistrarEvento(new EventoDeportivo(7, "Carrera 5K PLANAZO", new DateTime(2026, 11, 29, 7, 0, 0), 30000m, 150));
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("================================");
        Console.WriteLine("        PLANAZO - EVENTOS");
        Console.WriteLine("================================\n");
        Console.WriteLine("1. Ver eventos disponibles");
        Console.WriteLine("2. Consultar evento");
        Console.WriteLine("3. Reservar entradas");
        Console.WriteLine("4. Ver reservas realizadas");
        Console.WriteLine("5. Cancelar una reserva");
        Console.WriteLine("6. Ver resumen de reservas");
        Console.WriteLine("0. Salir\n");
    }

    private static void VerEventosDisponibles()
    {
        Console.WriteLine("\nEVENTOS DISPONIBLES");
        foreach (Evento evento in Gestor.Eventos)
            Console.WriteLine($"{evento.Id}. {evento.Nombre} ({evento.GetType().Name}) | {evento.Fecha:dd/MM/yyyy HH:mm} | {evento.CuposDisponibles}/{evento.CapacidadMaxima} cupos | {evento.CalcularPrecioFinal():C0}");
    }

    private static void ConsultarEvento()
    {
        int id = LeerEntero("\nIngrese el ID del evento: ");
        Evento? evento = Gestor.BuscarEvento(id);
        if (evento is null)
        {
            Console.WriteLine("No existe un evento con ese identificador.");
            return;
        }
        Console.WriteLine($"\n{evento.Nombre} ({evento.GetType().Name})\nFecha: {evento.Fecha:dd/MM/yyyy HH:mm}\nPrecio final: {evento.CalcularPrecioFinal():C0}\nCupos disponibles: {evento.CuposDisponibles} de {evento.CapacidadMaxima}");
    }

    private static void ReservarEntradas()
    {
        try
        {
            Console.WriteLine();
            string nombre = LeerTexto("Nombre de la persona: ");
            int eventoId = LeerEntero("ID del evento: ");
            int cantidad = LeerEntero("Cantidad de entradas: ");
            Reserva reserva = Gestor.CrearReserva(nombre, eventoId, cantidad);
            Console.WriteLine($"Reserva #{reserva.Id} confirmada para {reserva.Usuario.Nombre}. Total: {reserva.ValorTotal:C0}.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"No se pudo crear la reserva: {ex.Message}"); }
        catch (InvalidOperationException ex) { Console.WriteLine($"No se pudo crear la reserva: {ex.Message}"); }
    }

    private static void VerReservas()
    {
        Console.WriteLine("\nRESERVAS REALIZADAS");
        if (Gestor.Reservas.Count == 0) { Console.WriteLine("Todavía no hay reservas."); return; }
        foreach (Reserva reserva in Gestor.Reservas)
            Console.WriteLine($"#{reserva.Id} | {reserva.Usuario.Nombre} | {reserva.Evento.Nombre} | {reserva.CantidadEntradas} entradas | {reserva.ValorTotal:C0} | {reserva.Estado}");

        Console.Write("\n¿Consultar reservas por usuario? (S/N): ");
        if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase)) return;
        string usuario = LeerTexto("Nombre del usuario: ");
        IReadOnlyList<Reserva> reservasUsuario = Gestor.BuscarReservasPorUsuario(usuario);
        if (reservasUsuario.Count == 0) Console.WriteLine("No se encontraron reservas para ese usuario.");
        foreach (Reserva reserva in reservasUsuario)
            Console.WriteLine($"#{reserva.Id} | {reserva.Evento.Nombre} | {reserva.CantidadEntradas} entradas | {reserva.ValorTotal:C0} | {reserva.Estado}");
    }

    private static void CancelarReserva()
    {
        try
        {
            int id = LeerEntero("\nIngrese el ID de la reserva que desea cancelar: ");
            Gestor.CancelarReserva(id);
            Console.WriteLine($"La reserva #{id} fue cancelada y sus cupos fueron devueltos.");
        }
        catch (ArgumentException ex) { Console.WriteLine($"No se pudo cancelar: {ex.Message}"); }
        catch (InvalidOperationException ex) { Console.WriteLine($"No se pudo cancelar: {ex.Message}"); }
    }

    private static void VerResumen()
    {
        ResumenReservas resumen = Gestor.ObtenerResumen();
        Console.WriteLine($"\nRESUMEN DE RESERVAS ACTIVAS\nReservas activas: {resumen.ReservasActivas}\nEntradas reservadas: {resumen.EntradasReservadas}\nValor total: {resumen.ValorTotal:C0}");
    }

    private static int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out int valor)) return valor;
            Console.WriteLine("Entrada inválida. Escriba un número entero.");
        }
    }

    private static string LeerTexto(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string? valor = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(valor)) return valor;
            Console.WriteLine("El texto no puede estar vacío.");
        }
    }
}
