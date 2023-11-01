using SQLite;

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

public class Recipe
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PicturePath { get; set; } = string.Empty;
    public string Preparation { get; set; } = string.Empty;
    public int Portions { get; set; }
}

public class RecipeIngredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public int RecipeID { get; set; }
    public int IngredientID { get; set; }
    public int Quantity { get; set; }
}

public class Meal
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public DateTime Date { get; set; }
    public int RecipeID { get; set; } // Reference to the recipe used for the meal
    public int Portions { get; set; } // Portion specification for this meal
}

public class GroceryList
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public DateTime StartDate { get; set; } // Start date of the date range
    public DateTime EndDate { get; set; } // End date of the date range
}


public class RecipeDatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    public RecipeDatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Ingredient>().Wait();
        InitializeRecipesTable(); // Call a method to set up the Recipe table
        _database.CreateTableAsync<RecipeIngredient>().Wait();
        _database.CreateTableAsync<Meal>().Wait();
        _database.CreateTableAsync<GroceryList>().Wait();
    }

    /// <summary>
    /// Initialize the Recipes table with custom data types. The TEXT datatype has been assigned to the preparation so it accepts longer strings
    /// </summary>
    private void InitializeRecipesTable()
    {
        _database.ExecuteAsync(
            $@"CREATE TABLE IF NOT EXISTS {nameof(Recipe)} (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                {nameof(Recipe.Name)} varchar,
                {nameof(Recipe.PicturePath)} varchar,
                {nameof(Recipe.Preparation)} TEXT,
                {nameof(Recipe.Portions)} INTEGER
            )").Wait();
    }


    #region Read
    public async Task<List<Ingredient>> GetAllIngredientsAsync()
    {
        return await _database.Table<Ingredient>().ToListAsync();
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        return await _database.Table<Recipe>().ToListAsync();
    }

    public async Task<List<RecipeIngredient>> GetAllRecipeIngredientsAsync()
    {
        return await _database.Table<RecipeIngredient>().ToListAsync();
    }
    #endregion

    #region Create Update
    public async Task<int> AddOrUpdateRecipeAsync(Recipe recipe)
    {
        var existingRecipe = await _database.Table<Recipe>().Where(r => r.Name == recipe.Name).FirstOrDefaultAsync();
        if (existingRecipe == null)
        {
            return await _database.InsertAsync(recipe);
        }
        else
        {
            recipe.ID = existingRecipe.ID; // Update the existing recipe's ID
            return await _database.UpdateAsync(recipe);
        }
    }

    public async Task<int> AddOrUpdateIngredientAsync(Ingredient ingredient)
    {
        var existingIngredient = await _database.Table<Ingredient>().Where(i => i.Name == ingredient.Name).FirstOrDefaultAsync();
        if (existingIngredient == null)
        {
            return await _database.InsertAsync(ingredient);
        }
        else
        {
            ingredient.ID = existingIngredient.ID; // Update the existing using ID
            return await _database.UpdateAsync(ingredient);
        }
    }

    public async Task<int> AddOrUpdateRecipeIngredientAsync(RecipeIngredient recipeIngredient)
    {
        var existingRelationship = await _database.Table<RecipeIngredient>()
            .Where(ri => ri.RecipeID == recipeIngredient.RecipeID && ri.IngredientID == recipeIngredient.IngredientID)
            .FirstOrDefaultAsync();

        if (existingRelationship == null)
        {
            return await _database.InsertAsync(recipeIngredient);
        }
        else
        {
            recipeIngredient.ID = existingRelationship.ID; // Update the existing relationship's ID
            return await _database.UpdateAsync(recipeIngredient);
        }
    }
    #endregion

    #region Delete
    public async Task<int> DeleteRecipeAsync(string recipeName)
    {
        var recipeToDelete = await _database.Table<Recipe>().Where(r => r.Name == recipeName).FirstOrDefaultAsync();
        if (recipeToDelete != null)
        {
            await _database.ExecuteAsync($"DELETE FROM {nameof(RecipeIngredient)} WHERE RecipeID = {recipeToDelete.ID}");
            // delete from recipe table
            return await _database.DeleteAsync(recipeToDelete);
        }
        return 0;
    }
    #endregion




    #region Meals

    public async Task<int> AddMealAsync(Meal meal)
    {
        return await _database.InsertAsync(meal);
    }

    public async Task<int> UpdateMealAsync(Meal meal)
    {
        return await _database.UpdateAsync(meal);
    }

    public async Task<int> DeleteMealAsync(int mealID)
    {
        return await _database.DeleteAsync<Meal>(mealID);
    }

    public async Task<List<Meal>> GetMealsForDateAsync(DateTime date)
    {
        return await _database.Table<Meal>().Where(m => m.Date == date).ToListAsync();
    }

    #endregion


    #region Grocery List

    public async Task<int> AddGroceryListAsync(GroceryList groceryList)
        {
            return await _database.InsertAsync(groceryList);
        }

        public async Task<List<GroceryList>> GetGroceryListsAsync()
        {
            return await _database.Table<GroceryList>().ToListAsync();
        }

    public async Task<List<(Ingredient, double)>> GenerateGroceryListAsync(DateTime startDate, DateTime endDate)
    {
        var meals = await _database.Table<Meal>()
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .ToListAsync();

        var groceryList = new List<(Ingredient, double)>();

        foreach (var meal in meals)
        {
            var recipeIngredients = await _database.Table<RecipeIngredient>()
                .Where(ri => ri.RecipeID == meal.RecipeID)
                .ToListAsync();

            var recipe = await _database.Table<Recipe>().Where(r => r.ID == meal.RecipeID).FirstOrDefaultAsync();

            if (recipe != null)
            {
                var portionMultiplier = meal.Portions / (double)recipe.Portions;

                foreach (var recipeIngredient in recipeIngredients)
                {
                    var ingredient = await _database.Table<Ingredient>().Where(i => i.ID == recipeIngredient.IngredientID).FirstOrDefaultAsync();

                    if (ingredient != null)
                    {
                        // Adjust the quantity based on portionMultiplier
                        double adjustedQuantity = recipeIngredient.Quantity * portionMultiplier;
                        groceryList.Add((ingredient, adjustedQuantity));
                    }
                }
            }
        }

        return groceryList;
    }





    #endregion




}



class Program
{
    static async Task Main(string[] args)
    {

        string dbFileName = "RecipesDataBase.db";
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);

        // Initialize the database service
        var dbService = new RecipeDatabaseService(dbPath);

        try
        {
            // Add three ingredients to the database
            var ingredient1 = new Ingredient { Name = "Ingredient 1", Unit = Unit.pieces, WhereToFind = "Store 5" };
            var ingredient2 = new Ingredient { Name = "Ingredient 2", Unit = Unit.pieces, WhereToFind = "Store 2" };
            var ingredient3 = new Ingredient { Name = "Ingredient 3", Unit = Unit.cups, WhereToFind = "Store 3" };

            await dbService.AddOrUpdateIngredientAsync(ingredient1);
            await dbService.AddOrUpdateIngredientAsync(ingredient2);
            await dbService.AddOrUpdateIngredientAsync(ingredient3);

            // Create two recipes using two ingredients each
            var recipe1 = new Recipe { Name = "Recipe 1", Preparation = "Prepare Recipe 1", Portions = 4 };
            var recipe2 = new Recipe { Name = "Recipe 4", Preparation = "Prepare Recipe 2", Portions = 2 };

            // bug here. While associating recipeingredients the recipe1 ID is always 0. The Id needs to be readed from the table searching by name.

            await dbService.AddOrUpdateRecipeAsync(recipe1);
            await dbService.AddOrUpdateRecipeAsync(recipe2);


            // Associate ingredients with recipes
            var recipeIngredient1 = new RecipeIngredient { RecipeID = recipe1.ID, IngredientID = ingredient1.ID, Quantity = 200 };
            var recipeIngredient2 = new RecipeIngredient { RecipeID = recipe1.ID, IngredientID = ingredient2.ID, Quantity = 3 };
            var recipeIngredient3 = new RecipeIngredient { RecipeID = recipe2.ID, IngredientID = ingredient2.ID, Quantity = 2 };
            var recipeIngredient4 = new RecipeIngredient { RecipeID = recipe2.ID, IngredientID = ingredient3.ID, Quantity = 1 };



            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient1);
            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient2);
            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient3);
            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient4);


            Console.WriteLine("Added ingredients and recipes. Press a key to continue");
            Console.ReadKey();

            // List all recipes and ingredients
            Console.WriteLine("List all recipes and ingredients");
            var recipes = await dbService.GetAllRecipesAsync();
            foreach (var r in recipes)
            {
                Console.WriteLine($"Recipe: {r.Name}");
                var recipeIngredients = await dbService.GetAllRecipeIngredientsAsync();
                var ingredients = await dbService.GetAllIngredientsAsync();
                foreach (var ri in recipeIngredients)
                {
                    if (ri.RecipeID == r.ID)
                    {
                        var ingredient = ingredients.Find(i => i.ID == ri.IngredientID);
                        if (ingredient is not null)
                            Console.WriteLine($"  - {ingredient.Name}: {ri.Quantity} {ingredient.Unit}");
                    }
                }
            }


            Console.WriteLine("press to delete Recipe 1");
            Console.ReadKey();

            // Delete one of the recipes and its relationships
            //await dbService.DeleteRecipeAsync("Recipe 1");
            //await dbService.DeleteRecipeAsync(recipeToDelete);

            Console.WriteLine("Deleting Recipe 1");
            Console.ReadKey();



            // List remaining recipes and ingredients
            recipes = await dbService.GetAllRecipesAsync();
            Console.WriteLine("\n\nRecipes and Ingredients after deletion:");
            foreach (var r in recipes)
            {
                Console.WriteLine($"Recipe: {r.Name}");
                var recipeIngredients = await dbService.GetAllRecipeIngredientsAsync();
                var ingredients = await dbService.GetAllIngredientsAsync();
                foreach (var ri in recipeIngredients)
                {
                    if (ri.RecipeID == r.ID)
                    {
                        var ingredient = ingredients.Find(i => i.ID == ri.IngredientID);
                        if(ingredient is not null )
                            Console.WriteLine($"  - {ingredient.Name}: {ri.Quantity} {ingredient.Unit}");
                    }
                }
            }

            // create a couple of meals
            // Create two meals for different dates
            var meal1 = new Meal { Date = DateTime.Now.Date, RecipeID = recipe1.ID, Portions = recipe1.Portions };
            var meal2 = new Meal { Date = DateTime.Now.AddDays(1).Date, RecipeID = recipe2.ID, Portions = recipe2.Portions };

            await dbService.AddMealAsync(meal1);
            await dbService.AddMealAsync(meal2);

            // Generate a grocery list for a date range
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.AddDays(2).Date;

            var groceryList = await dbService.GenerateGroceryListAsync(startDate, endDate);

            Console.WriteLine("Grocery List for the specified date range:");
            foreach (var (ingredient, quantity) in groceryList)
            {
                Console.WriteLine($"{ingredient.Name} ({ingredient.Unit}): {quantity} {ingredient.WhereToFind}");
            }



        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


}