using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    Vector2 screenMin, screenMax;

    Vector2 XYmin = new Vector2(-9.5f, -1f);
    Vector2 XYmax = new Vector2(-3.7f, 4.7f);
    float x0 = 0.0f, y0 = 0.0f;
    float flightSpeed = 4.0f;
    float rocketSpeed = 10.0f;
    float launchTime;
    bool isLaunched = false;
    int flightIndex = 0;
    Vector3 targetSpeed;
    Vector3 targetPos;

    public GameObject plane;
    public GameObject rocket;
    public GameObject target;
    GameObject myplane;
    GameObject myrocket;

    float myTime;
    void Start()
    {
        screenMin = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        screenMax = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        myTime = 0.0f;
        flightIndex = Random.Range(0, 5);
        flightSpeed = Random.Range(2.0f, 4.0f);
        x0 = Random.Range(XYmin.x, XYmax.x);
        y0 = Random.Range(XYmin.y, XYmax.y);
        if (!myplane)
            myplane = Instantiate(plane);
        myplane.transform.position = new Vector3(x0, y0);
    }

    Vector2 FlightFunction(float t, int fIndex)
    {
        float x = x0 + t;
        float[] y = { y0 + 0.5f * Mathf.Sin(t),
                      y0 + 0.1f * Mathf.Cos(t),
                      y0 - 1.8f * t + 0.5f,
                      y0 + 2.1f * t,
                      y0,
                      y0 + 1.2f * Mathf.Sin(t)
                    };

        return new Vector2(x, y[fIndex]);
    }

    void FixedUpdate()
    {
        myTime += Time.deltaTime;
        if (isLaunched && Time.time - launchTime >= 3.0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (!myplane)
            return;
        GameObject launcher = gameObject;
        float xs0 = myplane.transform.position.x;
        float ys0 = myplane.transform.position.y;

        Vector3 nextPoint = FlightFunction(myTime, flightIndex);

        if(nextPoint.x > screenMax.x || nextPoint.y > screenMax.y ||
            nextPoint.x < screenMin.x || nextPoint.y < screenMin.y)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        targetSpeed = (nextPoint - myplane.transform.position) / Time.deltaTime;
        myplane.transform.position = Vector2.MoveTowards(myplane.transform.position, nextPoint, Time.deltaTime * flightSpeed);
        float x = nextPoint.x; float y = nextPoint.y;

        //plane rotation
        Vector2 moveDirection = new Vector2(x - xs0, y - ys0);
        if(moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            myplane.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        //launcher rotation
        Vector2 launcherDirection = new Vector2(x - launcher.transform.position.x, y - launcher.transform.position.y);
        if(launcherDirection != Vector2.zero)
        {
            float launcherAngle = Mathf.Atan2(launcherDirection.y, launcherDirection.x) * Mathf.Rad2Deg;
            launcher.transform.rotation = Quaternion.AngleAxis(launcherAngle, Vector3.forward);
        }

        //targeting
        Vector3 tarPos = nextPoint;
        for (int i = 0; i < 10; i++)
        {
            float dist = (launcher.transform.position - tarPos).magnitude;
            float timeToTarget = dist / rocketSpeed;
            tarPos = nextPoint + targetSpeed * timeToTarget;
        }
        if(target)
            target.transform.position = tarPos;

        if(myrocket)
        {
            myrocket.transform.rotation = transform.rotation;
            myrocket.transform.position = Vector3.MoveTowards(myrocket.transform.position, targetPos, Time.deltaTime * rocketSpeed);
        }
    }

    void OnMouseDown()
    {
        if(!myrocket && myplane)
        {
            myrocket = Instantiate(rocket, transform.position, transform.rotation);
            targetPos = target.transform.position;
            Destroy(target);
            launchTime = Time.time;
            isLaunched = true;
        }
    }
}
