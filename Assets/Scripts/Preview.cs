using UnityEngine;
using System;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    private Image SplashScreen;
    [Range(0.01f,5)] public float speed;
    public float delay = 1;
    public static event Action Endless;

    private void Start() {
        SplashScreen = GetComponent<Image>();
    }

    void Update()
    {   delay -= Time.deltaTime;
         if(delay <= 0) {
           SplashScreen.color -= new Color(0,0,0,speed) * Time.deltaTime;
           if(SplashScreen.color.a <= 0) {
            Endless.Invoke();
            Destroy(gameObject);
         }
       }
    }
}
