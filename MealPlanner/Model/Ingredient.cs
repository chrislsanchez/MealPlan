namespace MealPlanner.Model;

public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public Unit Unit { get; set; }
    public SupermarketSection WhereToFind { get; set; }
}