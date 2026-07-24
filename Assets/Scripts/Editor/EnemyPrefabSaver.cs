#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;


public static class EnemyPrefabSaver
{
    [MenuItem("Tools/Enemy/Save Selected Enemy Prefab")]
    private static void SaveSelectedEnemyPrefab()
    {
        GameObject sourceObject = Selection.activeGameObject;

        if (sourceObject == null)
        {
            Debug.Log("저장할 Enemy Root를 선택해야 합니다.");

            return;
        }

        Enemy sourceEnemy = sourceObject.GetComponent<Enemy>();

        if (sourceEnemy == null)
        {
            Debug.LogError($"{sourceObject.name}의 Root에 Enemy 컴포넌트가 없습니다.", sourceObject);

            return;
        }

        string defaultName = sourceObject.name.Replace("(Clone)", "").Trim();

        string savePath =
            EditorUtility.SaveFilePanelInProject(
                "Enemy Prefab 저장",
                defaultName,
                "prefab",
                "Enemy Prefab을 저장할 위치를 선택하세요."
            );

        if (string.IsNullOrEmpty(savePath))
        {
            return;
        }

        /*
         * 조립 중인 원본을 건드리지 않고
         * 저장용 복제본을 만든다.
         */
        GameObject saveObject = Object.Instantiate(sourceObject);

        saveObject.name = defaultName;

        /*
         * 활성화 과정에서 AI가 움직이지 않도록
         * 우선 Root를 비활성화한다.
         */
        saveObject.SetActive(false);

        Enemy saveEnemy = saveObject.GetComponent<Enemy>();

        saveEnemy.enabled = true;

        Rigidbody2D saveRigidbody = saveObject.GetComponent<Rigidbody2D>();

        if (saveRigidbody != null)
        {
            saveRigidbody.simulated = true;
            saveRigidbody.bodyType = RigidbodyType2D.Dynamic;

            saveRigidbody.linearVelocity = Vector2.zero;

            saveRigidbody.angularVelocity = 0f;
        }

        
        saveObject.SetActive(true);

        bool saveSuccess;

        PrefabUtility.SaveAsPrefabAsset(
            saveObject,
            savePath,
            out saveSuccess
        );

        Object.DestroyImmediate(saveObject);

        if (saveSuccess == false)
        {
            Debug.LogError($"Enemy Prefab 저장 실패: {savePath}");

            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Enemy Prefab 저장 완료: {savePath}");
    }

    [MenuItem("Tools/Enemy/Save Selected Enemy Prefab",true)]
    private static bool ValidateSaveSelectedEnemyPrefab()
    {
        return Selection.activeGameObject != null;
    }
}

#endif