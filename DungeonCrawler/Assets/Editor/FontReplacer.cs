using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPFontReplacer : EditorWindow
{
    TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Fonts")]
    static void Init()
    {
        TMPFontReplacer window = (TMPFontReplacer)EditorWindow.GetWindow(typeof(TMPFontReplacer));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Replace all TMP Fonts in Scene", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts"))
        {
            ReplaceFonts();
        }
    }

    void ReplaceFonts()
    {
        if (newFont == null)
        {
            Debug.LogWarning("No TMP font selected!");
            return;
        }

        TMP_Text[] allTMPs = Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        foreach (TMP_Text tmp in allTMPs)
        {
            Undo.RecordObject(tmp, "Replace TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
        }

        Debug.Log("Replaced " + allTMPs.Length + " TMP fonts in this scene.");
    }
}
