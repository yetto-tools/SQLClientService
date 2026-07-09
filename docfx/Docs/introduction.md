# Introducción

**DBSQLClient** es un cliente ligero sobre `Microsoft.Data.SqlClient` para ejecutar consultas y
procedimientos almacenados en SQL Server desde .NET, sin la sobrecarga de un ORM completo.

## ¿Qué resuelve?

Trabajar con `SqlConnection`/`SqlCommand` directamente implica repetir mucho código de plomería:
abrir y cerrar conexiones, mapear filas a objetos a mano, convertir valores nulos, leer parámetros
de salida, etc. `DBSQLClient` encapsula ese código repetitivo en unas pocas piezas:

- **[`SqlClientService`](xref:DBSQLClient.Conexion.SqlClientService)** — punto de entrada único
  para ejecutar SQL de texto (`QueryAsync`/`Query`) o procedimientos almacenados
  (`ExecuteAsync`/`Execute`), en versión síncrona y asíncrona.
- **[`SqlQueryResult`](xref:DBSQLClient.Conexion.SqlQueryResult)** — envuelve el `DataSet` devuelto
  por SQL Server y ofrece salidas listas para usar: `ToList<T>()`, `FirstOrDefault<T>()`,
  `AsDataTable()`, `ToJson()`, parámetros de salida vía `GetOutputValue<T>()`, etc.
- **`SqlResultMapper`** (`Servicio/Mapper`) — mapea automáticamente columnas a propiedades usando
  atributos como `[Column]`, `[PrimaryKey]`, `[ForeignKey]`, y resuelve relaciones
  `OneToMany`/`ManyToOne`/`ManyToMany`/`OneToOne` entre tablas del mismo resultado.
- **`SqlParams`** (`Servicio/Parameter`) — builder de parámetros con métodos tipados
  (`SqlParams.Int`, `SqlParams.String`, `SqlParams.DateTime`, `SqlParams.OutParam`, etc.) que
  convierten `null` a `DBNull.Value` automáticamente y normalizan el `@` en el nombre.

## Características principales

- Ejecución de queries de texto y stored procedures, síncrona o asíncrona.
- Mapeo automático `DataTable` → objetos tipados (`ToList<T>`, `FirstOrDefault<T>`), incluyendo
  relaciones entre tablas de un mismo resultado.
- Manejo simplificado de parámetros de entrada, salida (`Output`), entrada/salida (`InputOutput`)
  y valor de retorno (`ReturnValue`).
- Serialización/deserialización JSON integrada (`ToJson`, `FromJson`, `SaveToJsonFileAsync`), con
  atributos para controlar el formato de fechas (`[JsDateTime]`) y exclusión de columnas
  (`[NotMapped]`, `[NotSerialized]`).
- Mensajes de error enriquecidos con el texto del comando SQL que falló.

## Requisitos

- .NET 10.
- `Microsoft.Data.SqlClient` (referenciado como dependencia del paquete).

## Siguientes pasos

Continúa con [Getting Started](getting-started.md) para instalar el paquete y ejecutar tu primera
consulta, o consulta directamente la [Referencia API](../api/toc.yml) generada desde el
código fuente.
