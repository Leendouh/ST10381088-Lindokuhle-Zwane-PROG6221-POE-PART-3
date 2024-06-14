using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ST10381088_Lindokuhle_Zwane_PROG6221_POE_PART_3
{
    public partial class MainWindow : Window
    {
        // List to store recipe details
        public List<RecipeDetails> IngredientsList = new List<RecipeDetails>();

        // Dictionary to store calories and food groups
        public Dictionary<string, double> CaloriesAndFoodGroup = new Dictionary<string, double>();

        // Constructor for the main window
        public MainWindow()
        {
            InitializeComponent();
        }

        // Method triggered when a recipe is selected in the ListBox
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            display.Content = ""; // Clear previous content

            if (list.SelectedItem != null)
            {
                var selectedRecipeName = list.SelectedItem.ToString(); // Get selected recipe name
                var selectedRecipe = IngredientsList.FirstOrDefault(r => r.RecipeName == selectedRecipeName); // Find recipe details

                if (selectedRecipe != null)
                {
                    // Sort ingredients alphabetically by name
                    var sortedIngredients = IngredientsList
                        .Where(r => r.RecipeName == selectedRecipeName)
                        .OrderBy(r => r.ItemName)
                        .ToList();

                    // Create a formatted list of ingredients
                    string ingredientsList = string.Join("\n", sortedIngredients.Select(r =>
                        $"Ingredient: {r.ItemName}\n" +
                        $"Measurement: {r.ItemMeasurement}\n" +
                        $"Quantity: {r.ItemQuantity} {r.ItemMeasurement}\n" +
                        $"Calories: {r.ItemCalories}\n" +
                        $"Food Group: {r.ItemFoodGroup}\n"
                    ));

                    // Display recipe details in the TextBlock
                    display.Content = $"Recipe Name: {selectedRecipe.RecipeName}\n\n" +
                                      $"{ingredientsList}\n" +
                                      $"Steps to prepare recipe:\n{selectedRecipe.RecipeDescription}\n";
                }
            }
            else
            {
                display.Content = "Select A Recipe From Your List"; // Reset display if no recipe is selected
            }
        }

        // Delegate for showing calorie count message
        public delegate void CalorieCountDelegate(string noticeMsg);

        // Method to show a MessageBox with calorie count
        public void ShowCalorieCount(string message)
        {
            MessageBox.Show(message, "Calorie Count", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Method to add calories for each ingredient
        public void AddCalories(CalorieCountDelegate calorieCount)
        {
            double totalCalories = IngredientsList.Sum(ingredient => ingredient.ItemCalories);

            // Show total calories message
            if (totalCalories >= 300)
                calorieCount($"Recipe has exceeded 300 calories.\nCalorie Total = {totalCalories}");
            else
                calorieCount($"Calorie Total For Your Recipe: {totalCalories}");
        }

        // Button click handler for adding food group and calories
        private void FoodGroup_Click(object sender, RoutedEventArgs e)
        {
            AddCalories(ShowCalorieCount);
        }

        // Method to clear all text boxes
        private void ClearTextBoxes()
        {
            ingredient.Clear();
            measure.Clear();
            qty.Clear();
            rname.Clear();
            textb.Document.Blocks.Clear();
            calories.Clear();
            foodGroupComboBox.SelectedIndex = -1;
        }

        // Method to clear everything and reset the application state
        private void ClearEverything()
        {
            ClearTextBoxes(); // Clear all text boxes
            IngredientsList.Clear(); // Clear the ingredients list
            list.ItemsSource = null;
            list.Items.Clear(); // Clear items in the ListBox
            display.Content = "Select A Recipe From Your List"; // Update display message
        }

        // Method to scale recipe ingredients by a factor
        private void ScaleRecipe(double scaleBy)
        {
            foreach (var ingredient in IngredientsList)
                ingredient.ScaleRecipe(scaleBy); // Scale each ingredient
        }

        // Method to reset all recipe scaling to original quantities
        private void ResetRecipeScaling()
        {
            foreach (var ingredient in IngredientsList)
                ingredient.ResetRecipe(); // Reset each ingredient
        }

        // Radio button event handlers for scaling recipe
        private void Radio1_Checked(object sender, RoutedEventArgs e)
        {
            if (radio1.IsChecked == true)
            {
                ScaleRecipe(0.5); // Scale by 0.5
                radio1.IsChecked = false; // Uncheck the radio button
            }
        }

        private void Radio2_Checked(object sender, RoutedEventArgs e)
        {
            if (radio2.IsChecked == true)
            {
                ScaleRecipe(2); // Scale by 2
                radio2.IsChecked = false; // Uncheck the radio button
            }
        }

        private void Radio3_Checked(object sender, RoutedEventArgs e)
        {
            if (radio3.IsChecked == true)
            {
                ScaleRecipe(3); // Scale by 3
                radio3.IsChecked = false; // Uncheck the radio button
            }
        }

        // Button click handler to undo recipe scaling
        private void UndoScaling_Click(object sender, RoutedEventArgs e)
        {
            ResetRecipeScaling(); // Reset scaling to original quantities
        }

        // Button click handler to clear all data
        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Clear Data?", "Reset App", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                ClearEverything(); // Clear all data and reset
            }
        }

        // Button click handler to exit the application
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Clear Data and Exit?", "Exit App", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                ClearEverything(); // Clear all data and reset
                Close(); // Close the application
            }
        }

        // Button click handler to add a new recipe
        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
                // Validate if any of the required fields are empty
                if (string.IsNullOrWhiteSpace(ingredient.Text) ||
                    string.IsNullOrWhiteSpace(measure.Text) ||
                    string.IsNullOrWhiteSpace(qty.Text) ||
                    string.IsNullOrWhiteSpace(calories.Text) ||
                    string.IsNullOrWhiteSpace(foodGroupComboBox.Text) ||
                    string.IsNullOrWhiteSpace(rname.Text) ||
                    new TextRange(textb.Document.ContentStart, textb.Document.ContentEnd).Text.Trim().Length == 0)
                {
                    // Show error message for incomplete recipe information
                    MessageBox.Show("RECIPE INFORMATION IS INCOMPLETE OR INVALID, TRY AGAIN", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Gather values from the text boxes
                string name = ingredient.Text;
                string measurement = measure.Text;
                if (!double.TryParse(qty.Text, out double quantity))
                {
                    // Show error message for invalid quantity
                    MessageBox.Show("Invalid quantity. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!double.TryParse(calories.Text, out double calorie))
                {
                    // Show error message for invalid calorie input
                    MessageBox.Show("Invalid calories input. Please enter a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string recipeName = rname.Text;
                string recipeDescription = new TextRange(textb.Document.ContentStart, textb.Document.ContentEnd).Text.Trim();
                string foodGroup = foodGroupComboBox.Text;

                // Create a new RecipeDetails object
                RecipeDetails recipeDetails = new RecipeDetails(name, recipeName, quantity, measurement, calorie, foodGroup, recipeDescription);

                // Add the recipe details to the list
                IngredientsList.Add(recipeDetails);

                // Clear the text boxes
                ClearTextBoxes();

                // Check if the user wants to add another ingredient to the current recipe
                MessageBoxResult result = MessageBox.Show("Enter more ingredients?", "Ingredients", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // If yes, keep the recipe name and description for next ingredient
                    rname.Text = recipeName;
                    textb.Document.Blocks.Clear();
                    textb.Document.Blocks.Add(new Paragraph(new Run(recipeDescription)));
                }
                else
                {
                    // If no, add the recipe name to the list box and clear the text boxes
                    list.Items.Add(recipeName);
                }

                // Check if total calories exceed 300
                double totalCalories = IngredientsList.Sum(ingredient => ingredient.ItemCalories);
                if (totalCalories >= 300)
                {
                    MessageBox.Show($"Recipe has exceeded 300 calories.\nCalorie Total = {totalCalories}", "Calorie Count Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            
        }

        // Text changed event handler for filtering recipes by ingredient and food group
        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the filter text from the TextBox
            string filterText = filterIngredient.Text.Trim();

            // Filter recipes by ingredient name and food group
            var filteredRecipes = IngredientsList
                .Where(recipe =>
                    recipe.ItemName.Contains(filterText, StringComparison.OrdinalIgnoreCase) || // Filter by ingredient name
                    recipe.ItemFoodGroup.Contains(filterText, StringComparison.OrdinalIgnoreCase)) // Filter by food group
                .Select(recipe => recipe.RecipeName)
                .Distinct()
                .ToList();

            // Clear current items in the ListBox
            list.Items.Clear();

            // Update the ListBox with filtered recipes
            foreach (var recipe in filteredRecipes)
            {
                list.Items.Add(recipe);
            }
        }



        // Button click handler for applying filter
        private void FilterRecipes_Click(object sender, RoutedEventArgs e)
        {
            Filter_TextChanged(sender, null); // Trigger text changed event to apply filter
        }

        // Class to hold recipe details
        public class RecipeDetails
        {
            public string ItemName { get; }
            public string RecipeName { get; }
            public double ItemQuantity { get; private set; }
            public string ItemMeasurement { get; }
            public double ItemCalories { get; set; }
            public string ItemFoodGroup { get; }
            public string RecipeDescription { get; }

            private readonly double _originalQuantity;

            // Constructor to initialize recipe details
            public RecipeDetails(string itemName, string recipeName, double itemQuantity, string itemMeasurement, double itemCalories, string itemFoodGroup, string recipeDescription)
            {
                ItemName = itemName;
                RecipeName = recipeName;
                ItemQuantity = itemQuantity;
                ItemMeasurement = itemMeasurement;
                ItemCalories = itemCalories;
                ItemFoodGroup = itemFoodGroup;
                RecipeDescription = recipeDescription;
                _originalQuantity = itemQuantity;
            }

            // Method to scale recipe quantities
            public void ScaleRecipe(double scaleBy)
            {
                ItemQuantity = _originalQuantity * scaleBy; // Adjust quantity based on scale factor
            }

            // Method to reset recipe quantities to original values
            public void ResetRecipe()
            {
                ItemQuantity = _originalQuantity; // Reset quantity to original value
            }
        }

        // Button click handler to display recipe details
        private void RecipeDisplay_Click(object sender, RoutedEventArgs e)
        {
            if (list.SelectedItem != null)
            {
                var selectedRecipeName = list.SelectedItem.ToString(); // Get selected recipe name
                var selectedRecipeDetails = IngredientsList.Where(r => r.RecipeName == selectedRecipeName).ToList(); // Find matching recipe details

                if (selectedRecipeDetails.Count > 0)
                {
                    var recipeDetailsText = $"Recipe Name: {selectedRecipeName}\n\n";
                    double totalCalories = 0;

                    foreach (var detail in selectedRecipeDetails)
                    {
                        // Add ingredient details to the recipe text
                        recipeDetailsText += $"Ingredient: {detail.ItemName}\n" +
                                             $"Quantity: {detail.ItemQuantity} {detail.ItemMeasurement}\n" +
                                             $"Calories: {detail.ItemCalories}\n" +
                                             $"Food Group: {detail.ItemFoodGroup}\n\n";

                        // Accumulate total calories for the recipe
                        totalCalories += detail.ItemCalories;
                    }

                    // Add recipe steps and total calories to the recipe text
                    recipeDetailsText += $"Steps to prepare recipe:\n{selectedRecipeDetails.First().RecipeDescription}\n\n";
                    recipeDetailsText += $"Total Calories: {totalCalories}";

                    // Display recipe details in a MessageBox
                    MessageBox.Show(recipeDetailsText, "Recipe Details", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe from the list.", "No Recipe Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

