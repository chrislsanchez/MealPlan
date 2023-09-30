namespace MealPlannerApp.Model;

[Table(nameof(Recipe))]
public class Recipe
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; }
    public string PicturePath { get; set; }
    public string Preparation { get; set; }
    public int Portions { get; set; }
}
