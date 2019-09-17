using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneScript : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        GameObject rocket = GameObject.Find("myrocket(Clone)");
        Destroy(rocket);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
