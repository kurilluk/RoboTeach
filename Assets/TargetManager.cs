using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using Random = System.Random;

public class TargetManager : MonoBehaviour
{
    [SerializeField] UnityEvent VictoryEvent;
    [SerializeField] UnityEvent ScoreTeller;

    public Vector3[] randomPositions;
    public int acturalTargetIndex = 0;
    public List<int> possibleIndices = new List<int>();   //  Declare list
    public int numberOfLevels = 5;

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
        randomPositions = positions;

        //Instantiate(targetGO, randomPositions[0], Quaternion.identity);
        //Instantiate(targetGO, randomPositions[1], Quaternion.identity);

        //!!!! Watch out this spawns recursively more targets !!!!!
        for (int n = 0; n < randomPositions.Length; n++)    //  Populate list
        {
            possibleIndices.Add(n);
        }

        MoveMe(randomPositions[SelectRandom()]);
        //possibleIndices.RemoveAt(0);
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
        if (acturalTargetIndex >= numberOfLevels)
        {
            VictoryEvent.Invoke();
            Debug.Log("You've won!");
            ResetIndex();
        }
        else
        {
            MoveMe(randomPositions[SelectRandom()]);
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

    public int SelectRandom ()
    {
        int index = UnityEngine.Random.Range(0, possibleIndices.Count - 1);    //  Pick random element from the list
        int i = possibleIndices[index];    //  i = the number that was randomly picked
        possibleIndices.RemoveAt(index);   //  Remove chosen element
        return i;

    }


    //-------------------- Random Shuffle ------------------------

    //public static class ListExtensions
    //{
    //    private static readonly Random Random = new Random();

    //    public static void Shuffle<T>(this IList<T> list)
    //    {
    //        int n = list.Count;
    //        while (n > 1)
    //        {
    //            n--;
    //            int k = Random.Next(n + 1);
    //            T value = list[k];
    //            list[k] = list[n];
    //            list[n] = value;
    //        }
    //    }
    //}
}
