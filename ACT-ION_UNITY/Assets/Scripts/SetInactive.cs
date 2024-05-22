using UnityEngine;
using UnityEngine.Audio;

public class SetInactive : MonoBehaviour
{
    public GameObject target;


    AudioMixer mixer;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I am " +GameSingleton.GetInstance().playerClientID.ToString());
        }
        if (Input.GetKey(KeyCode.O))
        {
            target.SetActive(true);
        }
    }
}
