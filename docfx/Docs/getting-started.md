# Getting Started

## Instalación

Agrega el paquete `DBSQLClient` a tu proyecto:

```bash
dotnet add package DBSQLClient
```

## Crear el servicio

`SqlClientService` necesita una cadena de conexión. Regístralo como singleton o instáncialo
directamente:

```csharp
using DBSQLClient.Conexion;

var connectionString = "Server=localhost;Database=MiBaseDatos;Trusted_Connection=True;TrustServerCertificate=True;";
var service = new SqlClientService(connectionString);
```

Si usas inyección de dependencias, registra la interfaz `ISQLClientService`:

```csharp
builder.Services.AddSingleton<ISQLClientService>(
    _ => new SqlClientService(connectionString));
```

## Tu primera consulta

```csharp
var result = await service.QueryAsync("SELECT Id, Name, Email FROM Users WHERE IsActive = 1");

var users = result.ToList<User>(); // mapeo automático por nombre de columna/propiedad
```

`User` solo necesita un constructor público sin parámetros; las propiedades se emparejan por
nombre (o por `[Column("nombre_columna")]` si el nombre en SQL es distinto):

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
}
```

## Ejecutar un procedimiento almacenado

```csharp
using DBSQLClient.Servicio.Parameter;

var result = await service.ExecuteAsync(
    "sp_GetUser",
    SqlParams.AddParams(("Id", 123)));

var user = result.FirstOrDefault<User>();
```

Ver la referencia rápida de `SqlParams` en el `README.md` de la raíz del repositorio para todos
los métodos de construcción de parámetros (tipados, opcionales, de salida, builder fluido, etc.).

## Leer parámetros de salida

```csharp
var outputTotal = SqlParams.OutParam("Total", SqlDbType.Int);

var result = await service.ExecuteAsync(
    "sp_GetUserOrderTotal",
    SqlParams.AddParams(("UserId", 123)).Concat(new[] { outputTotal }).ToArray());

int total = result.GetOutputValue<int>("Total") ?? 0;
```

## Consultas síncronas

Todos los métodos asíncronos tienen su equivalente síncrono (`Query`/`Execute` en vez de
`QueryAsync`/`ExecuteAsync`), útil en contextos donde no se puede usar `await`:

```csharp
var result = service.Query("SELECT * FROM Users WHERE Id = @Id", SqlParams.AddParams(("Id", 123)));
```

## Serializar el resultado a JSON

```csharp
string json = result.ToJson<User>(); // lista tipada
string jsonDataSet = result.ToJsonDataSet(); // todas las tablas del DataSet
```

## Manejo de errores

Las excepciones de SQL Server (`SqlException`) se envuelven en `InvalidOperationException` con el
texto del comando que falló incluido en el mensaje, para facilitar el diagnóstico:

```csharp
try
{
    await service.ExecuteAsync("sp_QueNoExiste");
}
catch (InvalidOperationException ex)
{
    // ex.Message incluye el nombre del procedimiento y el error original de SQL Server
    // ex.InnerException es la SqlException original
}
```

## Siguiente paso

Explora la [referencia API completa](../api/toc.yml) generada a partir de los comentarios
XML del código fuente, o revisa el proyecto `DBSQLClient.Demo` en el repositorio para ver un
ejemplo end-to-end (tienda en línea con inventario y facturación).
