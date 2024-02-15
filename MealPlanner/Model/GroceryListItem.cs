namespace MealPlanner.Model;
public class GroceryListItem
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public int IngredientID { get; set; }
    public double Quantity { get; set; }
    public bool IsBought { get; set; }
}
