using System.Diagnostics;

namespace MealPlanner.Services;

public class RecipeDatabaseService
{
    private SQLiteAsyncConnection _database;
    public string DatabasePath { get => _databasePath; }
    public string DatabaseName = "MealPlanner.db";
    private string _databasePath = Path.Combine(FileSystem.Current.AppDataDirectory, "MealPlanner.db");

    public RecipeDatabaseService()
    {
        _databasePath = Path.Combine(FileSystem.Current.AppDataDirectory, DatabaseName);
        _database = new SQLiteAsyncConnection(_databasePath);
        _database.CreateTableAsync<Ingredient>().Wait();
        InitializeRecipesTable(); // Call a method to set up the Recipe table
        _database.CreateTableAsync<RecipeIngredient>().Wait();
        _database.CreateTableAsync<Meal>().Wait();
        _database.CreateTableAsync<GroceryListItem>().Wait();
    }

    public RecipeDatabaseService(string dbName) : this()
    {
        DatabaseName = dbName;
    }

    /// <summary>
    /// Initialize the Recipes table with custom data types. The TEXT datatype has been assigned to the preparation so it accepts longer strings
    /// </summary>
    private void InitializeRecipesTable()
    {
        if (_database is null)
        {
            Debug.WriteLine("APP DEBUG: the databse is null. Error");
            return;
        }

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
    /// <summary>
    /// gets and retursn all the ingredients from the database
    /// </summary>
    /// <returns></returns>
    public async Task<List<Ingredient>> GetAllIngredientsAsync()
    {
        return await _database.Table<Ingredient>().ToListAsync();
    }

    /// <summary>
    /// returns a list of all recepies
    /// </summary>
    /// <returns></returns>
    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        return await _database.Table<Recipe>().ToListAsync();
    }

    /// <summary>
    /// returns a list of all recipe ingrediennt relationships
    /// </summary>
    /// <returns></returns>
    public async Task<List<RecipeIngredient>> GetAllRecipeIngredientsAsync()
    {
        return await _database.Table<RecipeIngredient>().ToListAsync();
    }

    /// <summary>
    /// Get an ingredient by id
    /// </summary>
    /// <param name="ingredientID"></param>
    /// <returns></returns>
    public async Task<Ingredient> GetIngredientByID(int ingredientID)
    {
        Ingredient ingredient = await _database.Table<Ingredient>().Where(i => i.ID == ingredientID).FirstOrDefaultAsync();

        return ingredient;
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


    public async Task<int> AddGroceryListItemAsync(Ingredient ingredient, double quantity)
    {
        var groceryListItem = new GroceryListItem
        {
            IngredientID = ingredient.ID,
            Quantity = quantity,
            IsBought = false
        };

        return await _database.InsertAsync(groceryListItem);
    }

    public async Task<int> AddGroceryList(List<GroceryListItem> groceryListItems)
    {
        if (groceryListItems == null || groceryListItems.Count() == 0)
        {
            return 0; // No items to add
        }

        foreach (var groceryListItem in groceryListItems)
        {
            await _database.InsertAsync(groceryListItem);
        }

        return groceryListItems.Count;
    }

    public async Task MarkGroceryItemAsBoughtAsync(int groceryListItemID)
    {
        var groceryListItem = await _database.Table<GroceryListItem>().Where(item => item.ID == groceryListItemID).FirstOrDefaultAsync();

        if (groceryListItem != null)
        {
            groceryListItem.IsBought = true;
            await _database.UpdateAsync(groceryListItem);
        }
    }

    public async Task MarkGroceryItemAsBoughtAsync(string ingredientName, double quantity)
    {
        var groceryItem = await _database.Table<GroceryListItem>()
            .Where(item => item.IsBought == false) // Consider only items that are not bought
            .FirstOrDefaultAsync();

        if (groceryItem != null)
        {
            // Check if the ingredient name and quantity match
            var ingredient = await _database.Table<Ingredient>()
                .Where(i => i.Name == ingredientName && i.ID == groceryItem.IngredientID)
                .FirstOrDefaultAsync();

            if (ingredient != null && groceryItem.Quantity == quantity)
            {
                // Mark the item as bought
                groceryItem.IsBought = true;
                await _database.UpdateAsync(groceryItem);
            }
        }
    }
    public async Task MarkGroceryItemAsNOTBoughtAsync(string ingredientName, double quantity)
    {
        var groceryItem = await _database.Table<GroceryListItem>()
            .Where(item => item.IsBought == true) // Consider only items that are bought
            .FirstOrDefaultAsync();

        if (groceryItem != null)
        {
            // Check if the ingredient name and quantity match
            var ingredient = await _database.Table<Ingredient>()
                .Where(i => i.Name == ingredientName && i.ID == groceryItem.IngredientID)
                .FirstOrDefaultAsync();

            if (ingredient != null && groceryItem.Quantity == quantity)
            {
                // Mark the item as not bought
                groceryItem.IsBought = false;
                await _database.UpdateAsync(groceryItem);
            }
        }
    }

    public async Task ClearGroceryListAsync()
    {
        await _database.DropTableAsync<GroceryListItem>();
        await _database.CreateTableAsync<GroceryListItem>();
    }

    public async Task<List<GroceryListItem>> GenerateGroceryListAsync(DateTime startDate, DateTime endDate)
    {
        var meals = await _database.Table<Meal>()
            .Where(m => m.Date >= startDate && m.Date <= endDate)
            .ToListAsync();

        var groceryList = new List<GroceryListItem>();

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

                        // Add grocery list item to the database
                        await AddGroceryListItemAsync(ingredient, adjustedQuantity);

                        // Create GroceryListItem object for the result
                        var groceryListItem = new GroceryListItem
                        {
                            IngredientID = ingredient.ID,
                            Quantity = adjustedQuantity,
                            IsBought = false
                        };

                        groceryList.Add(groceryListItem);
                    }
                }
            }
        }

        return groceryList;
    }

    #endregion

}