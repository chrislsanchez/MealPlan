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




public class RecipeDatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    public RecipeDatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Ingredient>().Wait();
        InitializeRecipesTable(); // Call a method to set up the Recipe table
        _database.CreateTableAsync<RecipeIngredient>().Wait();
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
            await dbService.DeleteRecipeAsync("Recipe 1");
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


}