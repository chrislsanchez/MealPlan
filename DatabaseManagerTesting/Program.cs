using SQLite;

public class Ingredient
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Text { get; set; }
    public string Unit { get; set; }
    public string WhereToFind { get; set; }
}

public class Recipe
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Name { get; set; }
    public string PicturePath { get; set; }
    public string Preparation { get; set; }
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
        _database.CreateTableAsync<Recipe>().Wait();
        _database.CreateTableAsync<RecipeIngredient>().Wait();
    }

    // Create
    public async Task<int> AddOrUpdateIngredientAsync(Ingredient ingredient)
    {
        var existingIngredient = await _database.Table<Ingredient>().Where(i => i.Text == ingredient.Text).FirstOrDefaultAsync();
        if (existingIngredient != null)
        {
            ingredient.ID = existingIngredient.ID;
            return await _database.UpdateAsync(ingredient);
        }
        else
        {
            return await _database.InsertAsync(ingredient);
        }
    }

    public async Task<int> AddIngredientAsync(Ingredient ingredient)
    {
        return await _database.InsertAsync(ingredient);
    }

    public async Task<int> AddRecipeAsync(Recipe recipe)
    {
        return await _database.InsertAsync(recipe);
    }

    public async Task<int> AddRecipeIngredientAsync(RecipeIngredient recipeIngredient)
    {
        return await _database.InsertAsync(recipeIngredient);
    }

    // Read
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

    // Update
    public async Task<int> UpdateIngredientAsync(Ingredient ingredient)
    {
        return await _database.UpdateAsync(ingredient);
    }

    public async Task<int> UpdateRecipeAsync(Recipe recipe)
    {
        return await _database.UpdateAsync(recipe);
    }

    public async Task<int> UpdateRecipeIngredientAsync(RecipeIngredient recipeIngredient)
    {
        return await _database.UpdateAsync(recipeIngredient);
    }

    // Delete
    public async Task<int> DeleteIngredientAsync(Ingredient ingredient)
    {
        return await _database.DeleteAsync(ingredient);
    }

    // Delete Recipe and Relationships
    public async Task<int> DeleteRecipeAsync(Recipe recipe)
    {
        var recipeIngredients = await _database.Table<RecipeIngredient>().Where(ri => ri.RecipeID == recipe.ID).ToListAsync();
        foreach (var ri in recipeIngredients)
        {
            await _database.DeleteAsync(ri);
        }

        return await _database.DeleteAsync(recipe);
    }

    public async Task<int> DeleteRecipeIngredientAsync(RecipeIngredient recipeIngredient)
    {
        return await _database.DeleteAsync(recipeIngredient);
    }
}



class Program
{
    static async Task Main(string[] args)
    {

        string dbFileName = "recipe.db";
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbFileName);

        // Initialize the database service
        var dbService = new RecipeDatabaseService(dbPath);

        try
        {
            // Add three ingredients to the database
            var ingredient1 = new Ingredient { Text = "Ingredient 1", Unit = "grams", WhereToFind = "Store 1" };
            var ingredient2 = new Ingredient { Text = "Ingredient 2", Unit = "pieces", WhereToFind = "Store 2" };
            var ingredient3 = new Ingredient { Text = "Ingredient 3", Unit = "cups", WhereToFind = "Store 3" };

            await dbService.AddOrUpdateIngredientAsync(ingredient1);
            await dbService.AddOrUpdateIngredientAsync(ingredient2);
            await dbService.AddOrUpdateIngredientAsync(ingredient3);

            // Create two recipes using two ingredients each
            var recipe1 = new Recipe { Name = "Recipe 1", Preparation = "Prepare Recipe 1", Portions = 4 };
            var recipe2 = new Recipe { Name = "Recipe 3", Preparation = "Prepare Recipe 2", Portions = 2 };

            // bug here. While associating recipeingredients the recipe1 ID is always 0. The Id needs to be readed from the table searching by name.

            // Associate ingredients with recipes
            var recipeIngredient1 = new RecipeIngredient { RecipeID = recipe1.ID, IngredientID = ingredient1.ID, Quantity = 200 };
            var recipeIngredient2 = new RecipeIngredient { RecipeID = recipe1.ID, IngredientID = ingredient2.ID, Quantity = 3 };
            var recipeIngredient3 = new RecipeIngredient { RecipeID = recipe2.ID, IngredientID = ingredient2.ID, Quantity = 2 };
            var recipeIngredient4 = new RecipeIngredient { RecipeID = recipe2.ID, IngredientID = ingredient3.ID, Quantity = 1 };

            await dbService.AddRecipeAsync(recipe1);
            await dbService.AddRecipeAsync(recipe2);

            await dbService.AddRecipeIngredientAsync(recipeIngredient1);
            await dbService.AddRecipeIngredientAsync(recipeIngredient2);
            await dbService.AddRecipeIngredientAsync(recipeIngredient3);
            await dbService.AddRecipeIngredientAsync(recipeIngredient4);

            Console.WriteLine("Recipes and Ingredients added to the database...");

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
                        Console.WriteLine($"  - {ingredient.Text}: {ri.Quantity} {ingredient.Unit}");
                    }
                }
            }

            // Delete one of the recipes and its relationships
            var recipeToDelete = recipes[0];
            await dbService.DeleteRecipeAsync(recipeToDelete);

            Console.WriteLine($"Recipe '{recipeToDelete.Name}' and its relationships deleted.");

            // List remaining recipes and ingredients
            recipes = await dbService.GetAllRecipesAsync();
            Console.WriteLine("Recipes and Ingredients after deletion:");
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
                        Console.WriteLine($"  - {ingredient.Text}: {ri.Quantity} {ingredient.Unit}");
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