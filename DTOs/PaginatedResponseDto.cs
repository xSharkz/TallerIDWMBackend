/// <summary>
/// Representa una respuesta paginada para una colección de elementos de tipo genérico.
/// </summary>
/// <typeparam name="T">El tipo de los elementos en la colección.</typeparam>
public class PaginatedResponseDto<T>
{
    /// <summary>
    /// Obtiene o establece la lista de elementos de la página actual.
    /// </summary>
    /// <remarks>
    /// Esta propiedad contiene los elementos específicos de la página solicitada, del tipo especificado en el parámetro genérico <typeparamref name="T"/>.
    /// </remarks>
    public List<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Obtiene o establece el número total de páginas disponibles en la paginación.
    /// </summary>
    /// <remarks>
    /// Este valor se calcula en función del número total de elementos y la cantidad de elementos por página.
    /// </remarks>
    public int TotalPages { get; set; }

    /// <summary>
    /// Obtiene o establece el número de la página actual que se está visualizando.
    /// </summary>
    /// <remarks>
    /// Esta propiedad indica qué página está siendo consultada actualmente en la respuesta paginada.
    /// </remarks>
    public int CurrentPage { get; set; }
}
