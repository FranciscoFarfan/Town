using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DialogUIManager_TMP : MonoBehaviour
{
    public static DialogUIManager_TMP Instance { get; private set; }

    [Header("Opcional: asigna un sprite por defecto para portrait")]
    public Sprite defaultPortrait;

    GameObject canvasGO;
    TextMeshProUGUI titleText;
    TextMeshProUGUI bodyText;
    Image portraitImage;
    Transform buttonsParent;
    GameObject buttonPrefab;

    // Estado actual
    private List<(string label, Action action)> currentOptions = new List<(string, Action)>();
    private List<Button> currentButtons = new List<Button>();
    private int selectedIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateUI();
        Hide();
    }

    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }
    }

    void CreateUI()
    {
        EnsureEventSystem();

        // Canvas
        canvasGO = new GameObject("DialogCanvas_TMP");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasGO);

        // Panel background
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.6f);
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0.2f, 0.2f);
        panelRT.anchorMax = new Vector2(0.8f, 0.5f);
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Portrait (left)
        GameObject portraitGO = new GameObject("Portrait");
        portraitGO.transform.SetParent(panel.transform, false);
        portraitImage = portraitGO.AddComponent<Image>();
        portraitImage.color = Color.white;
        var pRT = portraitGO.GetComponent<RectTransform>();
        pRT.anchorMin = new Vector2(0.02f, 0.12f);
        pRT.anchorMax = new Vector2(0.22f, 0.88f);
        pRT.offsetMin = Vector2.zero;
        pRT.offsetMax = Vector2.zero;

        // Title (TMP)
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.fontSize = 20;
        titleText.enableWordWrapping = true;
        titleText.alignment = TextAlignmentOptions.Left;
        var tRT = titleGO.GetComponent<RectTransform>();
        tRT.anchorMin = new Vector2(0.25f, 0.62f);
        tRT.anchorMax = new Vector2(0.97f, 0.88f);
        tRT.offsetMin = Vector2.zero;
        tRT.offsetMax = Vector2.zero;

        // Body text (TMP)
        GameObject bodyGO = new GameObject("Body");
        bodyGO.transform.SetParent(panel.transform, false);
        bodyText = bodyGO.AddComponent<TextMeshProUGUI>();
        bodyText.fontSize = 16;
        bodyText.enableWordWrapping = true;
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        var bRT = bodyGO.GetComponent<RectTransform>();
        bRT.anchorMin = new Vector2(0.25f, 0.12f);
        bRT.anchorMax = new Vector2(0.97f, 0.60f);
        bRT.offsetMin = Vector2.zero;
        bRT.offsetMax = Vector2.zero;

        // Buttons parent
        GameObject btns = new GameObject("Buttons");
        btns.transform.SetParent(panel.transform, false);
        buttonsParent = btns.transform;
        var btnsRT = btns.AddComponent<RectTransform>();
        btnsRT.anchorMin = new Vector2(0.25f, 0.02f);
        btnsRT.anchorMax = new Vector2(0.97f, 0.12f);
        btnsRT.offsetMin = Vector2.zero;
        btnsRT.offsetMax = Vector2.zero;

        var hLayout = btns.AddComponent<HorizontalLayoutGroup>();
        hLayout.childForceExpandWidth = true;
        hLayout.childForceExpandHeight = true;
        hLayout.spacing = 8;
        btns.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // Button prefab (simple with TMP text)
        buttonPrefab = new GameObject("ButtonPrefab_TMP");
        var prefabRT = buttonPrefab.AddComponent<RectTransform>();
        prefabRT.sizeDelta = new Vector2(160, 36);

        // Ensure CanvasRenderer so UI draws and receives clicks correctly
        buttonPrefab.AddComponent<CanvasRenderer>();

        var bi = buttonPrefab.AddComponent<Image>();
        bi.color = new Color(1f, 1f, 1f, 0.95f);

        var b = buttonPrefab.AddComponent<Button>();
        // Default navigation = automatic (works with keyboard and EventSystem)

        GameObject txt = new GameObject("Text_TMP");
        txt.transform.SetParent(buttonPrefab.transform, false);
        // Text needs CanvasRenderer too
        txt.AddComponent<CanvasRenderer>();
        var tmp = txt.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        var txtRT = txt.GetComponent<RectTransform>();
        txtRT.anchorMin = Vector2.zero;
        txtRT.anchorMax = Vector2.one;
        txtRT.offsetMin = Vector2.zero;
        txtRT.offsetMax = Vector2.zero;

        // Start inactive (we instantiate copies)
        buttonPrefab.SetActive(false);
    }

    void Update()
    {
        // Only process keyboard when dialog visible and there are buttons
        if (canvasGO == null || !canvasGO.activeSelf) return;
        if (currentButtons.Count == 0) return;

        int count = currentButtons.Count;

        // Left / Right navigation
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selectedIndex = (selectedIndex + 1) % count;
            SelectButton(selectedIndex);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            selectedIndex = (selectedIndex - 1 + count) % count;
            SelectButton(selectedIndex);
        }

        // Confirm
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSelected();
            return;
        }

        // Shortcuts: Y/N for first/second option
        if (Input.GetKeyDown(KeyCode.Y) && currentOptions.Count > 0)
        {
            currentOptions[0].action?.Invoke();
            Hide();
            return;
        }
        if (Input.GetKeyDown(KeyCode.N) && currentOptions.Count > 1)
        {
            currentOptions[1].action?.Invoke();
            Hide();
            return;
        }

        // Close with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hide();
            return;
        }
    }

    void SelectButton(int idx)
    {
        if (currentButtons == null || currentButtons.Count == 0) return;
        idx = Mathf.Clamp(idx, 0, currentButtons.Count - 1);
        selectedIndex = idx;

        // Visual feedback: change color of selected vs others
        for (int i = 0; i < currentButtons.Count; i++)
        {
            var img = currentButtons[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = (i == idx) ? new Color(0.8f, 0.8f, 0.95f, 1f) : new Color(1f, 1f, 1f, 0.95f);
            }
        }

        // Tell EventSystem which is selected (so navigation works)
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(currentButtons[idx].gameObject);
        }
    }

    void ActivateSelected()
    {
        if (currentButtons == null || currentButtons.Count == 0) return;
        int idx = Mathf.Clamp(selectedIndex, 0, currentButtons.Count - 1);
        // Invoke the stored action
        if (idx >= 0 && idx < currentOptions.Count)
        {
            try { currentOptions[idx].action?.Invoke(); } catch (Exception ex) { Debug.LogException(ex); }
        }
        Hide();
    }

    public void Hide()
    {
        if (canvasGO != null) canvasGO.SetActive(false);
        // Clear selection state
        currentOptions.Clear();
        foreach (var b in currentButtons) if (b != null) Destroy(b.gameObject);
        currentButtons.Clear();
        selectedIndex = 0;
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Muestra un diálogo; options es un array de (label, Action).
    /// </summary>
    public void ShowDialog(string title, string body, Sprite portrait = null, params (string label, Action action)[] options)
    {
        if (canvasGO == null)
        {
            Debug.LogWarning("DialogUIManager_TMP: canvas no creado.");
            return;
        }

        canvasGO.SetActive(true);

        // Set title/body
        if (titleText != null) titleText.text = title ?? "";
        if (bodyText != null) bodyText.text = body ?? "";

        // Portrait
        if (portraitImage != null)
        {
            portraitImage.sprite = portrait != null ? portrait : defaultPortrait;
            portraitImage.enabled = portraitImage.sprite != null;
        }

        // Clear old buttons from UI and memory
        if (buttonsParent == null)
        {
            Debug.LogWarning("DialogUIManager_TMP: buttonsParent null");
        }
        else
        {
            var toDelete = new List<GameObject>();
            foreach (Transform t in buttonsParent)
            {
                if (t != null) toDelete.Add(t.gameObject);
            }
            foreach (var go in toDelete)
            {
                if (go != null) Destroy(go);
            }
        }
        currentButtons.Clear();
        currentOptions.Clear();

        // Create new buttons
        if (options != null)
        {
            foreach (var opt in options)
            {
                // capture local copy to avoid closure issues
                var labelLocal = opt.label;
                var actionLocal = opt.action;

                var go = Instantiate(buttonPrefab, buttonsParent);
                go.SetActive(true);

                // ensure UI renderer is present (safety)
                if (go.GetComponent<CanvasRenderer>() == null)
                    go.AddComponent<CanvasRenderer>();
                var txt = go.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null) txt.text = labelLocal;

                var btn = go.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        try { actionLocal?.Invoke(); } catch (Exception ex) { Debug.LogException(ex); }
                        Hide();
                    });
                    // optional: highlight on selection via navigation
                    btn.transition = Selectable.Transition.ColorTint;
                }

                currentButtons.Add(btn);
                currentOptions.Add((labelLocal, actionLocal));
            }
        }

        // default selection to first button
        if (currentButtons.Count > 0)
        {
            selectedIndex = 0;
            SelectButton(selectedIndex);
        }
        else
        {
            // no buttons: close with any key or ESC - but leave visible until Hide is called
        }
    }
}
