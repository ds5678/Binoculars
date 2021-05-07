using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing_release : MonoBehaviour
{
    Animator anim;
    float a = 0.5f;
    float f = 0.0f;
    int i = 0;

    //private IEnumerator coroutineTest;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Debug.Log(TMPro.TMP_Settings.version);
    }



    void IdleFluc()
    {
        if (f < 1.0f)
        {
            a += i == 0 ? 0.03f : -0.03f; // add if i is 0, otherwise substract

            if (a > 0.9f) i = 1;
            if (a < 0.1f) i = 0;

            anim.SetFloat("idle_random", a);

            f += 0.1f;
        }
        else
        {
            i = Random.Range(0, 2);
            f = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("bring");
            InvokeRepeating("IdleFluc", 0.0f, 0.1f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.SetBool("active", true); 
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetTrigger("idle_twiddle");

        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.SetBool("active", false);

        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetTrigger("put_down");

        }






    }
}
