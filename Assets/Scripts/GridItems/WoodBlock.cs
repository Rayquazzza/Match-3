using DG.Tweening;
using UnityEngine;

public class WoodBlock : GridItem
{
    [SerializeField] private int maxHealth = 2;
    private int currentHealth = 0;
    private SpriteRenderer sr;
    private float initialAlpha = 1f;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = initialAlpha;
        sr.color = c;
        
        currentHealth = maxHealth;
    }

    public override bool IsMovable => false;
    public override bool IsMatchable => false;

    public override void OnNearbyMatch()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            // On le retire de la grille logique avant de le détruire
            // pour que les bonbons puissent tomber immédiatement
            Vector2Int pos = grid.GetPositionOf(this);
            if (pos.x != -1) grid.AllItems[pos.x, pos.y] = null;

            OnDestroyItem();
        }
        else
        {
            Color c = sr.color;
            c.a -= 0.4f;
            sr.color = c;

            transform.DOShakePosition(0.2f, 0.1f);
        }
    }

    public override void OnDestroyItem()
    {
        // On peut jouer un effet de débris ici
        GameServiceLocator.Get<IEffectService>().PlayExplosion(transform.position);

        // Comme c'est un obstacle, on peut donner plus de points
        GameServiceLocator.Get<IScoreService>().AddScore(200);

        // Retour au pool
        GetComponent<PoolMember>().ReturnToPool();
    }
}