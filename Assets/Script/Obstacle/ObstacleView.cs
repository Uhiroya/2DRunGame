//using System.Collections;
//using System.Collections.Generic;
//using UniRx;
//using UnityEngine;

//public class ObstacleView : MonoBehaviour
//{
//    public readonly ReactiveProperty<Collider2D> IsHit = new(default);

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.tag == "Player")
//        {
//            IsHit.Value = other;
//        }
//    }

//    public void OnDestroy()
//    {
//        IsHit.Dispose();
//    }
//}
