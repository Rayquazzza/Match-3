# 🍭 Match-3 Game Engine & Level Editor (C# / Unity)
<img src="Media/gameplay.gif" width="600" alt="Démonstration du Gameplay" />

Un moteur de jeu Match-3 complet, modulaire et hautement extensible développé sous Unity en C#. 

Ce projet met en avant des pratiques d'architecture logicielle rigoureuses : **programmation orientée contrats (Inversion de Dépendances)**, **découplage strict des responsabilités**, **optimisation mémoire (Zero-Allocation)** et **développement d'outils éditeur personnalisés sur-mesure**.

---

## 🎯 Points Forts de l'Architecture

* **Inversion de Dépendances & Service Locator :** Découplage total des sous-systèmes via des interfaces (`IMatchService`, `IScoreService`, `IAudioService`, `IEffectService`, etc.).
* **Modèle de Grille Découpé (MVC / Controller-Data) :** Séparation nette de la matrice de données (`GridData`), des calculs de combinaisons (`MatchChecker`, `MatchProcessor`), des déplacements (`GridShifter`, `GridSwap`) et de la vue (`GridVisualizer`).
* **Système d'Obstacles & Overlays Polymorphe :** Support dynamique d'éléments destructibles à points de vie (`WoodBlock`) et de surcouches de blocage brisables (`IceOverlay`) basés sur l'abstraction `GridItem`.
* **Éditeur de Niveaux Personnalisé (Tool Dev) :** Fenêtre d'édition visuelle dans Unity (`GridEditorWindow`) avec peinture de grilles à la souris, gestion multi-couches (Base/Overlay) et configuration du pool de couleurs.
* **Audio & Effets Réactifs (Observer Pattern) :** Gestion centralisée du son via FMOD (`SoundEvent`) réagissant aux événements de gameplay sans couplage direct.
* **Performance & Zero-Allocation :** Utilisation systématique de l'**Object Pooling** (`IPoolingService`) pour éliminer les allocations sur le Heap durant le gameplay et éviter les pics de Garbage Collection.

---

## 🏗️ Architecture & Design Patterns

### 1. Inversion de Dépendance & Service Locator
L'ensemble des services système communique via le `GameServiceLocator`. Les composants ne connaissent jamais la classe concrète qui exécute l'action :

```csharp
// Exemple de destruction d'un overlay de glace libérant la case sous-jacente
public class IceOverlay : GridItem
{
    public override bool IsMovable => false;
    public override bool IsMatchable => false;

    public override void BreakLayer()
    {
        // Appel au service d'effets visuels sans dépendance directe
        GameServiceLocator.Get<IEffectService>().PlayExplosion(transform.position);

        Vector2Int pos = grid.GetPositionOf(this);
        if (pos.x != -1) 
        {
            grid.ClearOverlayAt(pos.x, pos.y);
        }
    }
}
```
### 2. Chef d'Orchestre de Grille (`GridController`)
Le `GridController` orchestre des composants spécialisés ayant chacun une responsabilité unique (Single Responsibility Principle)

| Composant | Rôle & Responsabilité |
| :--- | :--- |
| **GridData** | Stocke la matrice 2D des objets, gère les overlays et valide les positions. |
| **GridSpawner** | Génère la grille sans match initial et gère le réapprovisionnement avec auto-shuffle si aucun coup n'est possible. |
| **GridSwap** | Valide et anime les permutations de tuiles, annule les mouvements invalides. |
| **MatchChecker & MatchProcessor** | Algorithmes d'analyse matricielle (L/T/5 alignés) et gestion des cascades/combos. |
| **GridShifter** | Calcule la gravité et la chute fluide des éléments. |
| **GridVisualizer** | Gère le positionnement spatial et l'interpolation graphique avec DOTween. |

### 3. Effets Spécialisés & Combinaisons (`ScriptableObject`)
Les effets des bonbons spéciaux (Lignes horizontales/verticales, Explosions 3x3) sont encapsulés dans des `ScriptableObjects` héritant de `SpecialEffect`.
Cela permet d'ajouter de nouveaux bonus sans modifier le code de la grille :

```csharp
[CreateAssetMenu(menuName = "Match3/Effects/Explosion")]
public class ExplosionEffect : SpecialEffect
{
    public override void Execute(int x, int y, GridData grid, GridController controller)
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (grid.IsValidPos(i, j))
                {
                    grid.ClearMatchAt(i, j);
                }
            }
        }
    }

    public override void ExecuteCombo(GridItem source, GridItem target, GridData grid, GridController controller) { }
}

```

## 🛠️ Tool Dev : Unity Level Editor
Pour accélérer la création de contenu, un outil éditeur personnalisé (`GridEditorWindow`) a été développé pour Unity (`EditorWindow`) :

* **Painting Interactif :** Peinture à la souris (clic gauche pour placer, clic droit pour effacer).
* **Multi-Layering :** Bascule instantanée entre la couche de base (Bonbons / Obstacles) et la couche de surcouche (`IceOverlay`).
* **Configuration Dynamique :** Redimensionnement à la volée de la grille (`Width x Height`) et sélection visuelle des bonbons autorisés au tirage.

[Tools/Level Editor] -> Ouvre l'éditeur visuel synchronisé avec les ScriptableObjects LevelData.

## 🎧 Audio & Feedback Visuel

* **FMOD Integration :** Gestion des événements audio (`SoundEvent`) avec paramètres dynamiques pour augmenter le pitch ou la variation en fonction du niveau de combo (`Combo`).
* **Propreté Visuelle(`BackgroundTile`):** Adaptation dynamique des bordures et coins de la grille selon la géométrie du niveau (gestion des cases vides/invalides).
* **Caméra Responsive(`CameraFitter`) :** Calcul automatique du champ de vision et du zoom de la caméra selon le ratio de l'écran et la taille de la grille.

## 🛠️ Stack Technique

* **Moteur :** Unity 2022+ / C#
* **Tweening :** DOTween (DOTween Pro)
* **Audio Engine :** FMOD Unity Integration
* **Architecture :** Service Locator, Observer, Object Pooling, Scriptable Object Data Driven
