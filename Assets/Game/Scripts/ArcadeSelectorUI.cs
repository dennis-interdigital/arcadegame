using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InterDigital
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ArcadeSelectorUI : BaseUI, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        [Header("Data")]
        public ArcadeGameSO arcadeDatabase; // Your single SO with arcades list

        [Header("Preview / Layout")]
        public RectTransform rootArcadeGameItemContainer;    // Parent RectTransform where previews will be instantiated
        public float previewSpacing;    // spacing between previews (X axis)

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
        List<GameObject> activeArcadeItemUIList = new List<GameObject>();

        // dragging
        bool isDragging = false;
        Vector2 pointerStartPos;
        Vector2 pointerCurrentPos;
        Vector3 previewsAnchorStartPos;

        public override void Init(GameManager inGameManager)
        {
            base.Init(inGameManager);

            buttonNext.onClick.AddListener(OnClickNext);
            buttonPrev.onClick.AddListener(OnClickPrev);
            buttonPlay.onClick.AddListener(OnClickPlay);

            int count = activeArcadeItemUIList.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject obj = activeArcadeItemUIList[i];
                if (obj != null)
                    Destroy(obj);
            }
            activeArcadeItemUIList.Clear();

            count = arcadeDatabase.arcadeGameItemList.Count;
            for (int i = 0; i < count; i++)
            {
                ArcadeGameItem item = arcadeDatabase.arcadeGameItemList[i];
                GameObject go = null;

                // If your ArcadeEntry has a previewPrefab field, reflection will pick it up (optional)
                var entryType = item.GetType();
                var prefabField = entryType.GetField("previewPrefab");
                if (prefabField != null)
                {
                    var prefabObj = prefabField.GetValue(item) as GameObject;
                    if (prefabObj != null)
                        go = Instantiate(prefabObj, rootArcadeGameItemContainer);
                }

                // fallback
                if (go == null)
                {
                    go = Instantiate(item.prefabGameIcon, rootArcadeGameItemContainer);
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
                activeArcadeItemUIList.Add(go);
            }

            previewsAnchorStartPos = rootArcadeGameItemContainer != null ? (Vector3)rootArcadeGameItemContainer.localPosition : Vector3.zero;
            currentIndex = Mathf.Clamp(currentIndex, 0, Mathf.Max(0, activeArcadeItemUIList.Count - 1));
            SnapToIndex(currentIndex, instant: true);

            RefreshUI();

            transitionCanvasGroup.alpha = 1f;
        }

        void SetIndex(int index)
        {
            if (index < 0 || arcadeDatabase == null || index >= arcadeDatabase.arcadeGameItemList.Count) return;
            int previous = currentIndex;
            currentIndex = index;
            SnapToIndex(currentIndex, instant: false);
            RefreshUI();
        }

        void SnapToIndex(int index, bool instant)
        {
            if (activeArcadeItemUIList.Count == 0 || rootArcadeGameItemContainer == null) return;

            Vector3 target = previewsAnchorStartPos + Vector3.left * (index * previewSpacing);

            if (instant)
                rootArcadeGameItemContainer.localPosition = target;
            else
                rootArcadeGameItemContainer.DOLocalMove(target, 0.35f).SetEase(Ease.OutCubic);

            // zoom selected and reset others
            for (int i = 0; i < activeArcadeItemUIList.Count; i++)
            {
                if (activeArcadeItemUIList[i] == null) continue;
                activeArcadeItemUIList[i].transform.DOKill();
                if (i == index)
                    activeArcadeItemUIList[i].transform.DOScale(selectedScale, zoomDuration).SetEase(Ease.OutBack);
                else
                    activeArcadeItemUIList[i].transform.DOScale(normalScale, zoomDuration).SetEase(Ease.OutCubic);
            }
        }

        void RefreshUI()
        {
            if (arcadeDatabase != null && arcadeDatabase.arcadeGameItemList.Count > 0)
            {
                var entry = arcadeDatabase.arcadeGameItemList[currentIndex];
                if (textName != null) textName.text = entry.name ?? "<unknown>";
            }
            else
            {
                if (textName != null) textName.text = "-";
            }

            buttonPrev.interactable = currentIndex > 0;
            buttonNext.interactable = (arcadeDatabase != null && currentIndex < arcadeDatabase.arcadeGameItemList.Count - 1);
            buttonPlay.interactable = (arcadeDatabase != null && arcadeDatabase.arcadeGameItemList.Count > 0);
        }

        public void ForceShow()
        {
            transitionCanvasGroup.alpha = 1f;
        }

        public IEnumerator HideTransition()
        {
            //transitionCanvasGroup.blocksRaycasts = true;

            //yield return transitionCanvasGroup.DOFade(1f, fadeDuration).SetEase(fadeEase).WaitForCompletion();
            //yield return new WaitForSeconds(0.08f);
            //yield return transitionCanvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase).WaitForCompletion();
            //transitionCanvasGroup.blocksRaycasts = false;

            //yield return new WaitForSeconds(fadeDuration);

            //SceneManager.LoadScene(sceneName);
            yield return transitionCanvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase).WaitForCompletion();
        }

        public void OnClickNext()
        {
            int newIndex = Mathf.Clamp(currentIndex + 1, 0, arcadeDatabase.arcadeGameItemList.Count - 1);
            if (newIndex != currentIndex) SetIndex(newIndex);
        }

        public void OnClickPrev()
        {
            int newIndex = Mathf.Clamp(currentIndex - 1, 0, arcadeDatabase.arcadeGameItemList.Count - 1);
            if (newIndex != currentIndex) SetIndex(newIndex);
        }

        public void OnClickPlay()
        {
            ArcadeGameItem item = arcadeDatabase.arcadeGameItemList[currentIndex];

            string sceneToLoad = item.sceneName;

            // Play selection punch zoom
            if (activeArcadeItemUIList.Count > currentIndex && activeArcadeItemUIList[currentIndex] != null)
            {
                var t = activeArcadeItemUIList[currentIndex].transform;
                t.DOKill();
                Sequence seq = DOTween.Sequence();
                seq.Append(t.DOScale(selectedScale * 1.07f, 0.12f));
                seq.Append(t.DOScale(selectedScale, 0.12f));
            }

            gameManager.LoadGameScene(sceneToLoad);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            pointerStartPos = eventData.position;
            pointerCurrentPos = pointerStartPos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || rootArcadeGameItemContainer == null) return;
            pointerCurrentPos = eventData.position;
            float deltaX = pointerCurrentPos.x - pointerStartPos.x;

            Vector3 desired = previewsAnchorStartPos + Vector3.left * (currentIndex * previewSpacing) + Vector3.right * deltaX;
            rootArcadeGameItemContainer.localPosition = desired;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;
            float deltaX = eventData.position.x - pointerStartPos.x;

            if (Mathf.Abs(deltaX) >= swipeThreshold)
            {
                if (deltaX < 0f) OnClickNext(); else OnClickPrev();
            }
            else
            {
                SnapToIndex(currentIndex, instant: false);
            }
        }
    }
}

