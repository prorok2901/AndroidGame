using UnityEngine;
using UnityEngine.UI;


public class MovePlayer : MonoBehaviour
{
    public CharacterController Player;
    public float Speed = 5;
    private Vector3 vectorMove;
    public Image ControlTrameMove;

    private float XP = 1000f;
    private bool rip = false;
        
    private ControlMove movePlayer;

    private void Start()
    {
        Player = GetComponent<CharacterController>();
        movePlayer = ControlTrameMove.GetComponent<ControlMove>();

    }
    void FixedUpdate()
    {
        if (XP >= 0)
            if(rip)
            Rip();
        else
            PlayerMone();
    }
    private void PlayerMone()
    {
        vectorMove = Vector3.zero;
        vectorMove.z = movePlayer.Horizontal() * -Speed;
        vectorMove.x = movePlayer.Vertical() * Speed;

        vectorMove = transform.TransformDirection(vectorMove);

        //передвижение самой модели 
        Player.Move(vectorMove * Time.deltaTime);
    }
    public void DamageNPC(float damage)
    {
        XP -= damage;
    }
    private void Rip()
    {
        rip = true;
        Destroy(this.gameObject);
    }
}
