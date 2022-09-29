using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;

public class TargetManager : MonoBehaviour
{
    [SerializeField] UnityEvent VictoryEvent;
    [SerializeField] UnityEvent ScoreTeller;

    public Vector3[] randomPositions;
    public int acturalTargetIndex = 0;

    public GameObject targetGO;

    public Action ShowScore;

    private void Awake()
    {
        InitializeMe(randomPositions);
    }
    private void Start()
    {
        EnableMe();
    }


    public void InitializeMe(Vector3[] positions)
    {
        //Debug.Log("Initialized the Target.");
        //randomPositions = positions;
        for (int i = 0; i < randomPositions.Length; i++)
        {
            float radius = 0.5f;
            
            randomPositions[i] = new Vector3(UnityEngine.Random.Range(this.transform.position.x - radius, this.transform.position.x + radius), UnityEngine.Random.Range(this.transform.position.y - radius, this.transform.position.y + radius), UnityEngine.Random.Range(this.transform.position.z - radius, this.transform.position.z + radius));
        }
        ResetIndex();
        MoveMe(randomPositions[0]);
    }

    private void ResetIndex()
    {
        //Debug.Log("The Index has been reset.");
        acturalTargetIndex = 0;
    }

    private void EnableMe()
    {
        //Debug.Log("Target just got Enabled.");
        targetGO.SetActive(true);
        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        for (float f = 0; f <= 1; f += 0.05f)
        {
            StopCoroutine(Shrink());
            //Debug.Log("Growing animation frame: "+ f.ToString());
            targetGO.transform.localScale = new Vector3(1f, 1f, 1f) * f;
            yield return new WaitForSeconds(0.05f);
        }
    }
    IEnumerator Shrink()
    {
        StopCoroutine(Grow());
        for (float f = 1; f >= 0.000001; f -= 0.05f)
        {
            //Debug.Log("Shrinking animation frame: " + f.ToString());
            targetGO.transform.localScale = new Vector3(1f, 1f, 1f) * f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void MoveMe(Vector3 moveHere)
    {
        //Debug.Log("Target moved to a new position: " + moveHere.ToString());
        this.transform.position = moveHere;
        // TODO: Add animation using LERP
    }

    private void TargetHit()
    {
        StopCoroutine(Grow());
        StartCoroutine(Shrink());
        ScoreTeller.Invoke();

    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("You've hit the target!");
        TargetHit();
        acturalTargetIndex++;
        if (acturalTargetIndex >= randomPositions.Length)
        {
            VictoryEvent.Invoke();
            Debug.Log("You've won!");
            ResetIndex();
        }
        else
        {
            MoveMe(randomPositions[acturalTargetIndex]);
            EnableMe();

        }
    }

    public void OnForfait(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Space pressed indeed!");
            acturalTargetIndex++;
            MoveMe(randomPositions[acturalTargetIndex]);
            if (acturalTargetIndex >= randomPositions.Length)
            {
                VictoryEvent.Invoke();
                Debug.Log("Forfaited.");
                ResetIndex();
            }
        }

    }
}
