#OtherInformation: <Nullable>enable</Nullable>
[]: # OtherInformation: <ImplicitUsings>enable</ImplicitUsings>

> **Referencia ejecutable:** [`DBSQLClient.Demo`](../DBSQLClient.Demo/README.md) tiene un
> ejemplo copiable por cada capacidad descrita en este README (relaciones, parámetros de
> salida, JSON, etc.), corriendo contra una base de datos que se crea sola con `dotnet run`.

```csharp 
# Configuración avanzada para formateo salida del JSON (JsonSerializerOptions) 
# ============================================


var options = new JsonSerializerOptions
{
    // ============================================
    // FORMATO Y PRESENTACIÓN
    // ============================================
    
    // Formato con indentación (más legible)
    WriteIndented = true,  // Por defecto: true en tu clase
    
    // Tamaño de la indentación
    IndentCharacter = ' ',  // Carácter para indentar (espacio por defecto)
    IndentSize = 2,         // Número de caracteres (2 espacios por defecto)
    
    // ============================================
    // NOMENCLATURA DE PROPIEDADES
    // ============================================
    
    // Convierte nombres de propiedades
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // "Name" → "name"
    // Opciones: null (sin cambios), JsonNamingPolicy.CamelCase, JsonNamingPolicy.SnakeCaseLower, JsonNamingPolicy.SnakeCaseUpper, JsonNamingPolicy.KebabCaseLower, JsonNamingPolicy.KebabCaseUpper

    
    // Convierte nombres de diccionarios
    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    
    // ============================================
    // MANEJO DE VALORES
    // ============================================
    
    // Ignora propiedades null al escribir
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    // Opciones:
    // - Never: Incluye todo
    // - Always: Ignora todo
    // - WhenWritingNull: Ignora solo null
    // - WhenWritingDefault: Ignora valores por defecto (0, false, null, etc.)
    
    // Permite comentarios en JSON al leer
    ReadCommentHandling = JsonCommentHandling.Skip,
    
    // Permite comas finales
    AllowTrailingCommas = true,
    
    // ============================================
    // NÚMEROS Y TIPOS ESPECIALES
    // ============================================
    
    // Permite leer números como strings
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    // Opciones:
    // - Strict: Solo números
    // - AllowReadingFromString: "123" → 123
    // - WriteAsString: 123 → "123"
    // - AllowNamedFloatingPointLiterals: Permite NaN, Infinity
    
    // ============================================
    // CASE SENSITIVITY
    // ============================================
    
    // Ignora mayúsculas/minúsculas al deserializar
    PropertyNameCaseInsensitive = true,  // Por defecto: true en tu clase
    
    // ============================================
    // ENCODING Y CARACTERES
    // ============================================
    
    // Codifica caracteres HTML (<, >, &, ', ")
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    // Opciones:
    // - JavaScriptEncoder.Default: Codifica todo
    // - JavaScriptEncoder.UnsafeRelaxedJsonEscaping: Más permisivo
    
    // ============================================
    // PROFUNDIDAD Y TAMAÑO
    // ============================================
    
    // Profundidad máxima de objetos anidados
    MaxDepth = 64,  // Por defecto: 64
    
    // Tamaño máximo del buffer
    DefaultBufferSize = 16384,  // 16KB por defecto
    
    // ============================================
    // REFERENCIAS Y CICLOS
    // ============================================
    
    // Manejo de referencias circulares
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    // Opciones:
    // - null: Error en ciclos
    // - ReferenceHandler.Preserve: Usa $id y $ref
    // - ReferenceHandler.IgnoreCycles: Ignora referencias circulares
    
    // ============================================
    // CONVERSORES PERSONALIZADOS
    // ============================================
    
    // Agregar conversores personalizados
    Converters = 
    {
        new JsonStringEnumConverter(),  // Convierte enums a strings
        // Puedes agregar tus propios conversores aquí
    },
    
    // ============================================
    // OTRAS OPCIONES
    // ============================================
    
    // Permite campos además de propiedades
    IncludeFields = false,
    
    // Ignora propiedades de solo lectura al serializar
    IgnoreReadOnlyProperties = false,
    
    // Ignora propiedades de solo lectura al deserializar
    IgnoreReadOnlyFields = false,
    
    // Orden de las propiedades
    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
    
    // Respeta [Required] attributes
    RespectRequiredConstructorParameters = false,
    
    // Respeta atributos de validación
    RespectNullableAnnotations = false,
    
    // Permite referencias no resueltas
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
    
    // Comportamiento de strings desconocidas en enums
    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
};



// ============================================
// Ejemplo 1: JSON Compacto (sin indentación)
// ============================================
var compactOptions = new JsonSerializerOptions
{
    WriteIndented = false
};
string compact = result.ToJson(compactOptions);
// Resultado: [{"Id":1,"Name":"Juan"},{"Id":2,"Name":"María"}]

// ============================================
// Ejemplo 2: CamelCase (estilo JavaScript)
// ============================================
var camelCaseOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
string camelJson = result.ToJson(camelCaseOptions);
/* Resultado:
[
  {
    "id": 1,
    "name": "Juan",
    "email": "juan@email.com"
  }
]
*/

// ============================================
// Ejemplo 3: Incluir valores null y por defecto
// ============================================
var includeAllOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
};
string withNulls = result.ToJson(includeAllOptions);
/* Resultado:
[
  {
    "Id": 1,
    "Name": "Juan",
    "Email": null,
    "IsActive": false
  }
]
*/

// ============================================
// Ejemplo 4: Manejo especial de números
// ============================================
var numberOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    NumberHandling = JsonNumberHandling.WriteAsString
};
string numbersAsStrings = result.ToJson(numberOptions);
/* Resultado:
[
  {
    "Id": "1",
    "Price": "99.99",
    "Name": "Producto"
  }
]
*/

// ============================================
// Ejemplo 5: Manejo de referencias circulares
// ============================================
var cyclicOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    ReferenceHandler = ReferenceHandler.IgnoreCycles
};
string safeJson = result.ToJson(cyclicOptions);

// ============================================
// Ejemplo 6: Caracteres especiales sin codificar
// ============================================
// Ya no hace falta armar esto a mano: la libreria usa UnsafeRelaxedJsonEscaping por
// defecto (ver "Acentos y Unicode en el JSON" mas abajo), asi que ToJsonString()/ToJson<T>()
// ya devuelven el texto sin escapar. Este ejemplo queda para cuando necesitas tu propio
// JsonSerializerOptions por otro motivo (namingPolicy, indentacion, etc.) y tambien queres
// este encoder ahi.
var relaxedOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
};
string unescaped = result.ToJson(relaxedOptions);
// "Nombre": "José & María" (en lugar de "Jos\u00E9 & Mar\u00EDa")

// ============================================
// Ejemplo 7: Configuración para APIs REST
// ============================================
var apiOptions = new JsonSerializerOptions
{
    WriteIndented = false,  // Compacto para red
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,  // Estándar JavaScript
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // Menos datos
    PropertyNameCaseInsensitive = true,  // Flexible en lectura
    Converters = { new JsonStringEnumConverter() }  // Enums legibles
};
string apiJson = result.ToJson(apiOptions);

// ============================================
// Ejemplo 8: Configuración para logs/debug
// ============================================
var debugOptions = new JsonSerializerOptions
{
    WriteIndented = true,  // Legible
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,  // Ver todo
    ReferenceHandler = ReferenceHandler.IgnoreCycles,  // Evitar errores
    MaxDepth = 10  // Limitar profundidad
};
string debugJson = result.ToJson(debugOptions);






// ============================================
// Configuración por defecto reutilizable
private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,  // Flexible al leer
    WriteIndented = true,  // Formato legible
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // No incluir nulls
    Converters = { new JsonStringEnumConverter() }  // Enums como texto
};
```

## Mapeo de relaciones (SqlResultMapper)

`SqlQueryResult` puede mapearse a objetos con relaciones (uno-a-uno, uno-a-muchos, muchos-a-uno
y muchos-a-muchos) usando los métodos de extensión en `DBSQLClient.Servicio.Mapper.RelationsMapper`.

Declara las relaciones en tus modelos con atributos:

```csharp
public class User
{
    [PrimaryKey]
    [Column("user_id")]
    public int Id { get; set; }

    [Column("user_name")]
    public string Name { get; set; }

    [OneToOne(typeof(UserProfile))]
    public UserProfile Profile { get; set; }

    [OneToMany(typeof(Order))]
    public List<Order> Orders { get; set; } = new();

    [ManyToMany(typeof(Role), typeof(UserRole))]
    public List<Role> Roles { get; set; } = new();
}

public class Order
{
    [PrimaryKey]
    [Column("order_id")]
    public int Id { get; set; }

    [ForeignKey(typeof(User))]
    [Column("user_id")]
    public int UserId { get; set; }

    [ManyToOne(typeof(User))]
    public User User { get; set; }
}
```

`[Column("nombre_columna")]` es opcional: si no se indica, se usa el nombre de la propiedad tal
cual viene en el `DataTable`. `[PrimaryKey]` y `[ForeignKey]` permiten que el mapper resuelva las
claves automáticamente al agrupar padres con hijos o al cruzar tablas de unión.

Con las relaciones declaradas, el nombre de la propiedad de navegación y las claves se resuelven
solas — no hace falta pasarlas como string:

```csharp
// Tabla 0 = User, tabla 1 = UserProfile
var user = result.MapOneToOne<User, UserProfile>();

// Tabla 0 = User, tabla 1 = Orders
var user = result.MapOneToMany<User, Order>();

// Tabla 0 = Orders, tabla 1 = User (un único padre compartido por todos los hijos)
var orders = result.MapManyToOne<Order, User>();

// Tabla 0 = Users, tabla 1 = Roles, tabla 2 = UserRole (tabla de unión)
var users = result.MapManyToMany<User, Role, UserRole>();

// Varios padres, cada uno con sus propios hijos (tabla 0 = Users, tabla 1 = Orders)
var users = result.OneToManyMultiple<User, Order>();
```

### Atributos vs. métodos `Map*`: qué hace cada uno

Son dos cosas distintas que trabajan juntas, y es fácil confundirlas porque comparten nombre:

- **Los atributos** (`[OneToOne]`, `[OneToMany]`, `[ManyToOne]`, `[ManyToMany]`) son **metadata**:
  se declaran una sola vez sobre la propiedad de navegación de un modelo y describen la forma de
  esa relación (qué tipo hay del otro lado). No hacen nada por sí solos — no consultan la base ni
  arman objetos.
- **Los métodos `Map*`** (`MapOneToOne`, `MapOneToMany`, `MapManyToOne`, `MapManyToMany`,
  `MapMultipleOneToMany`, y sus atajos `OneToOne`/`OneToMany`/`ManyToOne`/`ManyToMany`/
  `OneToManyMultiple`) son la **operación en tiempo de ejecución**: leen las tablas de un
  `SqlQueryResult` con varios result sets, buscan por reflexión el atributo que corresponde para
  saber en qué propiedad escribir, y arman los objetos. Cada uno espera un número y orden de
  tablas específico, que tiene que coincidir con lo que devuelve tu procedimiento almacenado.

| Atributo (en el modelo) | Método(s) que lo leen | Tablas esperadas |
|---|---|---|
| `[OneToOne(typeof(Child))]` en `Parent` | `MapOneToOne<Parent, Child>` | 0 = `Parent` (1 fila), 1 = `Child` (0 o 1 fila) |
| `[OneToMany(typeof(Child))]` en `Parent` | `MapOneToMany<Parent, Child>` (un padre) | 0 = `Parent` (1 fila), 1 = hijos |
| `[OneToMany(typeof(Child))]` en `Parent` | `MapMultipleOneToMany<Parent, Child>` / `OneToManyMultiple` (varios padres) | 0 = padres, 1 = hijos (agrupados por PK/FK) |
| `[ManyToOne(typeof(Parent))]` en `Child` | `MapManyToOne<Child, Parent>` | 0 = hijos, 1 = `Parent` único compartido (1 fila) |
| `[ManyToMany(typeof(Right), typeof(Join))]` en `Left` | `MapManyToMany<Left, Right, Join>` | 0 = `Left`, 1 = `Right`, 2 = `Join` |

**Dónde se declara cada atributo** es la parte que más confunde: se pone del lado que tiene la
propiedad de navegación, y su nombre describe la relación *desde ese lado*. Por eso
`[OneToMany(typeof(Order))]` va en `User.Orders`, pero `[ManyToOne(typeof(User))]` va en
`Order.User` — son la misma relación de datos vista desde los dos lados, con dos atributos
distintos en dos modelos distintos, y dos métodos distintos (`MapOneToMany<User, Order>` espera
tabla 0 = User; `MapManyToOne<Order, User>` espera tabla 0 = Orders) según cuál uses.

Un modelo puede declarar **más de un** `[ManyToOne]` en propiedades distintas, uno por cada tipo
de padre posible — útil para "pertenece a A o a B, nunca ambos" (ej: una orden de un usuario
registrado o de un invitado): `MapManyToOne<Order, User>()` y
`MapManyToOne<Order, GuestCheckout>()` resuelven cada uno el suyo sin pisarse.

Si tus nombres de propiedad o de claves no siguen la convención de los atributos (o el modelo no
tiene atributos), todos los métodos aceptan los nombres explícitos como parámetros opcionales,
por ejemplo `result.MapOneToMany<User, Order>("Orders")` o
`result.MapManyToMany<User, Role, UserRole>("Roles", leftKey: "Id", joinLeftKey: "UserId", joinRightKey: "RoleId")`.

## Origen del modelo: [Table] y [StoredProcedure]

Estos atributos de clase documentan de dónde vienen los datos de un modelo. Por ahora son solo
metadata (consultable por reflexión vía `MetadataCache`): `SqlClientService.QueryAsync`/`ExecuteAsync`
siguen recibiendo el nombre de la tabla o del procedimiento como `string`, no lo resuelven todavía
a partir del atributo.

```csharp
// El modelo representa filas de una tabla, para lectura directa (SELECT).
[Table("Users")]
public class User { /* ... */ }

// El modelo representa el resultado de un procedimiento almacenado específico.
[StoredProcedure("sp_GetUserOrderTotal")]
public class UserOrderTotal
{
    public int UserId { get; set; }
    public int TotalOrders { get; set; }
}
```

Son mutuamente excluyentes: un modelo no puede tener ambos a la vez. `MetadataCache` lanza
`InvalidOperationException` si detecta los dos atributos en el mismo tipo.

## Excluir propiedades del mapeo: [NotMapped]

Marca con `[NotMapped]` cualquier propiedad que no venga de la base de datos (por ejemplo, una
propiedad calculada). Queda completamente fuera del mapeo: no se busca su columna, no participa
como PK/FK ni en relaciones. Es necesario en propiedades de solo lectura (sin `set`), ya que de
lo contrario el mapper fallaría al intentar asignarles un valor:

```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    [NotMapped]
    public string DisplayName => $"{Name} <{Email}>";
}
```

Aplica tanto a `SqlResultMapper` (`MapOneToOne`, `MapOneToMany`, etc.) como a `SqlQueryResult.ToList<T>()`/`FirstOrDefault<T>()`.

## Acentos y Unicode en el JSON

`ToJsonString()`, `ToJsonDocument()` y `SqlQueryResult.ToJson`/`ToJson<T>` usan
`JavaScriptEncoder.UnsafeRelaxedJsonEscaping` por defecto, así que un valor como `"Camiseta
Básica"` sale tal cual en el JSON en vez del `System.Text.Json` por defecto, que escapa todo
carácter no-ASCII como secuencia `\uXXXX` (`"Camiseta Básica"`) — válido, pero mucho menos
legible para logs, debugging, o cualquier texto en español/con acentos.

Este encoder sigue escapando lo necesario para que el JSON sea válido (comillas, barras
invertidas, caracteres de control), pero **no** escapa caracteres sensibles para HTML (`<`, `>`,
`&`, `'`). Es la elección correcta para una respuesta de API consumida por `fetch`/HTTP client
(el caso normal), pero si vas a incrustar este JSON directo dentro de una página HTML (ej. en un
`<script>` inline), hacé el escape correspondiente en ese punto — no asumas que este JSON ya es
seguro para ese contexto.

## Excluir propiedades solo del JSON: [NotSerialized]

`[NotSerialized]` es distinto de `[NotMapped]`: la propiedad **sigue mapeándose** desde la base
de datos con normalidad, pero nunca aparece en el JSON que producen `ToJsonString()`
(`ObjectJsonExtensions`) ni `SqlQueryResult.ToJson<T>()`. Tampoco afecta a `FromJson<T>`/
`FromJsonFileAsync<T>` (deserialización): solo bloquea la dirección de *salida*. Útil para
columnas sensibles que necesitás leer de la base pero nunca deberían salir en una respuesta
expuesta hacia afuera (ej: un hash de password):

```csharp
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    [Column("password_hash")]
    [NotSerialized]
    public string? PasswordHash { get; set; }
}

var user = result.FirstOrDefault<User>();
user.PasswordHash;       // viene de la base, tiene el valor real
user.ToJsonString();     // el JSON no incluye "PasswordHash"
```

Si ya pasás tu propio `JsonSerializerOptions` a estos métodos, `[NotSerialized]` se sigue
respetando siempre que ese `JsonSerializerOptions` no traiga su propio `TypeInfoResolver`
configurado — en ese caso se asume una configuración deliberada tuya y no se toca.

## Fechas compatibles con JavaScript: [JsDateTime]

Por defecto, `System.Text.Json` serializa un `DateTime` sin indicar zona horaria (ej.
`2026-07-08T10:58:53.16`). Un cliente JavaScript que haga `new Date("2026-07-08T10:58:53.16")`
lo interpreta como **hora local del navegador**, no UTC — el mismo valor se ve distinto según en
qué huso horario esté quien lo consuma. `[JsDateTime]` fuerza el sufijo `Z` (ISO-8601 UTC), que
`new Date(...)` interpreta sin ambigüedad:

```csharp
public class Order
{
    [Column("order_date")]
    [JsDateTime]
    public DateTime OrderDate { get; set; }
}

order.ToJsonString(); // "OrderDate": "2026-07-08T10:58:53.160Z"
```

Funciona igual sobre `DateTime?`. Asume que el valor ya representa UTC (como un `DATETIME` de
SQL Server, que llega con `DateTimeKind.Unspecified` pero sin desplazamiento de por medio) y lo
marca como tal al serializar — **no convierte** una hora local a UTC en ningún caso, ni siquiera
con un formato personalizado (ver abajo). Solo afecta la serialización (mismo alcance que
`[NotSerialized]`): la deserialización con `FromJson`/`FromJsonFileAsync` sigue aceptando
cualquier formato ISO-8601 válido con normalidad.

### Formato personalizado

El default (`[JsDateTime]` sin argumento, o `[JsDateTime("ISO-8601")]` de forma explícita) es
`yyyy-MM-ddTHH:mm:ss.fffK`. Cualquier otro valor se pasa tal cual a
`DateTime.ToString(formato, CultureInfo.InvariantCulture)` — cualquier cadena de formato estándar
o personalizada de `System.Globalization` que uses normalmente en C#, siempre en cultura
invariante (un JSON no debería cambiar de forma según el servidor que lo genera) y siempre sobre
el valor ya normalizado a UTC:

```csharp
[JsDateTime("yyyy-MM-dd")]   // "2026-07-08" (solo fecha, sin hora)
[JsDateTime("o")]            // round-trip de .NET, con más precisión de fracción de segundo
[JsDateTime("R")]            // RFC1123
```

## Procedimientos con parámetros de salida (Output / InputOutput / ReturnValue)

Los parámetros creados con `SqlParams.OutParam`, `SqlParams.InOutParam` o `SqlParams.ReturnParam`
se combinan con los de entrada en el mismo arreglo. Después de ejecutar, el valor de salida se lee
desde el resultado (`SqlQueryResult`), no desde la variable original del parámetro:

```csharp
var parameters = SqlParams.AddParams(("UserId", 1))
    .Append(SqlParams.OutParam("TotalOrders", SqlDbType.Int))
    .ToArray();

var result = await db.ExecuteAsync("sp_GetUserOrderTotal", parameters);

var total = result.GetOutputValue<int>("TotalOrders"); // funciona con o sin "@"
bool huboSalida = result.HasOutputParameters;
foreach (var (name, value) in result.OutputParameters)
{
    Console.WriteLine($"{name} = {value}");
}
```

Esto funciona igual para `QueryAsync`/`ExecuteAsync` (async) y `Query`/`Execute` (sync). Si el
procedimiento no asigna un valor a un parámetro de salida, `GetOutputValue<T>` devuelve el valor
por defecto de `T` en vez de lanzar una excepción.

### ⚠️ Output decimal: siempre con precisión y escala

Un `SqlParameter` `SqlDbType.Decimal` sin `Precision`/`Scale` explícitas asume `Scale = 0`, y
ADO.NET **redondea el valor de salida a un entero sin avisar** (`99.97` vuelve `100`, sin
excepción ni warning). Usa siempre la sobrecarga con precisión/escala para output decimales:

```csharp
// ❌ Riesgoso: Scale=0 implícito trunca/redondea el decimal a entero
SqlParams.OutParam("Total", SqlDbType.Decimal)

// ✅ Correcto
SqlParams.OutParam("Total", SqlDbType.Decimal, precision: 10, scale: 2)
// o, sobre un OutParam ya creado:
SqlParams.OutParam("Total", SqlDbType.Decimal).WithPrecision(10, 2)
```

### Reusar el mismo arreglo de parámetros entre ejecuciones

El mismo `SqlParameter[]` se puede reusar de forma segura en múltiples ejecuciones (secuenciales,
en un loop, o en llamadas concurrentes) sin necesidad de reconstruirlo cada vez — internamente,
`SqlCommandExecutor` clona cada parámetro antes de agregarlo al comando (un `SqlParameter` solo
puede pertenecer a una `SqlParameterCollection` a la vez) y copia los valores de salida de vuelta
al original después de cada ejecución:

```csharp
var idParam = SqlParams.Param("UserId", 0);
var totalParam = SqlParams.OutParam("TotalOrders", SqlDbType.Int);
var parameters = new[] { idParam, totalParam };

foreach (var userId in userIds)
{
    idParam.Value = userId; // mutar antes de cada ejecución
    var result = await db.ExecuteAsync("sp_GetUserOrderTotal", parameters);
    Console.WriteLine($"Usuario {userId}: {result.GetOutputValue<int>("TotalOrders")}");
}
```
