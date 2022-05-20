using UnityEngine;
using UnityEngine.UI;
public class npcScript : MonoBehaviour
{
    private float XP = 100;
    public float speed;
    
    //для патрулирования
    public Transform[] moveSpots;
    private int numberSpots;
    bool PatrulSpots = false;
    public float startWaitTime = 10;
    private float waitTime;

    //для поиска игрока
    public GameObject prefab;
    public Transform Gan;
    private float TimeShot;
    public float DelaytTime;
    [SerializeField] private Transform Player;
    [Range(0, 180)] public float ViewAngle = 45f;//поле зрения;
    public float speedRotation = 10;

    //после потери игрока поиск относительно последнего положения игрока
    private Transform lastPositionPlayer;

    public float Dist = 10;
    public float StopDistance = 5;
    RaycastHit hitBack;
    RaycastHit hit;

    public float rotationSpeed = 100;
    float realAngle;

    bool StopTowards = false;
    public Image PlayerMove;
    ControlMove controlMove;
    MovePlayer PlayerXP;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        controlMove = PlayerMove.GetComponent<ControlMove>();
        PlayerXP = Player.GetComponent<MovePlayer>();
        numberSpots = 0;
    }
    private void FixedUpdate()
    {
        if (XP > 0)
        {
            if (DistantToTarget(Player.transform, transform) < Dist)
            {
                if (IsInView(Player) || HearingArea())//видит или слышит ли бот игрока
                {
                    lastPositionPlayer = Player;//запоминаем позицию игрока, в случае когда игрок сткроется пойдём по эти координатам
                    BulletCreator();
                    RotateToTarget(Player);
                    if (DistantToTarget(Player.transform, transform) < StopDistance && DistantToTarget(Player.transform, transform) > Dist)
                    {
                        transform.Translate(transform.position);
                    }
                    else if (DistantToTarget(Player.transform, transform) < StopDistance)
                    {
                        if (Back())
                        {
                            if (DistantToTarget(Player.transform, transform) < 1.5f)
                            {
                                QuickKill();
                            }
                            else
                                transform.position = DistansNPC(speed, Player);

                        }
                        else
                            transform.position = DistansNPC(-speed, Player);
                    }
                }
                else
                {
                    Patrul();
                }
            }
            else
            {
                Patrul();
            }
        }
        else Rip(); 
    }
    //Патруль по точкам
    private void Patrul()
    {
        if(StopTowards == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveSpots[numberSpots].position, speed * Time.deltaTime);
            RotateToTarget(moveSpots[numberSpots]);
            if (DistantToTarget(moveSpots[numberSpots], transform) < 1f)
            {
                StopTowards = true;
            }
        }
        else
        {
            if (waitTime <= 0)
            {
                if (numberSpots == 3)
                {
                    PatrulSpots = true;
                }
                else if(numberSpots == 0)
                    PatrulSpots = false;

                if (PatrulSpots)
                    numberSpots--;
                else
                    numberSpots++;

                waitTime = startWaitTime;
                StopTowards = false;
            }
            else
            {
                if (numberSpots == 0 || numberSpots == 3)
                    waitTime -= Time.deltaTime;
                else waitTime = 0;
            }  
        }
    }

    //стрельба
    private void BulletCreator()
    {

        if (TimeShot <= 0)
        {
            TimeShot = DelaytTime;
            Instantiate(prefab, Gan.position, Gan.rotation);
        }
        else TimeShot -= Time.deltaTime;
    }

    //сокращешие повторяющих частей кода(дистанция до цели(игрок/точка))
    float DistantToTarget(Transform targetNPC, Transform npcTipo)
    {
        return Vector3.Distance(transform.position, targetNPC.position);
    }

    //сокращение большого куска кода, связанный с направлением движения
    Vector3 DistansNPC(float speed, Transform target)
    {
        return Vector3.MoveTowards(new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(target.position.x, transform.position.y, target.position.z), speed * Time.deltaTime);
    }

    // виден ли игрок цель
    private bool IsInView(Transform target) 
    {
        realAngle = Vector3.Angle(transform.forward, target.position - transform.position);
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, Dist) )
        {
            if (realAngle < ViewAngle && hit.transform == target.transform)
            {
                return true;
            }
        }
        return false;
    }

    //поворачивает в стороно цели со скоростью rotationSpeed
    private void RotateToTarget(Transform target)
    {
        Vector3 vectorLook = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(vectorLook.x, 0, vectorLook.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * speedRotation);
    }

    // слух бота при громком движении игрока
    private bool HearingArea()
    {
        if (DistantToTarget(Player.transform, transform) < 5f)
        {
            return controlMove.NoisePlayer();
        }
        return false;
    }

    // есть ли стена сзади?
    private bool Back()
    {
        if(Physics.Raycast(transform.position, -(Player.position - transform.position), out hitBack, 0.5f))
        {
            if(hitBack.collider.name == "map_1")
            {
                return true;
            }
        }
        return false;
    }

    //quick kill - быстрое убийство
    private void QuickKill()
    {
        PlayerXP.DamageNPC(100f);
    }
    public void DamagePlayer(float damage)
    {
        XP -= damage;
    }
    private void Rip()
    {
        Destroy(this.gameObject);
    }
}
