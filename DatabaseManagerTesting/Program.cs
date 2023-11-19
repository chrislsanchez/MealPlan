using SQLite;

[StoreAsText]
public enum Unit
{
    grams,
    pieces,
    cups
}

public enum SupermarketSection
{
    Other, // Other Area
    Fruits_and_Vegetables, // Obst und Gemüse Bread
    Brot_Bakery, // Backwaren
    Meat_and_Sausages, // Fleisch und Wurst
    Dairy_Products, // Milchprodukte
    Frozen_Foods, // Tiefkühlkost
    Sweets, // Süßwaren
    Beverages, // Getränke
    Household_Items, // Haushaltswaren
    Hygiene_Products, // Hygieneartikel1
    Canned_Goods, // Konserven
    Pasta_and_Rice, // Nudeln und Reis
    Oils_Sauces_and_Spices // Öle, Soßen & Gewürze
}

public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public Unit Unit { get; set; }
    public SupermarketSection WhereToFind { get; set; }
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

public class GroceryListItem
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public int IngredientID { get; set; }
    public double Quantity { get; set; }
    public bool IsBought { get; set; }
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
        _database.CreateTableAsync<GroceryListItem>().Wait();
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
        if (groceryListItems == null || !groceryListItems.Any())
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



class Program
{
    static async Task Main(string[] args)
    {

        Console.WriteLine("DATABASE TEST PLAYGROUND\n");


        Console.WriteLine("Creating or opening the database...");
        string dbFileName = "RecipesDataBase.db";
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);

        // Initialize the database service
        RecipeDatabaseService dbService;

        try
        {
            dbService = new RecipeDatabaseService(dbPath);
            Console.WriteLine(" Succeeded. n");

            Console.WriteLine("Testing Ingredient Creation...");
            Ingredient ingredient1, ingredient2, ingredient3;
       
            // Add three ingredients to the database
            ingredient1 = new Ingredient { Name = "Cheese", Unit = Unit.grams, WhereToFind = SupermarketSection.Dairy_Products };
            ingredient2 = new Ingredient { Name = "Pumpkin", Unit = Unit.pieces, WhereToFind = SupermarketSection.Fruits_and_Vegetables };
            ingredient3 = new Ingredient { Name = "Olives Oil", Unit = Unit.cups, WhereToFind = SupermarketSection.Oils_Sauces_and_Spices };

            await dbService.AddOrUpdateIngredientAsync(ingredient1);
            await dbService.AddOrUpdateIngredientAsync(ingredient2);
            await dbService.AddOrUpdateIngredientAsync(ingredient3);

            Console.WriteLine(" Succeeded. \n");


            Console.WriteLine("Testing Recipe Creation...");
            Recipe recipe1, recipe2;

            // Create two recipes using two ingredients each
            recipe1 = new Recipe { Name = "Recipe 1", Preparation = "Prepare Recipe 1", Portions = 4 };
            recipe2 = new Recipe { Name = "Recipe 4", Preparation = "Prepare Recipe 2", Portions = 2 };

            // bug here. While associating recipeingredients the recipe1 ID is always 0. The Id needs to be readed from the table searching by name.

            await dbService.AddOrUpdateRecipeAsync(recipe1);
            await dbService.AddOrUpdateRecipeAsync(recipe2);
            Console.WriteLine(" Succeeded. \n");


            Console.WriteLine("Testing Recipe - ingredients association...");
            RecipeIngredient recipeIngredient1, recipeIngredient2, recipeIngredient3, recipeIngredient4;
        
            // Associate ingredients with recipes
            recipeIngredient1 = new RecipeIngredient { RecipeID = recipe1.ID, IngredientID = ingredient1.ID, Quantity = 200 };
            recipeIngredient2 = new RecipeIngredient { RecipeID = recipe1.ID, IngredientID = ingredient2.ID, Quantity = 3 };
            recipeIngredient3 = new RecipeIngredient { RecipeID = recipe2.ID, IngredientID = ingredient2.ID, Quantity = 2 };
            recipeIngredient4 = new RecipeIngredient { RecipeID = recipe2.ID, IngredientID = ingredient3.ID, Quantity = 1 };

            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient1);
            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient2);
            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient3);
            await dbService.AddOrUpdateRecipeIngredientAsync(recipeIngredient4);
            Console.WriteLine(" Succeeded. \n");


            Console.WriteLine("---------------------------\n Recipes and Ingredients \n---------------------------\n");
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
            Console.WriteLine("\n");

            Console.WriteLine("Test Recipe Deletion...\n");

            Console.WriteLine("Deleting Recipe 1 by name...");
            // Delete one of the recipes and its relationships
            await dbService.DeleteRecipeAsync("Recipe 1");
            //await dbService.DeleteRecipeAsync(recipeToDelete);
            Console.WriteLine(" Succeeded. \n");

            Console.WriteLine("---------------------------\n Recipes and Ingredients \n---------------------------\n");
            recipes = await dbService.GetAllRecipesAsync();
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
            Console.WriteLine("\n");


            Console.WriteLine("Creating some Meals...");
            Meal meal1, meal2;
        
            // create a couple of meals
            // Create two meals for different dates
            meal1 = new Meal { Date = DateTime.Now.Date, RecipeID = recipe1.ID, Portions = recipe1.Portions };
            meal2 = new Meal { Date = DateTime.Now.AddDays(1).Date, RecipeID = recipe2.ID, Portions = recipe2.Portions };

            await dbService.AddMealAsync(meal1);
            await dbService.AddMealAsync(meal2);
        
            Console.WriteLine(" Succeeded. \n");


            Console.WriteLine("Creating the Grocery List");
            // Generate a grocery list for a date range
            var startDate = DateTime.Now.Date;
            var endDate = DateTime.Now.AddDays(2).Date;

            var groceryList = await dbService.GenerateGroceryListAsync(startDate, endDate);

            foreach (var groceryItem in groceryList)
            {
                Ingredient ingredient = await dbService.GetIngredientByID(groceryItem.IngredientID);

                if (ingredient != null)
                {
                    Console.WriteLine($"Item: {ingredient.Name}");
                    Console.WriteLine($"Quantity: {groceryItem.Quantity} ({ingredient.Unit})");
                    Console.WriteLine($"Where to Find: {ingredient.WhereToFind}");
                    Console.WriteLine($"Is Bought: {groceryItem.IsBought}");
                    Console.WriteLine("----------------------");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed. An error occurred: {ex.Message}");
        }
    }
}