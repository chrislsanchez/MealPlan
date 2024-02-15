namespace MealPlanner.Model;
public class Meal
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public DateTime Date { get; set; }
    public int RecipeID { get; set; } // Reference to the recipe used for the meal
    public int Portions { get; set; } // Portion specification for this meal
}
