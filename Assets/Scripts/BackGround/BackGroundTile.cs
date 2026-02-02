using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    [Header("Bordures (Lignes)")]
    public GameObject borderUp;
    public GameObject borderDown;
    public GameObject borderLeft;
    public GameObject borderRight;

    [Header("Coins Extérieurs (Arrondis du bord)")]
    public GameObject cornerTL;
    public GameObject cornerTR;
    public GameObject cornerBL;
    public GameObject cornerBR;

    [Header("Coins Intérieurs (Intersections du L)")]
    public GameObject innerTL;
    public GameObject innerTR;
    public GameObject innerBL;
    public GameObject innerBR;

    public void UpdateVisual(bool up, bool down, bool left, bool right,
                             bool tl, bool tr, bool bl, bool br)
    {
        // 1. Les lignes droites (on les affiche si PAS de voisin direct)
        borderUp.SetActive(!up);
        borderDown.SetActive(!down);
        borderLeft.SetActive(!left);
        borderRight.SetActive(!right);

        // 2. Les coins extérieurs (on les affiche si les deux côtés sont vides)
        if (cornerTL) cornerTL.SetActive(!up && !left);
        if (cornerTR) cornerTR.SetActive(!up && !right);
        if (cornerBL) cornerBL.SetActive(!down && !left);
        if (cornerBR) cornerBR.SetActive(!down && !right);

        // 3. LA MAGIE : Les coins intérieurs (Intersections)
        // On l'affiche si on a deux voisins directs MAIS que la diagonale est VIDE
        if (innerTL) innerTL.SetActive(up && left && !tl);
        if (innerTR) innerTR.SetActive(up && right && !tr);
        if (innerBL) innerBL.SetActive(down && left && !bl);
        if (innerBR) innerBR.SetActive(down && right && !br);
    }
}