using UnityEngine;
using UnityEngine.Events;

public class FootstepsScript : MonoBehaviour
{
    [SerializeField] private RectTransform[] footstepsPositions;
    public UnityEvent gameWonEvent;
    private RectTransform rt;
    int cont = 0;
    private float timer;

    // Referencias para la huella tenue del siguiente paso
    private GameObject huellaGuia;
    private UnityEngine.UI.Image imagenGuia;
    private RectTransform rtGuia;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.position = footstepsPositions[0].position;
        timer = 0f;

        // Crear la huella guía tenue al inicio y ubicarla en el siguiente paso
        CrearHuellaGuia();
        ActualizarHuellaGuia();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
    }

    public void FootstepReached()
    {
        if (!gameObject.activeSelf) return;

        if (cont == footstepsPositions.Length - 1)
        {
            gameWonEvent.Invoke();
        }
        else if (timer < 0f)
        {
            cont++;
            timer = 0.1f;
            rt.position = footstepsPositions[cont].position;

            if (footstepsPositions[cont].name.Contains("Wide"))
                rt.sizeDelta = new Vector2(footstepsPositions[cont].GetComponent<RectTransform>().sizeDelta.x, 100);
            else
                rt.sizeDelta = new Vector2(100, 100);

            // Mover la huella guía tenue al nuevo siguiente paso
            ActualizarHuellaGuia();
        }
    }

    private void CrearHuellaGuia()
    {
        UnityEngine.UI.Image miImagen = GetComponent<UnityEngine.UI.Image>();
        if (miImagen == null) return;

        huellaGuia = new GameObject("HuellaGuiaNext");
        // Se coloca en el mismo contenedor padre para mantener el orden de renderizado
        huellaGuia.transform.SetParent(transform.parent, false);

        imagenGuia = huellaGuia.AddComponent<UnityEngine.UI.Image>();
        imagenGuia.sprite = miImagen.sprite;

        // Establecer transparencia (30% de opacidad)
        Color colorTenue = miImagen.color;
        colorTenue.a = 0.3f;
        imagenGuia.color = colorTenue;

        rtGuia = huellaGuia.GetComponent<RectTransform>();
    }

    private void ActualizarHuellaGuia()
    {
        if (huellaGuia == null) return;

        int siguientePaso = cont + 1;

        // Si existe un paso siguiente en el circuito
        if (siguientePaso < footstepsPositions.Length)
        {
            huellaGuia.SetActive(true);
            rtGuia.position = footstepsPositions[siguientePaso].position;

            if (footstepsPositions[siguientePaso].name.Contains("Wide"))
            {
                rtGuia.sizeDelta = new Vector2(footstepsPositions[siguientePaso].GetComponent<RectTransform>().sizeDelta.x, 100);
            }
            else
            {
                rtGuia.sizeDelta = new Vector2(100, 100);
            }
        }
        else
        {
            // Ocultar la huella tenue al llegar al último paso
            huellaGuia.SetActive(false);
        }
    }
}