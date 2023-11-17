using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PlayerView 
{
    [SerializeField] Animator _animator;
    public void OnWaiting()
    => _animator.SetTrigger("Waiting");
    public void OnWalk()
        => _animator.SetTrigger("Walking");
    public void OnDead()
    => _animator.SetTrigger("Dead");
}
