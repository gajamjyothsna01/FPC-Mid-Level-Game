using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   public Animator animator;
    public float playerSpeed;
    public float playerJumpForce;
    public GameObject steveModelPrefab;
    //public GameObject target;
    Rigidbody rb;
    public float rotationSpeed;
    public Camera cam;
    CapsuleCollider capsuleCollider;
    Quaternion playerRotataion, camRotation;
    public float minX = -90;
    public float maxX = 90;
    float inputX, inputZ;
    int ammo = 100;
    int medical = 100;
    int maxAmmo = 100;
    int maxMedical = 100;
    int reloadAmmo = 0;
    int maxReloadAmmo = 100;
    public Transform bulletLaunch; //Point to launch the Bullets.
    
   // public AudioSource audioSource;
   // bool isGrounded = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        //cam = GetComponent<Camera>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("IsAiming", !animator.GetBool("IsAiming"));
        }
       if(Input.GetMouseButton(0) && !animator.GetBool("IsFiring"))
        {
            if(ammo > 0)
            {
                // animator.SetBool("isFiring", !animator.GetBool("isFiring"));
                animator.SetTrigger("IsFiring");
                WhenZombieeGotHit();
                ammo = Mathf.Clamp(ammo - 10, 0, maxAmmo);
               // Debug.Log(ammo);
            }
            else
            {
                //Trigger the sound for empty bullets.
            }
            
        }
       if(Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("IsReload");
            int amountAmmoNeeded = maxReloadAmmo - reloadAmmo;
            int ammoAvailable = amountAmmoNeeded < ammo ? amountAmmoNeeded : ammo;
            ammo -= ammoAvailable;
            reloadAmmo += ammoAvailable;
            Debug.Log("Ammo Left" + ammo);
            Debug.Log("Ammo Reloaded" + reloadAmmo);
        }
       if(Mathf.Abs(inputX)>0 || Mathf.Abs(inputZ)>0)
        {
            if(!animator.GetBool("IsWalking"))
            {
                animator.SetBool("IsWalking", true);
            }
           
        }
       else if(animator.GetBool("IsWalking"))
        { 
            animator.SetBool("IsWalking", false);
        }



       
        //transform.Translate(inputX*playerSpeed, inputZ*playerSpeed, );
    }

    private void WhenZombieeGotHit()
    {
        RaycastHit hitInfo;
        //Launching The ray from the Bullet Lauch to get whether it hits zombiee or not.
        if (Physics.Raycast(bulletLaunch.position, bulletLaunch.forward, out hitInfo, 100f))
        {
            GameObject hitZombiee = hitInfo.collider.gameObject;
            if(hitZombiee.tag =="Zombiee")
            {
                if(UnityEngine.Random.Range(0, 10) < 5)
                {
                    GameObject tempRagDoll = hitZombiee.GetComponent<ZombieController>().ragDollPrefab;
                    GameObject newTempRagDoll = Instantiate(tempRagDoll, hitZombiee.transform.position, hitZombiee.transform.rotation);
                    newTempRagDoll.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
                    Destroy(hitZombiee);
                }
                else
                {
                    /*
                    hitZombiee.GetComponent<ZombieController>().TurnOffAllTriggerAnim();
                    hitZombiee.GetComponent<ZombieController>().anim.SetBool("isDead", true);
                    hitZombiee.GetComponent<ZombieController>().state = hitZombiee.GetComponent<ZombieController>().Sta.DEAD;*/
                    hitZombiee.GetComponent<ZombieController>().KillZombiee();

                }

            }
        }
    }

    private void FixedUpdate()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        transform.position = transform.position + new Vector3(inputX * playerSpeed,0, inputZ * playerSpeed);
        
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * playerJumpForce);
        }
        float mouseX = Input.GetAxis("Mouse X")*rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y")*rotationSpeed;



        playerRotataion = Quaternion.Euler(0,mouseY, 0) * playerRotataion; // rOTATION IN X
       // Debug.Log(playerRotataion);
        camRotation = ClampRotationOfPlayer(Quaternion.Euler(-mouseX, 0, 0)*camRotation);//Rotation in y
        //Debug.Log("camRotation" + camRotation);
        this.transform.localRotation = playerRotataion;
        cam.transform.localRotation = camRotation;
        //playerRotataion = transform.localRotation;



    }
    bool IsGrounded()
    {
        RaycastHit rayCasthit;
        if (Physics.SphereCast(transform.position, capsuleCollider.radius, Vector3.down,out rayCasthit, (capsuleCollider.height/2 ) - capsuleCollider.radius + 0.1f))
        {
            return true;

        }
        else
        {
            return false;
        }         

    }
    
    Quaternion ClampRotationOfPlayer(Quaternion n) //clamp - restricts the player rotation by maximum and minimumv value.
    {
        n.w = 1f;
        n.x /=n.w;
        n.y /= n.w;
        n.z /= n.w;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(n.x);
        angleX = Mathf.Clamp(angleX,minX,maxX);
        n.x = Mathf.Tan(Mathf.Deg2Rad *0.5f* angleX);
        return n;

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ammo" && ammo < maxAmmo)
                {
                    collision.gameObject.SetActive(false);
                         Debug.Log("Collected Ammo Box");
                   // ammo = ammo + 10;
            ammo = Mathf.Clamp(ammo + 10, 0,maxAmmo);
            Debug.Log("Ammo" + ammo);
        }
        if (collision.gameObject.tag == "Med" && medical < maxMedical)
        {
            collision.gameObject.SetActive(false);
            Debug.Log("Collected Med Box");
           // medical = medical + 10;
            medical = Mathf.Clamp(medical + 10, 0, maxMedical) ;


        }
        else if(collision.gameObject.tag == "Lava")
        {
            //Need to Trigger dead Sound, when medical is less than zero.
            if(medical < 0)
            {
                //audioSource.Play();
            }
            medical = Mathf.Clamp(medical - 10, 0, maxMedical) ;
            Debug.Log("Medical " + medical);
        }
    }
    public void TakeHit(float value) //Valuw will be decreased some part from Health.
    {
        medical = (int)(Mathf.Clamp(medical - value, 0, maxMedical));
        Debug.Log(medical);
        if(medical <= 0)
        {
            Vector3 position = new Vector3(transform.position.x, Terrain.activeTerrain.SampleHeight(this.transform.position), transform.position.z);
            GameObject tempSteve = Instantiate(steveModelPrefab,position,this.transform.rotation);
            tempSteve.GetComponent<Animator>().SetTrigger("Death");
            Debug.Log("Player is died");
            GameStarts.isGameOver = true; 
            Destroy(this.gameObject);
            
        }


    }
    
   
}
