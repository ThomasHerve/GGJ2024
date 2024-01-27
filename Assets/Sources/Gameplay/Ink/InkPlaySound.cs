using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGJ2024{

public class InkPlaySound : MonoBehaviour
{

    [SerializeField]
    public AudioSource audioSource;

    private List<PlayerController> m_Players;

    // Start is called before the first frame update
    void Start()
    {
        m_Players = GetComponents<PlayerController>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSource.isPlaying){
            if(m_Players.Any(x => x.IsInking))
                audioSource.Play();
        }
        else{
            if(!m_Players.Any(x => !x.IsInking))
                audioSource.Stop();
        }
    }
}
}