using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public Sprite[] sprites;
    public RawImage rawImage;
    public float time;

    private float currentTime = 0;
    private int frame = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(time < currentTime)
        {
            currentTime = 0;
            frame++;
            if(frame > sprites.Length - 1)
            {
                frame = 0;
            }
            rawImage.texture = sprites[frame].texture;
        }
    }
}
