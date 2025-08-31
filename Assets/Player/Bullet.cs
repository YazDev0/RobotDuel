using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        //  œ„Ì— ⁄‰œ «·«’ÿœ«„
        Destroy(gameObject);
    }
}
