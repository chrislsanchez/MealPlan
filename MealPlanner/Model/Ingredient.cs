namespace MealPlanner.Model;

[StoreAsText]
public enum Unit
{
    grams,
    pieces,
    cups
}


public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public Unit Unit { get; set; }
    public string WhereToFind { get; set; } = string.Empty;
}
