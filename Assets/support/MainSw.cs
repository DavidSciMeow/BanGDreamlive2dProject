using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(()=>{
            SceneManager.LoadScene("player");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
