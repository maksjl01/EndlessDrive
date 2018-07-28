using UnityEngine;
using UnityEngine.EventSystems;

public class SelectCar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private Vector2 m_startTouch;
    private Vector2 m_endTouch;

    public enum swipes { up, right, down, left }
    public swipes currentSwipe;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_startTouch = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_endTouch = eventData.position;

        Vector2 dir = Vector3.Normalize(new Vector3(m_endTouch.x - m_startTouch.x, m_endTouch.y - m_startTouch.y));
        
        if(m_startTouch - m_endTouch != Vector2.zero)
        {
            if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                currentSwipe = (dir.x > 0) ? swipes.right : swipes.left;
                GameManager.instance.SwapCar(currentSwipe);
            }
            else
            {
                currentSwipe = (dir.y > 0) ? swipes.up : swipes.down;
                if (currentSwipe == swipes.down)
                    GameManager.instance.StartGame();
            }
        }
    }
}
