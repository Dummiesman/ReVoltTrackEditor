using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

class UnloadExternalBitmaps : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    public void OnPreprocessBuild(BuildReport report)
    {
        AssetDatabase.ImportAsset("Assets/UI/BUTTONSPLACEHOLDER.png");
        AssetDatabase.ImportAsset("Assets/UI/CLOCKPLACEHOLDER.png");
        AssetDatabase.ImportAsset("Assets/UI/ICONSPLACEHOLDER.png");
        AssetDatabase.ImportAsset("Assets/UI/UIPLACEHOLDER2.png");
        AssetDatabase.ImportAsset("Assets/UI/UIPLACEHOLDER1.png");
    }
}