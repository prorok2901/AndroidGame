using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlMove : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 posicionControl;
    public Image ControlTrameMove;
    public Image controlMove;
    private Vector2 InputVector;

    private float noiseMove;

    void Start()
    {
        ControlTrameMove = GetComponent<Image>();
        controlMove = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //если совсем просто, то это: сравнения угла отклонения меж центром объекта касания и местом касания
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlTrameMove.rectTransform, eventData.position, eventData.pressEventCamera, out posicionControl))
        {

            posicionControl.x /= ControlTrameMove.rectTransform.sizeDelta.x;
            posicionControl.y /= ControlTrameMove.rectTransform.sizeDelta.y;

            InputVector = new Vector2(posicionControl.x * 2 - 1, posicionControl.y * 2 - 1); //установка точных координат из касания
            if (InputVector.magnitude >= 1.0f) 
            {
                InputVector = InputVector.normalized;
            }
            noiseMove = InputVector.magnitude;

            controlMove.rectTransform.anchoredPosition = new Vector2(InputVector.x * (ControlTrameMove.rectTransform.sizeDelta.x / 2), InputVector.y * (ControlTrameMove.rectTransform.sizeDelta.y / 2));
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        controlMove.rectTransform.anchoredPosition = Vector2.zero;
    }

    public float Horizontal()
    {
        return InputVector.x;
    }
    public float Vertical()
    {
        return InputVector.y;
    }
    

    public bool NoisePlayer()
    {
        if(Mathf.Pow(noiseMove,2) >= 1)
        {
            return true;
        }
        return false;
    }
}
