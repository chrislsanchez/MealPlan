namespace MealPlanner.Model;

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
