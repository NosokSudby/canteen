using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canteenTableSc : MonoBehaviour
{
    public bool firstBenchDown = false;
    public bool secondBenchDown = false;

    public Transform bench1;
    public Transform bench2;

    private void Start()
    {
        bench1 = this.transform.Find("firstBench");
        bench2 = this.transform.Find("secBench");
    }
    void Update()
    {
        if(firstBenchDown == false)
        {
            Vector3 benchPos = new Vector3(0.00661411509f, 0.254999995f, 0.488000005f);
            bench1.localPosition = benchPos;
        }
        else
        {
            Vector3 benchPos = new Vector3(0.00661411509f, 0.17933172f, 0.6679492f);
            bench1.localPosition = benchPos;
        }
        if(bench2 != null)
        {
            if (secondBenchDown == false)
            {
                Vector3 secBenchPos = new Vector3(0.00661411509f, 0.254999995f, -0.222000003f);
                bench2.localPosition = secBenchPos;
            }
            else
            {
                Vector3 secBenchPos = new Vector3(0.00661411509f, 0.17933172f, -0.409160197f);
                bench2.localPosition = secBenchPos;
            }
        }
    }

    
}
