namespace MealPlannerApp.Model;

[Table(nameof(Ingredient))]
public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Text { get; set; }
    public string Unit { get; set; }
    public string WhereToFind { get; set; }
}
