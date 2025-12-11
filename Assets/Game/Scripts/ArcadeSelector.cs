// ArcadeSelector.cs
// Place this on a GameObject in your UI scene. This script persists across scene loads (singleton).
// Requires DOTween (for tweens) and Unity UI (Image/Text/Button).
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ArcadeSelector : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [Header("Data")]
    public ArcadeDatabase arcadeDatabase; // Your single SO with arcades list

    [Header("Preview / Layout")]
    public RectTransform previewParent;    // Parent RectTransform where previews will be instantiated
    public GameObject fallbackPreviewPrefab; // A simple prefab with an Image (optional)
    public float previewSpacing = 600f;    // spacing between previews (X axis)

    [Header("UI")]
    public Button buttonNext;
    public Button buttonPrev;
    public Button buttonPlay;
    public TextMeshProUGUI textName;

    [Header("Zoom / Swipe")]
    public Vector3 selectedScale = Vector3.one * 1.12f;
    public Vector3 normalScale = Vector3.one;
    public float zoomDuration = 0.25f;
    public float swipeThreshold = 120f;

    [Header("Scene Transition")]
    [Tooltip("CanvasGroup used to fade to the arcade scene. If null, this GameObject's CanvasGroup will be used.")]
    public CanvasGroup transitionCanvasGroup;
    public float fadeDuration = 0.45f;
    public Ease fadeEase = Ease.Linear;

    // runtime
    int currentIndex = 0;
    List<GameObject> instantiatedPreviews = new List<GameObject>();

    // dragging
    bool isDragging = false;
    Vector2 pointerStartPos;
    Vector2 pointerCurrentPos;
    Vector3 previewsAnchorStartPos;

    // singleton guard
    private static ArcadeSelector instance;

    public void Init()
    {
        // Singleton: prevent duplicates when returning to selector scene
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);

        // default transition canvas group
        if (transitionCanvasGroup == null)
        {
            transitionCanvasGroup = GetComponent<CanvasGroup>();
            if (transitionCanvasGroup == null)
            {
                transitionCanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        // wire buttons
        if (buttonNext != null) buttonNext.onClick.AddListener(OnNext);
        if (buttonPrev != null) buttonPrev.onClick.AddListener(OnPrev);
        if (buttonPlay != null) buttonPlay.onClick.AddListener(OnPlay);

        // build previews
        BuildPreviews();
        UpdateUIImmediate();

        // start invisible transition (clear)
        //transitionCanvasGroup.alpha = 0f;
        //transitionCanvasGroup.blocksRaycasts = false;
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;

        if (buttonNext != null) buttonNext.onClick.RemoveListener(OnNext);
        if (buttonPrev != null) buttonPrev.onClick.RemoveListener(OnPrev);
        if (buttonPlay != null) buttonPlay.onClick.RemoveListener(OnPlay);
    }

    void BuildPreviews()
    {
        ClearPreviews();

        if (arcadeDatabase == null || arcadeDatabase.arcades == null) return;

        for (int i = 0; i < arcadeDatabase.arcades.Count; i++)
        {
            var entry = arcadeDatabase.arcades[i];
            GameObject go = null;

            // If your ArcadeEntry has a previewPrefab field, reflection will pick it up (optional)
            var entryType = entry.GetType();
            var prefabField = entryType.GetField("previewPrefab");
            if (prefabField != null)
            {
                var prefabObj = prefabField.GetValue(entry) as GameObject;
                if (prefabObj != null)
                    go = Instantiate(prefabObj, previewParent);
            }

            // fallback
            if (go == null)
            {
                if (fallbackPreviewPrefab != null)
                {
                    go = Instantiate(fallbackPreviewPrefab, previewParent);
                }
                else
                {
                    // create a simple image object so something appears
                    GameObject simple = new GameObject($"ArcadePreview_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    simple.transform.SetParent(previewParent, false);
                    go = simple;
                }

                var image = go.GetComponentInChildren<Image>();
                if (image != null && entry.icon != null)
                {
                    image.sprite = entry.icon;
                    image.preserveAspect = true;
                }
            }

            // position horizontally
            RectTransform rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(i * previewSpacing, 0f);
            }
            else
            {
                go.transform.localPosition = new Vector3(i * previewSpacing, 0f, 0f);
            }

            go.transform.localScale = normalScale;
            instantiatedPreviews.Add(go);
        }

        previewsAnchorStartPos = previewParent != null ? (Vector3)previewParent.localPosition : Vector3.zero;
        currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, instantiatedPreviews.Count - 1));
        SnapToIndex(currentIndex, instant: true);
    }

    void ClearPreviews()
    {
        for (int i = 0; i < instantiatedPreviews.Count; i++)
        {
            if (instantiatedPreviews[i] != null)
                Destroy(instantiatedPreviews[i]);
        }
        instantiatedPreviews.Clear();
    }

    #region Navigation

    public void OnNext()
    {
        if (arcadeDatabase == null || arcadeDatabase.arcades.Count == 0) return;
        int newIndex = Mathf.Clamp(currentIndex + 1, 0, arcadeDatabase.arcades.Count - 1);
        if (newIndex != currentIndex) SetIndex(newIndex);
    }

    public void OnPrev()
    {
        if (arcadeDatabase == null || arcadeDatabase.arcades.Count == 0) return;
        int newIndex = Mathf.Clamp(currentIndex - 1, 0, arcadeDatabase.arcades.Count - 1);
        if (newIndex != currentIndex) SetIndex(newIndex);
    }

    void SetIndex(int index)
    {
        if (index < 0 || arcadeDatabase == null || index >= arcadeDatabase.arcades.Count) return;
        int previous = currentIndex;
        currentIndex = index;
        SnapToIndex(currentIndex, instant: false);
        UpdateUI(previousIndex: previous);
    }

    void SnapToIndex(int index, bool instant)
    {
        if (instantiatedPreviews.Count == 0 || previewParent == null) return;

        Vector3 target = previewsAnchorStartPos + Vector3.left * (index * previewSpacing);

        if (instant)
            previewParent.localPosition = target;
        else
            previewParent.DOLocalMove(target, 0.35f).SetEase(Ease.OutCubic);

        // zoom selected and reset others
        for (int i = 0; i < instantiatedPreviews.Count; i++)
        {
            if (instantiatedPreviews[i] == null) continue;
            instantiatedPreviews[i].transform.DOKill();
            if (i == index)
                instantiatedPreviews[i].transform.DOScale(selectedScale, zoomDuration).SetEase(Ease.OutBack);
            else
                instantiatedPreviews[i].transform.DOScale(normalScale, zoomDuration).SetEase(Ease.OutCubic);
        }
    }

    void UpdateUI(int previousIndex = -1)
    {
        if (arcadeDatabase != null && arcadeDatabase.arcades.Count > 0)
        {
            var entry = arcadeDatabase.arcades[currentIndex];
            if (textName != null) textName.text = entry.arcadeName ?? "<unknown>";
        }
        else
        {
            if (textName != null) textName.text = "-";
        }

        if (buttonPrev != null) buttonPrev.interactable = currentIndex > 0;
        if (buttonNext != null) buttonNext.interactable = (arcadeDatabase != null && currentIndex < arcadeDatabase.arcades.Count - 1);
        if (buttonPlay != null) buttonPlay.interactable = (arcadeDatabase != null && arcadeDatabase.arcades.Count > 0);
    }

    void UpdateUIImmediate()
    {
        UpdateUI(previousIndex: -1);
    }

    #endregion

    #region Play / Load with Transition

    public void OnPlay()
    {
        if (arcadeDatabase == null || arcadeDatabase.arcades.Count == 0) return;
        var entry = arcadeDatabase.arcades[currentIndex];
        if (entry == null)
        {
            Debug.LogWarning("ArcadeSelector: selected entry is null.");
            return;
        }

        // get sceneName from entry (ArcadeEntry has sceneName)
        string sceneToLoad = entry.sceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning($"ArcadeSelector: sceneName for '{entry.arcadeName}' is empty.");
            return;
        }

        // Play selection punch zoom
        if (instantiatedPreviews.Count > currentIndex && instantiatedPreviews[currentIndex] != null)
        {
            var t = instantiatedPreviews[currentIndex].transform;
            t.DOKill();
            Sequence seq = DOTween.Sequence();
            seq.Append(t.DOScale(selectedScale * 1.07f, 0.12f));
            seq.Append(t.DOScale(selectedScale, 0.12f));
        }

        // Start transition coroutine (fade out) then load scene
        StartCoroutine(DoTransitionAndLoad(sceneToLoad));
    }

    System.Collections.IEnumerator DoTransitionAndLoad(string sceneName)
    {
        // make sure transition blocks input
        transitionCanvasGroup.blocksRaycasts = true;

        // fade to 1
        yield return transitionCanvasGroup.DOFade(1f, fadeDuration).SetEase(fadeEase).WaitForCompletion();

        // optionally wait a short moment so fade completes visually
        yield return new WaitForSeconds(0.08f);


        // after scene load, keep selector alive (already DontDestroyOnLoad)
        // you might want to hide or reposition the selector in the new scene:
        // keep transition visible for a moment, then fade back in
        yield return transitionCanvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase).WaitForCompletion();
        transitionCanvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(fadeDuration);

        // load scene (synchronous load; change to async if you prefer)
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Swipe / Drag (UI events)

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        pointerStartPos = eventData.position;
        pointerCurrentPos = pointerStartPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || previewParent == null) return;
        pointerCurrentPos = eventData.position;
        float deltaX = pointerCurrentPos.x - pointerStartPos.x;

        Vector3 desired = previewsAnchorStartPos + Vector3.left * (currentIndex * previewSpacing) + Vector3.right * deltaX;
        previewParent.localPosition = desired;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        float deltaX = eventData.position.x - pointerStartPos.x;

        if (Mathf.Abs(deltaX) >= swipeThreshold)
        {
            if (deltaX < 0f) OnNext(); else OnPrev();
        }
        else
        {
            SnapToIndex(currentIndex, instant: false);
        }
    }

    #endregion
}
