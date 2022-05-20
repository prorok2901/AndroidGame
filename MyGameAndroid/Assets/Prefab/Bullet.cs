using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 10;
    private RaycastHit ray;
    private Vector3 lastPosicion;
    public Transform player;
    private MovePlayer PlayerXP;
    public Transform NPC;
    private npcScript NPCxp;

    private void Start()
    {
        NPC = GameObject.FindGameObjectWithTag("NPC").transform;
        NPCxp = NPC.GetComponent<npcScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerXP = player.GetComponent<MovePlayer>();
        lastPosicion = this.transform.position;
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        if (Physics.Linecast(lastPosicion, transform.position, out ray))
        {
            if(ray.collider.CompareTag("Player"))
            {
                PlayerXP.DamageNPC(20f);
            }
            else if(ray.collider.CompareTag("NPC"))
            {
                NPCxp.DamagePlayer(50);
            }
            DeleteBullet();
        }
    }
    private void DeleteBullet()
    {
        Destroy(this.gameObject);
    }
}
