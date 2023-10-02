namespace MealPlannerApp.Model;
public class Recipe
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PicturePath { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public int Portions { get; set; }
}
