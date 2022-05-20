using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlFire : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    //для передвижения джостика
    private Vector2 posicionControl;
    public Image ControlTrameFire;
    public Image controlFire;
    private Vector2 InputVector;//полученные координаты джостика

    //поворот персонажа
    public float Sensitiviry = 10;
    public Transform AxisGan;

    //стрельба
    public GameObject prefab;
    public Transform Gan;
    private float TimeShot;
    public float DelaytTime;
    private bool Clip = false;//для того, что б стрельба не прекращалась

    void Start()
    {
        ControlTrameFire = GetComponent<Image>();
        controlFire = transform.GetChild(0).GetComponent<Image>();
    }
    void FixedUpdate()
    {
        if (Clip == true)
        {
            BulletCreator();
        }
    }
    //удержавание
    public virtual void OnDrag(PointerEventData eventData)
    {
        //если совсем просто, то это: сравнения угла отклонения меж центром объекта касания и местом касания
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(ControlTrameFire.rectTransform, eventData.position, eventData.pressEventCamera, out posicionControl))
        {
            posicionControl.x /= ControlTrameFire.rectTransform.sizeDelta.x;//получение точных координат по оси x
            posicionControl.y /= ControlTrameFire.rectTransform.sizeDelta.y;//получение точных координат по оси y

            InputVector = new Vector2(posicionControl.x * 2 - 1, posicionControl.y * 2 - 1); //установка точных координат из касания
            //предотвращаем вылет якоря стика путём нормализации вектора до 1
            if (InputVector.magnitude > 1.0f)
            {
                InputVector = InputVector.normalized;
                Clip = true;
            }
            else Clip = false;
            Horizontal();
            controlFire.rectTransform.anchoredPosition = new Vector2(InputVector.x * (ControlTrameFire.rectTransform.sizeDelta.x / 2), InputVector.y * (ControlTrameFire.rectTransform.sizeDelta.y / 2));//перемещаем якорь согласно формуле
        }
    }
    //активируем при нажатии
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    //обнуляем вектор и ставим якорь стика на центр джостика
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Clip = false;
        InputVector = Vector2.zero;
        controlFire.rectTransform.anchoredPosition = Vector2.zero;
    }

    //поворот пушки(для тестового объекта) с помощью формулы Атангенса, так как как там до 180 градусов, используем Атангенс2(чисто фишка юнити)
    private void Horizontal()
    {
        AxisGan.localRotation = Quaternion.Euler(0, Mathf.Atan2(posicionControl.x * 2 - 1, posicionControl.y * 2 - 1) * Mathf.Rad2Deg, 0);
    }
    //спавн пули
    private void BulletCreator()
    {
        if (TimeShot <= 0)
        {
            TimeShot = DelaytTime;
            Instantiate(prefab, Gan.position, Gan.rotation);
        }
        else TimeShot -= Time.deltaTime;
    }
}
