namespace MealPlanner.Services;

public class RecipesDatabaseService
{

    private readonly SQLiteAsyncConnection _database;

    public RecipesDatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Ingredient>().Wait();
        _database.CreateTableAsync<Recipe>().Wait();
        _database.CreateTableAsync<RecipeIngredient>().Wait();
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




