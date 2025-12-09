using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DBSQLClient.Servicio.Conexion
{
    /// <resumen>
    /// Define métodos para ejecutar consultas SQL y procedimientos almacenados de forma asíncrona en un cliente de base de datos,
    /// devolviendo resultados en varios formatos.
    /// </resumen>
    /// <observaciones>Esta interfaz proporciona una forma estandarizada de interactuar con bases de datos SQL de forma asíncrona,
    /// admitiendo tanto comandos SQL sin procesar como procedimientos almacenados. Se espera que las implementaciones gestionen la conexión
    /// , la parametrización y la cancelación a través de los tokens proporcionados. Los métodos devuelven los resultados como DataTable,
    /// DataSet o un tipo deserializado, lo que permite patrones de acceso a datos flexibles. La seguridad de los subprocesos y el manejo de errores dependen
    /// de la implementación específica.</remarks>
    public interface ISQLClientService
    {
        /// <summary>
        /// Ejecuta el comando SQL especificado de forma asíncrona y devuelve los resultados como un <see cref="DataTable"/>.
        /// </summary>
        /// <remarks>El <see cref="DataTable"/> devuelto contiene el conjunto de resultados producido por el comando SQL
        /// . Si la operación se cancela mediante el token <paramref name="ct"/>, la tarea devuelta estará en un
        /// estado cancelado. El tiempo de espera se aplica a la ejecución del comando, no a toda la operación.</remarks>
        /// <param name="sqlCommand">La consulta SQL que se ejecutará en la base de datos. No puede ser nula ni estar vacía.</param>
        /// <param name="parameters">Una matriz opcional de objetos <see cref="SqlParameter"/> que se utilizarán con el comando SQL. Si es nulo, no
        /// se aplican parámetros.</param>
        /// <param name="ct">Un <see cref="CancellationToken"/> que se puede utilizar para cancelar la operación asíncrona.</param>
        /// <param name="TimeOut">El tiempo máximo, en milisegundos, que se espera a que se ejecute el comando antes de agotarse el tiempo de espera. Debe ser mayor
        /// que cero.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea contiene un <see cref="DataTable"/> con
        /// los resultados de la consulta. La tabla estará vacía si no se devuelven filas.</returns>s
        public Task<DataTable> QueryAsyncAsDataTable(
            string sqlCommand,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int TimeOut = 5000);

        /// <resumen>
        /// Ejecuta el comando SQL especificado de forma asíncrona y devuelve los resultados como un <see cref="DataSet"/>.
        /// </resumen>
        /// <observaciones>El <see cref="DataSet"/> devuelto contendrá los datos recuperados por el comando SQL
        /// . Si el comando no devuelve ningún resultado, el <see cref="DataSet"/> estará vacío. El método
        /// respeta el tiempo de espera y el token de cancelación especificados; si la operación se cancela o se agota el tiempo de espera, la
        /// tarea devuelta dará error.</remarks>
        /// <param name="sqlCommand">La consulta o comando SQL que se ejecutará en la base de datos. No puede ser nulo ni estar vacío.</param>
        /// <param name="parameters">Una matriz de objetos <see cref="SqlParameter"/> que se pasarán al comando SQL. Puede ser nulo si el comando
        /// no requiere parámetros.</param>
        /// <param name="ct">Un <see cref="CancellationToken"/> que se puede utilizar para cancelar la operación asíncrona.</param>
        /// <param name="TimeOut">El tiempo máximo, en milisegundos, que se puede esperar a que se ejecute el comando antes de que se agote el tiempo de espera. Debe ser mayor
        /// que cero.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea contiene un <see cref="DataSet"/> con los
        /// resultados del comando SQL.</returns>
        public Task<DataSet> ExecuteAsyncAsDataSet(
                string sqlCommand, 
                SqlParameter[]? parameters = default, 
                CancellationToken ct = default,  
                int TimeOut = 5000);

        /// <summary>
        /// Ejecuta el procedimiento almacenado SQL especificado de forma asíncrona y devuelve los resultados como un <see
        /// cref="DataSet"/>.
        /// </summary>
        /// <remarks>El <see cref="DataSet"/> devuelto contendrá todos los conjuntos de resultados producidos por el
        /// procedimiento almacenado. Si el procedimiento almacenado no devuelve ningún resultado, el <see cref="DataSet"/> estará
        /// vacío. El método lanza una excepción si el nombre del procedimiento almacenado no es válido o si se produce un error de base de datos
        ///.</remarks>
        /// <param name="nameStoredProcedure">El nombre del procedimiento almacenado que se va a ejecutar. No puede ser nulo ni estar vacío.
        /// <param name="parameters">Una matriz de objetos <see cref="SqlParameter"/> para pasar al procedimiento almacenado, o <see langword="null"/> para
        /// ejecutar sin parámetros.
        /// <param name="ct">Un <see cref="CancellationToken"/> que se puede utilizar para cancelar la operación asíncrona.</param>
        /// <param name="TimeOut">El tiempo de espera del comando, en milisegundos, para ejecutar el procedimiento almacenado. Debe ser mayor que cero.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea contiene un <see cref="DataSet"/> con los
        /// resultados de la ejecución del procedimiento almacenado.</returns>
        public Task<DataSet> ExecuteStoredProcedureAsync(
         string nameStoredProcedure,
         SqlParameter[]? parameters = default,
         CancellationToken ct = default,
         int TimeOut = 5000);

        /// <summary>
        /// Ejecuta el procedimiento almacenado SQL especificado de forma asíncrona y devuelve el resultado como un objeto de tipo
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>El método ejecuta el procedimiento almacenado utilizando los parámetros y el tiempo de espera proporcionados.
        /// Si la operación se cancela mediante el token de cancelación, la tarea devuelta se cancelará. Asegúrese de que
        /// el parámetro de tipo <typeparamref name="T"/> coincida con el tipo de resultado esperado del procedimiento almacenado para
        /// evitar errores en tiempo de ejecución.</remarks>
        /// <typeparam name="T">El tipo del objeto de resultado que se devolverá. Debe ser compatible con los datos devueltos por el
        /// procedimiento almacenado. </typeparam>
        /// <param name="nameStoredProcedure">El nombre del procedimiento almacenado SQL que se va a ejecutar. No puede ser nulo ni estar vacío. </param>
        /// <param name="parameters">Una matriz de objetos <see cref="SqlParameter"/> que se pasarán al procedimiento almacenado, o <see langword="null"/> para
        /// ejecutar sin parámetros.</param>
        /// <param name="ct">Un <see cref="CancellationToken"/> que se puede utilizar para cancelar la operación asíncrona. </param>
        /// <param name="TimeOut">El tiempo máximo, en milisegundos, que se debe esperar a que se ejecute el comando antes de que se agote el tiempo de espera. Debe ser mayor
        /// que cero.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea contiene un objeto de tipo <typeparamref
        /// name="T"/> con el resultado del procedimiento almacenado, o <see langword="null"/> si no se devuelve ningún resultado.</returns>
        public Task<T?> ExecuteStoredProcedureAsync<T>(
            string nameStoredProcedure,
            SqlParameter[]? parameters = default,
            CancellationToken ct = default,
            int TimeOut = 5000);
    }
}
