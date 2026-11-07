using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FootstepsScript : MonoBehaviour
{
    [SerializeField] private RectTransform[] footstepsPositions;
    public UnityEvent gameWonEvent;

    [Header("Ghost (siguiente paso)")]
    [SerializeField, Range(0f, 1f)] private float ghostAlpha = 0.45f;

    private RectTransform rt;
    private RectTransform ghostRt;
    int cont = 0;
    private float timer;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        timer = 0f;
        CreateGhost();
        ApplyStep();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    private void OnEnable()
    {
        if (ghostRt != null)
        {
            ghostRt.gameObject.SetActive(true);
            UpdateGhost();
        }
    }

    private void OnDisable()
    {
        if (ghostRt != null) ghostRt.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (ghostRt != null) Destroy(ghostRt.gameObject);
    }

    // Avanzar un paso (igual que antes)
    public void FootstepReached()
    {
        if (!gameObject.activeSelf) return;
        if (cont == footstepsPositions.Length - 1) gameWonEvent.Invoke();
        else if (timer < 0f)
        {
            cont++;
            timer = 0.1f;
            ApplyStep();
        }
    }

    // Retroceder un paso (conéctalo al botón de la tablet)
    public void FootstepBack()
    {
        if (!gameObject.activeSelf) return;
        if (cont <= 0) return;
        if (timer >= 0f) return;

        cont--;
        timer = 0.1f;
        ApplyStep();
    }

    private void ApplyStep()
    {
        PlaceAt(rt, cont);
        UpdateGhost();
    }

    private void UpdateGhost()
    {
        if (ghostRt == null) return;

        int next = cont + 1;
        if (next >= footstepsPositions.Length)
        {
            // No hay siguiente paso (ya estamos en el último)
            ghostRt.gameObject.SetActive(false);
            return;
        }

        ghostRt.gameObject.SetActive(true);
        PlaceAt(ghostRt, next);
    }

    private void PlaceAt(RectTransform target, int index)
    {
        target.position = footstepsPositions[index].position;
        if (footstepsPositions[index].name.Contains("Wide"))
            target.sizeDelta = new Vector2(footstepsPositions[index].sizeDelta.x, 100);
        else
            target.sizeDelta = new Vector2(100, 100);
    }

    private void CreateGhost()
    {
        Image source = GetComponent<Image>();
        if (source == null)
        {
            Debug.LogWarning("[FootstepsScript] No hay componente Image; no se puede crear el ghost.", this);
            return;
        }

        GameObject go = new GameObject(name + "_Ghost", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform.parent, false);
        // Justo detrás de la huella principal
        go.transform.SetSiblingIndex(transform.GetSiblingIndex());

        ghostRt = go.GetComponent<RectTransform>();
        ghostRt.anchorMin = rt.anchorMin;
        ghostRt.anchorMax = rt.anchorMax;
        ghostRt.pivot = rt.pivot;
        ghostRt.localScale = rt.localScale;
        ghostRt.rotation = rt.rotation;

        Image img = go.GetComponent<Image>();
        img.sprite = source.sprite;
        img.type = source.type;
        img.preserveAspect = source.preserveAspect;
        img.material = source.material;
        Color c = source.color;
        c.a = source.color.a * ghostAlpha;
        img.color = c;
        img.raycastTarget = false; // No debe interceptar toques
    }
}
