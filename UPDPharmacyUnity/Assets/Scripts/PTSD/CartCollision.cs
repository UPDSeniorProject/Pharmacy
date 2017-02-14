using UnityEngine;
using System.Collections;


//code handles cart movement when colliding with shelves
//sets triggers for animations to play on collision

public class CartCollision : MonoBehaviour {
    public Transform ShoppingCart;
    private Animator cartAnimator;
    private bool isRightCol;
    private bool isColliding;
    public Transform otherCol;
    private float timer;

    void Start()
    {
        isRightCol = transform.name.Equals("ColliderRight");
        cartAnimator = ShoppingCart.GetComponent<Animator>();
    }
    
    void OnTriggerEnter(Collider coll)
    {
        if (cartAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CartIdle" && (coll.name.Contains("Collision") || coll.name.Contains("Plane")))
        {
            timer = 0;
            cartAnimator.ResetTrigger("moveLeft");
            cartAnimator.ResetTrigger("moveRight");

            if (isRightCol)
                cartAnimator.SetTrigger("moveLeft");
            else
                cartAnimator.SetTrigger("moveRight");

            ShoppingCart.GetComponent<Collider>().enabled = false;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.name.Contains("Collision") || coll.name.Contains("Plane"))
        {
            timer = 0;
            isColliding = true;
            if(cartAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CartIdle")
            {
                cartAnimator.ResetTrigger("moveLeft");
                cartAnimator.ResetTrigger("moveRight");

                if (isRightCol)
                    cartAnimator.SetTrigger("moveLeft");
                else
                    cartAnimator.SetTrigger("moveRight");

                ShoppingCart.GetComponent<Collider>().enabled = false;
            }
        }
    }

    void Update()
    {
        if (!ShoppingCart.GetComponent<Renderer>().enabled)
            ShoppingCart.GetComponent<Collider>().enabled = false;
        timer += Time.deltaTime;
        if (!isColliding && !otherCol.GetComponent<CartCollision>().isColliding)
        {
            if (cartAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CartIdle" && timer >= 3)
            {
                cartAnimator.ResetTrigger("leftCenter");
                cartAnimator.ResetTrigger("rightCenter");
                ShoppingCart.GetComponent<Collider>().enabled = true;
            }

            cartAnimator.ResetTrigger("moveLeft");
            cartAnimator.ResetTrigger("moveRight");

            if (cartAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CartLeftIdle")
            {
                cartAnimator.SetTrigger("leftCenter");
                timer = 0;
            }
            else if (cartAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "CartRightIdle")
            {
                cartAnimator.SetTrigger("rightCenter");
                timer = 0;
            }

        }
    }
    
    void LateUpdate()
    {
        isColliding = false;
    }
}
