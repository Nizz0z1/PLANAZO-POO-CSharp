using Planazo.Modelos;
using Planazo.Servicios;

namespace Planazo.Pruebas;

internal static class PruebasEscenarios
{
    public static int Ejecutar()
    {
        int aprobadas = 0;
        EjecutarCaso("Reserva válida y reducción de cupos", () =>
        {
            GestorReservas gestor = CrearGestor();
            Reserva reserva = gestor.CrearReserva("Ana Pérez", 1, 2);
            Afirmar(reserva.Estado == EstadoReserva.Activa && reserva.ValorTotal == 100m, "La reserva no quedó activa con el total esperado.");
            Afirmar(gestor.BuscarEvento(1)!.CuposDisponibles == 3, "No se descontaron los cupos.");
        }, ref aprobadas);

        EjecutarCaso("Rechazo de cantidad superior a los cupos", () =>
        {
            GestorReservas gestor = CrearGestor();
            Esperar<InvalidOperationException>(() => gestor.CrearReserva("Ana", 1, 6));
            Afirmar(gestor.BuscarEvento(1)!.CuposDisponibles == 5, "Los cupos cambiaron tras rechazar la reserva.");
        }, ref aprobadas);

        EjecutarCaso("Rechazo de cantidad negativa", () =>
        {
            GestorReservas gestor = CrearGestor();
            Esperar<ArgumentOutOfRangeException>(() => gestor.CrearReserva("Ana", 1, -1));
            Afirmar(gestor.BuscarEvento(1)!.CuposDisponibles == 5, "Los cupos cambiaron tras rechazar la cantidad.");
        }, ref aprobadas);

        EjecutarCaso("Cancelación devuelve los cupos", () =>
        {
            GestorReservas gestor = CrearGestor();
            Reserva reserva = gestor.CrearReserva("Ana", 1, 3);
            gestor.CancelarReserva(reserva.Id);
            Afirmar(reserva.Estado == EstadoReserva.Cancelada, "La reserva no quedó cancelada.");
            Afirmar(gestor.BuscarEvento(1)!.CuposDisponibles == 5, "Los cupos no se devolvieron.");
        }, ref aprobadas);

        EjecutarCaso("Rechazo de segunda cancelación", () =>
        {
            GestorReservas gestor = CrearGestor();
            Reserva reserva = gestor.CrearReserva("Ana", 1, 1);
            gestor.CancelarReserva(reserva.Id);
            Esperar<InvalidOperationException>(() => gestor.CancelarReserva(reserva.Id));
            Afirmar(gestor.BuscarEvento(1)!.CuposDisponibles == 5, "Una segunda cancelación alteró los cupos.");
        }, ref aprobadas);

        EjecutarCaso("Consulta de evento inexistente", () =>
        {
            GestorReservas gestor = CrearGestor();
            Afirmar(gestor.BuscarEvento(999) is null, "El evento inexistente debería retornar null.");
            Esperar<ArgumentException>(() => gestor.CrearReserva("Ana", 999, 1));
        }, ref aprobadas);

        EjecutarCaso("Precios finales de los tipos de evento", () =>
        {
            Evento cine = new Cine(1, "Cine", DateTime.Today, 50000m, 5);
            Evento concierto = new Concierto(2, "Concierto", DateTime.Today, 50000m, 5);
            Evento taller = new Taller(3, "Taller", DateTime.Today, 50000m, 5);
            Evento deportivo = new EventoDeportivo(4, "Deportivo", DateTime.Today, 50000m, 5);
            Afirmar(cine.CalcularPrecioFinal() == 50000m, "Precio de cine incorrecto.");
            Afirmar(concierto.CalcularPrecioFinal() == 57500m, "Precio de concierto incorrecto.");
            Afirmar(taller.CalcularPrecioFinal() == 45000m, "Precio de taller incorrecto.");
            Afirmar(deportivo.CalcularPrecioFinal() == 54000m, "Precio deportivo incorrecto.");
        }, ref aprobadas);

        EjecutarCaso("Usuario y resumen de reservas activas", () =>
        {
            GestorReservas gestor = CrearGestor();
            Reserva una = gestor.CrearReserva("Ana Pérez", 1, 1);
            gestor.CrearReserva("ANA PÉREZ", 1, 2);
            gestor.CancelarReserva(una.Id);
            Afirmar(gestor.BuscarReservasPorUsuario("ana pérez").Count == 2, "No se encontraron las reservas del usuario.");
            ResumenReservas resumen = gestor.ObtenerResumen();
            Afirmar(resumen.ReservasActivas == 1 && resumen.EntradasReservadas == 2 && resumen.ValorTotal == 100m, "Resumen activo incorrecto.");
        }, ref aprobadas);

        Console.WriteLine($"\nResultado: {aprobadas}/8 casos aprobados.");
        return aprobadas == 8 ? 0 : 1;
    }

    private static GestorReservas CrearGestor()
    {
        var gestor = new GestorReservas();
        gestor.RegistrarEvento(new Cine(1, "Película de prueba", DateTime.Today, 50m, 5));
        return gestor;
    }

    private static void EjecutarCaso(string nombre, Action caso, ref int aprobadas)
    {
        try { caso(); aprobadas++; Console.WriteLine($"[OK] {nombre}"); }
        catch (Exception ex) { Console.WriteLine($"[FALLO] {nombre}: {ex.Message}"); }
    }

    private static void Afirmar(bool condicion, string mensaje)
    {
        if (!condicion) throw new InvalidOperationException(mensaje);
    }

    private static void Esperar<T>(Action accion) where T : Exception
    {
        try { accion(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Se esperaba una excepción {typeof(T).Name}.");
    }
}
