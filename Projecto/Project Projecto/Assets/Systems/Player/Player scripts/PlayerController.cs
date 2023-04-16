using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    //public GameObject Bullet;
    //public Transform weapon;
    public Interact CurrentTool;
    [SerializeField] float WalkSpeed;
    float horizontalMove;
    float VerticalMove;
    public Vector2 MoveDir;
    public bool CanAttack = true;
    public float playerHealth = 6f;
    public float MaxHealth = 6f;
    bool damageable = true;
    //Cam cam;
    public float Mana = 1f;
    float BulletVal = 0.05f;
    //Health UIHearts;
    Vector2 LastDir;
    Vector2 currentDir;
    public Vector2 DecidedDir;
    [SerializeField] float bulletPositionOffset;
    Vector3 BulletDirection;
    // Start is called before the first frame update
    void Start()
    {
        //cam = FindObjectOfType<Cam>();
        //UIHearts = FindObjectOfType<Health>();
        rb = GetComponent<Rigidbody2D>();   
        anim = GetComponent<Animator>();

        currentDir = MoveDir;
        LastDir = MoveDir;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        VerticalMove = Input.GetAxisRaw("Vertical");
        MoveDir = new Vector2(horizontalMove , VerticalMove);
        //to be applied in the gun tool
        //________________________________________
        //_______________________________________
        BulletDirection = DecidedDir * bulletPositionOffset;
        currentDir = MoveDir;

        Debug.Log("MoveDir: " + MoveDir + " , " + "CurrentDir: " + currentDir + " , "  + "LastDir: " + LastDir);

        Flip();
        Animation();
        interact();

        LastDirection();
        DirectionDesider();
    }
    private void FixedUpdate() 
    {
        Vector2 Target = new Vector2(transform.position.x + horizontalMove * WalkSpeed , transform.position.y + VerticalMove * WalkSpeed);
        rb.MovePosition(Target);   
    }
    void Flip()
    {
        Vector3 LocalScale = transform.localScale;
        if(horizontalMove > 0)
        {
            LocalScale.x = -1;
            transform.localScale = LocalScale;
        }
        else if(horizontalMove < 0)
        {
            LocalScale.x = 1;
            transform.localScale = LocalScale;   
        }
    }
    void Animation()
    {
        anim.SetInteger("speed",Mathf.RoundToInt(MoveDir.magnitude));
    }
    void interact()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentTool) {CurrentTool.interact();}
            
        }
    }

    public void TakeDamage(float Damage)
    {
        if(damageable)
        {
            StartCoroutine(DamageSequences(Damage));
        }
    }
    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    IEnumerator DamageSequences(float Damage)
    {
        playerHealth -= Damage;
        damageable = false;
        //UIHearts.CheckHearts();

        if (playerHealth <= 0f)
        {
            Die();
        }
        anim.SetTrigger("Hit");
        //cam.CamShake();
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("Damaged");
        yield return new WaitForSeconds(1.5f);
        damageable = true;
    }
        private void LastDirection()
    {
        if(currentDir != DecidedDir)
        {
            if(currentDir.magnitude != 0){LastDir = currentDir;    currentDir = MoveDir;}

            else{currentDir = MoveDir;}
        }
    }
    private void DirectionDesider()
    {
        if(currentDir.magnitude == 0)
        {
            if(LastDir.magnitude == 0)
            {
                DecidedDir.x = transform.localScale.x;
                DecidedDir.y = 0;
            }
            else
            {
                DecidedDir = LastDir;
            }
        }
        else
        {
            DecidedDir = currentDir;
        }
    }

    private void CurrentToolHandler()
    {
        //checks the inventory for the held tool
        //Assign it to the "current tool" variable

        Interact object_current_interact = Get_Current_Obj_from_inventory();
        //check if the current object is a tool
        if (GetType(object_current_interact))
            {
                CurrentTool = object_current_interact;
            }
    }

    private bool GetType (Interact obj)
    {
        return obj.getType() == "Tool";
    }

    private Interact Get_Current_Obj_from_inventory()
    {
        //getting current obj from inventory
        GameObject Current_obj_from_Inventory = null;
        return Current_obj_from_Inventory.GetComponent<Interact>();
    }



    //to be applied in the Gun tool
    //____________________________________
        void Attacksequences()
    {
        if(Input.GetKeyDown(KeyCode.Space) && CanAttack && Mana >= BulletVal)
        {
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        Mana -= BulletVal;
        CanAttack = false;
        anim.SetTrigger("Attack");
        //Instantiate(Bullet , weapon.position + BulletDirection , Quaternion.identity);
        yield return new WaitForSeconds(0.25f);
        CanAttack = true;
    }
}

