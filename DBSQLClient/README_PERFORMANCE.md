# Consideraciones de rendimiento

- **Lectura de resultados**: los comandos usan `CommandBehavior.Default`. Se evita `CommandBehavior.SequentialAccess` porque `DataTable.Load`, usado para materializar cada resultado, no recorre de forma confiable múltiples result sets del mismo comando cuando el lector está en modo secuencial (rompe casos como `MapOneToOne`/`MapOneToMany`, que dependen de más de un `SELECT` en el mismo procedimiento).
- **Múltiples result sets**: cada conjunto se materializa con `DataTable.Load`, iterando resultado por resultado con `NextResult`/`NextResultAsync` sin bucles adicionales ni copias manuales.
- **Cancelación cooperativa**: todos los métodos aceptan `CancellationToken`, lo que permite interrumpir ejecuciones largas y evitar saturación en escenarios de alta demanda.
- **Uso controlado de conexiones**: cada ejecución abre y cierra la conexión dentro de un bloque `using`, limitando la cantidad de conexiones activas y reduciendo el riesgo de throttling por exceso de sesiones abiertas.

Este diseño prioriza throughput manteniendo la compatibilidad con `DataSet` para consumidores existentes. En cargas muy altas, ajustar `Timeout` y el `CancellationToken` del comando ayuda a aplicar backpressure sin bloquear el hilo llamador.
