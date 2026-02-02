using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Candy))]
public class CandyInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 firstTouchPosition;
    private Vector2 lastTouchPosition;
    private Candy candy;

    void Start()
    {
        candy = GetComponent<Candy>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        lastTouchPosition = Camera.main.ScreenToWorldPoint(eventData.position);
        CalculateDirection();
    }

    private void CalculateDirection()
    {
        float swipeThreshold = 0.5f;
        Vector2 delta = lastTouchPosition - firstTouchPosition;

        if (delta.magnitude > swipeThreshold)
        {
            Vector2Int direction = Vector2Int.zero;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                direction = new Vector2Int(delta.x > 0 ? 1 : -1, 0);
            }
            else
            {
                direction = new Vector2Int(0, delta.y > 0 ? 1 : -1);
            }

            GameServiceLocator.Get<IMoveService>().AttemptSwap(candy, direction);
        }
    }
}