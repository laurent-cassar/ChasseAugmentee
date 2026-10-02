#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class CreateLoginScene
{
    [MenuItem("Tools/Create Login Scene (Auth/Login)")]
    public static void Create()
    {
        // Ensure folder exists
        Directory.CreateDirectory("Assets/Scenes/Auth");

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // EventSystem
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        // Title Text
        var title = CreateText("TitleText", "CHASSE AUGMENTÉE", canvasGO.transform, 72, TextAlignmentOptions.Center);
        SetRect(title.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150), new Vector2(900, 150));

        // Subtitle Text
        var subtitle = CreateText("SubtitleText", "LOGIN", canvasGO.transform, 48, TextAlignmentOptions.Center);
        SetRect(subtitle.gameObject, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -260), new Vector2(600, 100));

        // Login Input (username/email)
        var loginInput = CreateTMPInput("LoginInput", "USERNAME / EMAIL", canvasGO.transform, false);
        SetRect(loginInput.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(800, 100));

        // Password Input
        var passwordInput = CreateTMPInput("PasswordInput", "PASSWORD", canvasGO.transform, true);
        SetRect(passwordInput.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(800, 100));

        // Login Button
        var loginButton = CreateButton("LoginButton", "LOGIN", canvasGO.transform);
        SetRect(loginButton.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -140), new Vector2(400, 100));

        // GoToRegister Button (No account? CREATE ACCOUNT)
        var gotoReg = CreateButton("GoToRegisterButton", "No account? CREATE ACCOUNT", canvasGO.transform);
        SetRect(gotoReg.gameObject, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 80), new Vector2(600, 80));

        // Error Panel (hidden)
        var errorPanel = new GameObject("ErrorPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        errorPanel.transform.SetParent(canvasGO.transform, false);
        var panelImg = errorPanel.GetComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.6f);
        SetRect(errorPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900, 300));
        errorPanel.name = "ErrorPanel";
        errorPanel.SetActive(false);
        var errorText = CreateText("ErrorText", "", errorPanel.transform, 36, TextAlignmentOptions.Center);
        SetRect(errorText.gameObject, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(860, 260));

        // Save scene
        var scenePath = "Assets/Scenes/Auth/Login.unity";
        Directory.CreateDirectory(Path.GetDirectoryName(scenePath));
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Create Login Scene", "Scene created at Assets/Scenes/Auth/Login.unity", "OK");
    }

    static TextMeshProUGUI CreateText(string name, string text, Transform parent, float size, TextAlignmentOptions alignment)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        return tmp;
    }

    static TMP_InputField CreateTMPInput(string name, string placeholderText, Transform parent, bool isPassword)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var image = go.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.06f);

        var input = go.AddComponent<TMP_InputField>();

        // Text component
        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 36;
        text.color = Color.white;
        text.margin = new Vector4(10,10,10,10);
        input.textComponent = text;

        // Placeholder
        var phGO = new GameObject("Placeholder", typeof(RectTransform));
        phGO.transform.SetParent(go.transform, false);
        var ph = phGO.AddComponent<TextMeshProUGUI>();
        ph.text = placeholderText;
        ph.fontSize = 36;
        ph.color = new Color(1f,1f,1f,0.5f);
        input.placeholder = ph;

        input.contentType = isPassword ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;

        return input;
    }

    static Button CreateButton(string name, string label, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.9f, 1f);

        var btn = go.AddComponent<Button>();

        var txt = CreateText("Text", label, go.transform, 36, TextAlignmentOptions.Center);
        txt.color = Color.white;
        return btn;
    }

    static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
    }

    static void SetRect(Component comp, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 size)
    {
        SetRect(((Component)comp).gameObject, anchorMin, anchorMax, pivot, anchoredPos, size);
    }
}
#endif
