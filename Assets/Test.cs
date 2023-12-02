using AE_Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        EventCenter.AddEventListener<Collider>("BeAttacked", (res) =>
        {
            if (res != null)
            {
                print(res.name);
            }
        });
    }
}
