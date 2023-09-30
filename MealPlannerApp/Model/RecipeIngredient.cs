namespace MealPlannerApp.Model;

[Table(nameof(RecipeIngredient))]
public class RecipeIngredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    //[ForeignKey(nameof(Recipe))] // Defines a foreign key relationship with RecipeID.
    public int RecipeID { get; set; }
    public int IngredientID { get; set; }
    public double Quantity { get; set; }
}
