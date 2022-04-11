using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   public Animator animator;
    public float playerSpeed;
    public float playerJumpForce;
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
    int maxMed = 100;
    int reloadAmmo = 0;
    int maxReloadAmmo = 100;
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
            animator.SetBool("isAiming", !animator.GetBool("isAiming"));
        }
       if(Input.GetMouseButton(0) && !animator.GetBool("isFiring"))
        {
            if(ammo > 0)
            {
                // animator.SetBool("isFiring", !animator.GetBool("isFiring"));
                animator.SetTrigger("isFiring");
                ammo = Mathf.Clamp(ammo - 10, 0, maxAmmo);
                Debug.Log(ammo);
            }
            else
            {
                //Trigger the sound for empty bullets.
            }
            
        }
       if(Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("isReloading");
            int amountAmmoNeeded = maxReloadAmmo - reloadAmmo;
            int ammoAvailable = amountAmmoNeeded < ammo ? amountAmmoNeeded : ammo;
            ammo -= ammoAvailable;
            reloadAmmo += ammoAvailable;
            Debug.Log("Ammo Left" + ammo);
            Debug.Log("Ammo Reloaded" + reloadAmmo);
        }
       if(Mathf.Abs(inputX)>0 || Mathf.Abs(inputZ)>0)
        {
            if(!animator.GetBool("isWalking"))
            {
                animator.SetBool("isWalking", true);
            }
           
        }
       else if(animator.GetBool("isWalking"))
        { 
            animator.SetBool("isWalking", false);
        }



       
        //transform.Translate(inputX*playerSpeed, inputZ*playerSpeed, );
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
        if (collision.gameObject.tag == "Med" && medical < maxMed)
        {
            collision.gameObject.SetActive(false);
            Debug.Log("Collected Med Box");
           // medical = medical + 10;
            medical = Mathf.Clamp(medical + 10, 0, maxMed) ;


        }
        else if(collision.gameObject.tag == "Lava")
        {
            //Need to Trigger dead Sound, when medical is less than zero.
            if(medical < 0)
            {
                //audioSource.Play();
            }
            medical = Mathf.Clamp(medical - 10, 0, maxMed) ;
            Debug.Log("Medical " + medical);
        }
    }
}
