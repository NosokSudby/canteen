using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DontChangeScale : MonoBehaviour
{
    private Vector3 initialScale;
    // Start is called before the first frame update
    void Start()
    {
        // Сохраняем начальный масштаб дочернего объекта
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
            if (transform.parent != null)
            {
                transform.localScale = new Vector3(
                    initialScale.x / transform.parent.lossyScale.x,
                    initialScale.y / transform.parent.lossyScale.y,
                    initialScale.z / transform.parent.lossyScale.z
                );
            }
            else
            {
                // Если у объекта нет родителя, просто сохраняем начальный масштаб
                transform.localScale = initialScale;
            }
        
        
    }
}
