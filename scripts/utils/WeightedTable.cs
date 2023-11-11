using Godot;
using Godot.Collections;
using System;
using System.Linq;

/**
    Una WeightedTable es una estructura de datos que te permite almacenar elementos con pesos asociados,
    de tal manera que puedes seleccionar un elemento aleatorio considerando esos pesos. En otras palabras,
    es una tabla donde cada elemento tiene una probabilidad definida (su "peso") de ser seleccionado.

    Por ejemplo, podrías tener una WeightedTable con tres elementos: A, B, y C, con pesos de 1, 2, y 3,
    respectivamente. Al llamar a una función que selecciona un elemento aleatorio de esta tabla,
    A tendría 1 oportunidad, B tendría 2 oportunidades y C tendría 3 oportunidades. Entonces,
    la probabilidad de seleccionar A sería 1/6, la de seleccionar B sería 2/6 y la de seleccionar C sería 3/6.

    En el contexto de desarrollo de videojuegos, esto se utiliza a menudo para sistemas de botín ("loot"),
    generación procedural de enemigos, eventos aleatorios, etc. Permite una mayor flexibilidad y control
    sobre la aleatoriedad en el juego.
*/
public partial class WeightedTable
{
    // Inicializa el generador de números aleatorios.
    private static Random RandomProp { get; set; } = new Random();

    // Tabla para almacenar los elementos. Cada elemento es un diccionario.
    public Array<Dictionary> Table { get; private set; } = new Array<Dictionary>();

    // Suma total de los pesos para todos los elementos en la tabla.
    public int WeightSum { get; private set; } = 0;

    // Constructor por defecto
    public WeightedTable() {}

    // Función para generar un número entero aleatorio en un rango.
    private static int RandiRange(int minValue, int maxValue) => RandomProp.Next(minValue, maxValue + 1);
    
    // Agrega un nuevo elemento a la tabla junto con su peso.
    public void AddItem(string id, Variant item, int weight)
    {
        // Agrega el elemento y su peso en forma de diccionario a la tabla.
        Dictionary entry = new Dictionary();
        Table.Add(new Dictionary(){
            {"id", id},          // Identificador del elemento.
            {"item", item},      // El elemento en sí.
            {"weight", weight}   // Peso del elemento.
        });

        // Aumenta la suma total de pesos.
        WeightSum += weight;
    }

    // Elimina un elemento de la tabla según su identificador.
    public bool RemoveItem(string id)
    {
        // Busca el elemento en la tabla usando LINQ.
        #nullable enable
        Dictionary? foundEntry = Table.FirstOrDefault(entry => (string)entry["id"] == id);

        // Si no se encuentra el elemento, retorna falso.
        if (foundEntry == null) return false;
        
        // Si se encuentra el elemento...
        // Disminuye la suma total de los pesos.
        WeightSum -= (int)foundEntry["weight"];
        
        // Elimina el elemento de la tabla.
        Table.Remove(foundEntry);

        // Retorno de éxito.
        return true;
    }

    // Escoge un elemento aleatorio de la tabla, considerando los pesos y los elementos a excluir.
    #nullable enable
    public Variant? PickItem(Array<string>? itemsToExclude = null)
    {
        // Inicializa la lista de elementos a excluir si es nula.
        itemsToExclude ??= new Array<string>();

        // Obtiene la tabla y la suma de pesos ajustadas (sin los elementos a excluir).
        var (adjustedTable, adjustedWeightSum) = GetAdjustedTable(itemsToExclude);

        // Genera un número aleatorio entre 1 y la suma de pesos ajustada.
        int chosenWeight = RandiRange(1, adjustedWeightSum);
        int currentWight = 0;

        // Buscar sobre la tabla ajustada.
        Dictionary? chosenItem = adjustedTable.FirstOrDefault(entry => {
            // Suma los pesos mientras avanza.
            currentWight += (int)entry["weight"];

            // Si el peso acumulado supera o iguala al peso elegido aleatoriamente, retorna el elemento.
            return chosenWeight <= currentWight;
        });

        // Si no encuentra ningún elemento, retorna nulo.
        return chosenItem?["item"];
    }

    // Obtiene una tabla y una suma de pesos ajustadas, excluyendo ciertos elementos.
    private (Array<Dictionary> adjustedTable, int adjustedWeightSum) GetAdjustedTable(Array<string> itemsToExclude)
    {
        // Crea una nueva tabla sin los elementos a excluir.
        Array<Dictionary> adjustedTable = new Array<Dictionary>(
            Table.Where(entry => !itemsToExclude.Contains((string)entry["id"]))
        );

        // Calcula la nueva suma de pesos para la tabla ajustada.
        int adjustedWeightSum = adjustedTable.Sum(entry => (int)entry["weight"]);
        
        // Retorna la tabla y la suma de pesos ajustadas.
        return (adjustedTable, adjustedWeightSum);
    }
}