namespace ConnectFour;

public class EstadoJuego
{
	static EstadoJuego()
	{
		CalcularLugaresGanadores();
	}

    /// <summary>
    /// Indica si un jugador ha ganado, el juego es un empate o el juego está en curso
    /// </summary>
    public enum EstadoGanado
    {
        SinGanador = 0,
        Jugador1Gana = 1,
        Jugador2Gana = 2,
        Empate = 3
    }

    /// <summary>
    /// El jugador al que le toca jugar. Por defecto, el jugador 1 comienza primero
    /// </summary>
    public int TurnoJugador => Tablero.Count(x => x != 0) % 2 + 1;

    /// <summary>
    /// Número de turnos completados y piezas jugadas hasta ahora en el juego
    /// </summary>
    public int TurnoActual { get { return Tablero.Count(x => x != 0); } }

    public static readonly List<int[]> LugaresGanadores = new();

    public static void CalcularLugaresGanadores()
    {

        // Filas horizontales
        for (byte fila = 0; fila < 6; fila++)
        {

            byte columnaInicio = (byte)(fila * 7);
            byte columnaFin = (byte)((fila + 1) * 7 - 1);
            byte columnaActual = columnaInicio;
            while (columnaActual <= columnaFin - 3)
            {
                LugaresGanadores.Add(new int[] {
                    columnaActual,
                    (byte)(columnaActual + 1),
                    (byte)(columnaActual + 2),
                    (byte)(columnaActual + 3)
                });
                columnaActual++;
            }

        }

        // Filas verticales
        for (byte col = 0; col < 7; col++)
        {
            byte colFilaInicio = col;
            byte colFilaFin = (byte)(35 + col);
            byte filaActual = colFilaInicio;
            while (filaActual <= 14 + col)
            {
                LugaresGanadores.Add(new int[] {
                filaActual,
                (byte)(filaActual + 7),
                (byte)(filaActual + 14),
                (byte)(filaActual + 21)
                });
                filaActual += 7;
            }
        }

        // Diagonal en forma de barra "/"
        for (byte col = 0; col < 4; col++)
        {
            // la columna inicial debe ser 0-3
            byte colFilaInicio = (byte)(21 + col);
            byte colFilaFin = (byte)(35 + col);
            byte posicionActual = colFilaInicio;
            while (posicionActual <= colFilaFin)
            {
                LugaresGanadores.Add(new int[] {
                posicionActual,
                (byte)(posicionActual - 6),
                (byte)(posicionActual - 12),
                (byte)(posicionActual - 18)
                });
                posicionActual += 7;
            }
        }

        // Diagonal en forma de contrabarra "\"
        for (byte col = 0; col < 4; col++)
        {
            // la columna inicial debe ser 0-3
            byte colFilaInicio = (byte)(0 + col);
            byte colFilaFin = (byte)(14 + col);
            byte posicionActual = colFilaInicio;
            while (posicionActual <= colFilaFin)
            {
                LugaresGanadores.Add(new int[] {
                    posicionActual,
                    (byte)(posicionActual + 8),
                    (byte)(posicionActual + 16),
                    (byte)(posicionActual + 24)
                    });
                posicionActual += 7;
            }
        }
    }

    /// <summary>
    /// Verifica el estado del tablero en busca de un escenario de victoria
    /// </summary>
    /// <returns>0 - no hay ganador, 1 - jugador 1 gana, 2 - jugador 2 gana, 3 - empate</returns>
    public EstadoGanado VerificarGanador()
    {

        // Salir inmediatamente si se han jugado menos de 7 piezas
        if (Tablero.Count(x => x != 0) < 7) return EstadoGanado.SinGanador;

        foreach (var scenario in LugaresGanadores)
        {

            if (Tablero[scenario[0]] == 0) continue;

            if (Tablero[scenario[0]] ==
                Tablero[scenario[1]] &&
                Tablero[scenario[1]] ==
                Tablero[scenario[2]] &&
                Tablero[scenario[2]] ==
                Tablero[scenario[3]]) return (EstadoGanado)Tablero[scenario[0]];

        }

        if (Tablero.Count(x => x != 0) == 42) return EstadoGanado.Empate;

        return EstadoGanado.SinGanador;
    }

    /// <summary>
    /// Toma el turno actual y coloca una ficha en la columna solicitada indexada en 0
    /// </summary>
    /// <param name="column">Columna indexada en 0 donde colocar la ficha</param>
    /// <returns>El índice final del array donde se encuentra la ficha</returns>
    public byte JugarPieza(int column)
    {

        // Verificar si hay una victoria actual
        if (VerificarGanador() != 0) throw new ArgumentException("El juego ha terminado");

        // Verificar la columna
        if (Tablero[column] != 0) throw new ArgumentException("La columna está llena");

        // Colocar la ficha
        var posicionAterrizaje = column;
        for (var i = column; i < 42; i += 7)
        {
            if (Tablero[posicionAterrizaje + 7] != 0) break;
            posicionAterrizaje = i;
        }

        Tablero[posicionAterrizaje] = TurnoJugador;

        return ConvertirLugarDeAterrizajeAFila(posicionAterrizaje);
    }

    public List<int> Tablero { get; private set; } = new List<int>(new int[42]);

    public void ReiniciarTablero()
    {
        Tablero = new List<int>(new int[42]);
    }

    private byte ConvertirLugarDeAterrizajeAFila(int landingSpot)
    {
        return (byte)(Math.Floor(landingSpot / (decimal)7) + 1);
    }

}