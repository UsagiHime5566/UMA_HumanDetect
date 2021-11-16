using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HumanMove : MonoBehaviour
{
    int move_x = CustomGenerator.instance.move_x;
    float rand_z = CustomGenerator.instance.rand_z;
    float time = CustomGenerator.instance.time;

    public void Init(){
        int rand = Random.Range(0, 100);
        if(rand > 50){
            transform.position = new Vector3(CustomGenerator.instance.RightEdge.position.x, transform.position.y, Random.Range(-rand_z, rand_z));
            DoMoving(MoveDirection.ToLeft, time);
        } else {
            transform.position = new Vector3(CustomGenerator.instance.LeftEdge.position.x, transform.position.y, Random.Range(-rand_z, rand_z));
            DoMoving(MoveDirection.ToRight, time);
        }
        
    }

    public void DoMoving(MoveDirection direction, float time){
        if(direction == MoveDirection.ToRight){
            var toPos = transform.position + new Vector3(move_x, 0, 0);
            transform.LookAt(toPos);
            transform.DOMove(toPos, time).SetEase(Ease.Linear).OnComplete(() => {
                if(gameObject){
                    CustomGenerator.instance.humans.Remove(gameObject);
                    Destroy(gameObject, 1);
                }
            });
        }
        if(direction == MoveDirection.ToLeft){
            var toPos = transform.position + new Vector3(-move_x, 0, 0);
            transform.LookAt(toPos);
            transform.DOMove(toPos, time).SetEase(Ease.Linear).OnComplete(() => {
                if(gameObject){
                    CustomGenerator.instance.humans.Remove(gameObject);
                    Destroy(gameObject, 1);
                }
            });
        }
    }
}

public enum MoveDirection
{
    ToRight = 10,
    ToLeft = 20,
}
