#OtherInformation: <Nullable>enable</Nullable>
[]: # OtherInformation: <ImplicitUsings>enable</ImplicitUsings>

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