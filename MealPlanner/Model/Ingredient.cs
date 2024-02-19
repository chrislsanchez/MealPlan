namespace MealPlanner.Model;

/// <summary>
/// The Ingredient Class for Database Storage
/// </summary>
public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public Unit Unit { get; set; }
    public SupermarketSection WhereToFind { get; set; }
}

/// <summary>
/// Class containing the Ingreient info to display in the UI
/// </summary>
public partial class IngredientInfo() : ObservableObject
{
    [ObservableProperty]
    Unit unit;
    [ObservableProperty]
    float quantity;
    [ObservableProperty]
    string? name;

    public IngredientInfo(string name, Unit unit, float quantity) : this()
    {
        Name = name;
        Unit = unit;
        Quantity = quantity;
    }

}