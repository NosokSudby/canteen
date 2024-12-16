using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DontChangeScale : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            this.transform.localScale = new Vector3(3.366371f, 3.36637f, 2.953115f);
        }
    }
}
