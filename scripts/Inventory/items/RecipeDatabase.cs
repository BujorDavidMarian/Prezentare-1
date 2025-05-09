using UnityEngine;
using System.Collections.Generic;

public class RecipeDatabase : MonoBehaviour
{
    public static RecipeDatabase Instance;
    public List <ItemRecipe> recipes;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public int? GetCombinationResult(int id1, int id2)
    {
        foreach (var recipe in recipes)
        {
            if ((recipe.inputID1 == id1 && recipe.inputID2 == id2) ||
                (recipe.inputID1 == id2 && recipe.inputID2 == id1))
            {
                return recipe.resultID;
            }
        }
        return null;
    }
}