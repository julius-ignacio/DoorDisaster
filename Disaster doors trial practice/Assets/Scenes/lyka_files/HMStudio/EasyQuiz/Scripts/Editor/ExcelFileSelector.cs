using UnityEditor;
using UnityEngine;
using System.IO;

namespace HMStudio.EasyQuiz
{
    public class ExcelFileSelector : EditorWindow
    {
        private string filePath = "";

        [MenuItem("Tools/HMStudio/EasyQuiz/ImportFile",priority =  0)]
        public static void ShowWindow()
        {
            GetWindow<ExcelFileSelector>("Select Excel File");
        }

        [MenuItem("Tools/HMStudio/EasyQuiz/Clear",priority = 1)]
        public static void ClearFilePath()
        {
            PlayerPrefs.SetString("SelectedExcelPath", "");
        }
        
        private void OnEnable()
        {
            filePath = PlayerPrefs.GetString("SelectedExcelPath", "");
        }

        private void OnGUI()
        {
            GUILayout.Label("Select an Excel File", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Selected File:", filePath, EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space();

            if (GUILayout.Button("Browse", GUILayout.Height(30)))
            {
                string selectedPath =
                    EditorUtility.OpenFilePanel("Select Excel File", Application.streamingAssetsPath, "xls,xlsx");
                if (!string.IsNullOrEmpty(selectedPath) && File.Exists(selectedPath))
                {
                    filePath = selectedPath;
                    PlayerPrefs.SetString("SelectedExcelPath", filePath);
                }
            }

            EditorGUILayout.Space();

            if (!string.IsNullOrEmpty(filePath))
            {
                EditorGUILayout.HelpBox("File selected successfully!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("No file selected.", MessageType.Warning);
            }
        }
    }
}