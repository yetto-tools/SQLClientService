# Consideraciones de rendimiento

- **Lectura en streaming**: los comandos usan `CommandBehavior.SequentialAccess` para que el `SqlDataReader` entregue los campos en flujo y se minimice el buffering en memoria incluso cuando existen columnas grandes o múltiples conjuntos de resultados.
- **Múltiples result sets**: cada conjunto se materializa con `DataTable.Load`, que ya está optimizado internamente; se itera resultado por resultado sin bucles adicionales ni copias manuales.
- **Cancelación cooperativa**: todos los métodos aceptan `CancellationToken`, lo que permite interrumpir ejecuciones largas y evitar saturación en escenarios de alta demanda.
- **Uso controlado de conexiones**: cada ejecución abre y cierra la conexión dentro de un bloque `using`, limitando la cantidad de conexiones activas y reduciendo el riesgo de throttling por exceso de sesiones abiertas.

Este diseño prioriza throughput manteniendo la compatibilidad con `DataSet` para consumidores existentes. En cargas muy altas, ajustar `Timeout` y el `CancellationToken` del comando ayuda a aplicar backpressure sin bloquear el hilo llamador.
