using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[StoreAsText]
public enum Unit
{
    grams,
    pieces,
    cups
}

public enum SupermarketSection
{
    Other,
    Fruits_and_Vegetables,
    Brot_Bakery,
    Meat_and_Sausages,
    Dairy_Products,
    Frozen_Foods,
    Sweets,
    Beverages,
    Household_Items,
    Hygiene_Products,
    Canned_Goods,
    Pasta_and_Rice,
    Oils_Sauces_and_Spices
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
    public int RecipeID { get; set; }
    public int Portions { get; set; }
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
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public RecipeDatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await _database.CreateTableAsync<Ingredient>();
            await _database.CreateTableAsync<Recipe>();
            await _database.CreateTableAsync<RecipeIngredient>();
            await _database.CreateTableAsync<Meal>();
            await _database.CreateTableAsync<GroceryListItem>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<int> AddOrUpdateAsync<T>(T item, Func<SQLiteAsyncConnection, Task<T>> getExistingItem) where T : new()
    {
        await _semaphore.WaitAsync();
        try
        {
            var existingItem = await getExistingItem(_database);
            if (existingItem == null)
            {
                return await _database.InsertAsync(item);
            }
            else
            {
                (item as dynamic).ID = (existingItem as dynamic).ID;
                return await _database.UpdateAsync(item);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    #region Read
    public async Task<List<Ingredient>> GetAllIngredientsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.Table<Ingredient>().ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<Recipe>> GetAllRecipesAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.Table<Recipe>().ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<RecipeIngredient>> GetAllRecipeIngredientsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.Table<RecipeIngredient>().ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Ingredient> GetIngredientByID(int ingredientID)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.Table<Ingredient>().Where(i => i.ID == ingredientID).FirstOrDefaultAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion

    #region Create Update
    public async Task<int> AddOrUpdateRecipeAsync(Recipe recipe)
    {
        return await AddOrUpdateAsync(recipe, db => db.Table<Recipe>().Where(r => r.Name == recipe.Name).FirstOrDefaultAsync());
    }

    public async Task<int> AddOrUpdateIngredientAsync(Ingredient ingredient)
    {
        return await AddOrUpdateAsync(ingredient, db => db.Table<Ingredient>().Where(i => i.Name == ingredient.Name).FirstOrDefaultAsync());
    }

    public async Task<int> AddOrUpdateRecipeIngredientAsync(RecipeIngredient recipeIngredient)
    {
        return await AddOrUpdateAsync(recipeIngredient, db => db.Table<RecipeIngredient>()
            .Where(ri => ri.RecipeID == recipeIngredient.RecipeID && ri.IngredientID == recipeIngredient.IngredientID)
            .FirstOrDefaultAsync());
    }
    #endregion

    #region Delete
    public async Task<int> DeleteRecipeAsync(string recipeName)
    {
        await _semaphore.WaitAsync();
        try
        {
            var recipeToDelete = await _database.Table<Recipe>().Where(r => r.Name == recipeName).FirstOrDefaultAsync();
            if (recipeToDelete != null)
            {
                await _database.ExecuteAsync($"DELETE FROM {nameof(RecipeIngredient)} WHERE RecipeID = {recipeToDelete.ID}");
                return await _database.DeleteAsync(recipeToDelete);
            }
            return 0;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion

    #region Meals
    public async Task<int> AddMealAsync(Meal meal)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.InsertAsync(meal);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> UpdateMealAsync(Meal meal)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.UpdateAsync(meal);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> DeleteMealAsync(int mealID)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.DeleteAsync<Meal>(mealID);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<Meal>> GetMealsForDateAsync(DateTime date)
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.Table<Meal>().Where(m => m.Date == date).ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion

    #region Grocery List
    public async Task<int> AddGroceryListItemAsync(Ingredient ingredient, double quantity)
    {
        await _semaphore.WaitAsync();
        try
        {
            var groceryListItem = new GroceryListItem
            {
                IngredientID = ingredient.ID,
                Quantity = quantity,
                IsBought = false
            };

            return await _database.InsertAsync(groceryListItem);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<int> AddGroceryList(List<GroceryListItem> groceryListItems)
    {
        if (groceryListItems == null || !groceryListItems.Any())
        {
            return 0;
        }

        await _semaphore.WaitAsync();
        try
        {
            foreach (var groceryListItem in groceryListItems)
            {
                await _database.InsertAsync(groceryListItem);
            }

            return groceryListItems.Count;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task MarkGroceryItemAsBoughtAsync(int groceryListItemID)
    {
        await _semaphore.WaitAsync();
        try
        {
            var groceryListItem = await _database.Table<GroceryListItem>().Where(item => item.ID == groceryListItemID).FirstOrDefaultAsync();

            if (groceryListItem != null)
            {
                groceryListItem.IsBought = true;
                await _database.UpdateAsync(groceryListItem);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task MarkGroceryItemAsBoughtAsync(string ingredientName, double quantity)
    {
        await _semaphore.WaitAsync();
        try
        {
            var ingredient = await _database.Table<Ingredient>().Where(i => i.Name == ingredientName).FirstOrDefaultAsync();
            if (ingredient == null) return;

            var groceryItem = await _database.Table<GroceryListItem>()
                .Where(item => item.IngredientID == ingredient.ID && item.Quantity == quantity && item.IsBought)
                .FirstOrDefaultAsync();

            if (groceryItem != null)
            {
                groceryItem.IsBought = false;
                await _database.UpdateAsync(groceryItem);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 
    /// <code>
    /// Task<> example()
    /// if z 
    /// end
    /// </code>
    /// </summary>
    /// <returns></returns>
    public async Task<List<GroceryListItem>> GetAllGroceryItemsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _database.Table<GroceryListItem>().ToListAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion
}

// Usage example
public class Program
{
    public static async Task Main(string[] args)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "recipes.db");
        var dbService = new RecipeDatabaseService(dbPath);

        await dbService.InitializeAsync();

        var ingredient = new Ingredient { Name = "Tomato", Unit = Unit.pieces, WhereToFind = SupermarketSection.Fruits_and_Vegetables };
        await dbService.AddOrUpdateIngredientAsync(ingredient);

        var recipe = new Recipe { Name = "Tomato Salad", PicturePath = "tomato_salad.jpg", Preparation = "Chop tomatoes and mix with salad.", Portions = 2 };
        await dbService.AddOrUpdateRecipeAsync(recipe);

        var recipes = await dbService.GetAllRecipesAsync();
        foreach (var rec in recipes)
        {
            Console.WriteLine($"Recipe: {rec.Name}, Preparation: {rec.Preparation}");
        }
    }
}
