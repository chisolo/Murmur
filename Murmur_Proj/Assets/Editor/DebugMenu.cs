using UnityEngine;
using Lemegeton;
using UnityEditor;


public class DebugMenu
{
    public const string SkipTutorialMenuPath = "Dev/SkipTutorial";
    [MenuItem(SkipTutorialMenuPath, false, 0)]
    public static void SkipTutorialMenu()
    {
        var isChecked = EditorPrefs.GetBool("d_SkipTutorial", false);
        EditorPrefs.SetBool("d_SkipTutorial", !isChecked);
    }

    [MenuItem(SkipTutorialMenuPath, true)]
    private static bool ToggleSkipTutorial()
    {
        var isChecked = EditorPrefs.GetBool("d_SkipTutorial", false);
        Menu.SetChecked(SkipTutorialMenuPath, isChecked);
        return true;
    }

    public static bool IsSkipTutorial()
    {
        var isChecked = EditorPrefs.GetBool("d_SkipTutorial", false);
        return isChecked;
    }
}