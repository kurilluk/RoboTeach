using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewAnimation : MonoBehaviour
{
    [SerializeField]private Transform m_Arm;

    [SerializeField] private float m_Min;
    [SerializeField] private float m_Max;
    [SerializeField] private float m_Rotation;

    [SerializeField] private AnimationCurve _function;

    [SerializeField] private bool m_Preview;

    private float stepSize = 0.3f;
    static float t_actual = 0.0f;
    private float direction = 1; // can be -1 and 1
   // private bool m_Preview = false;
    
     private void Awake()
    {
        //m_Arm = transform; //.GetChild(0);
        m_Rotation = m_Arm.transform.eulerAngles.y;

        t_actual = Mathf.InverseLerp(m_Min,m_Max, m_Rotation);
    }

    IEnumerator RotateAnim()
    {
        m_Preview = true;

        while (m_Preview)
        {
           t_actual += stepSize * direction * Time.deltaTime;
            //t_actual = Mathf.InverseLerp(m_Min,m_Max, m_Rotation + (stepSize * direction * Time.deltaTime));

            if (t_actual > 1.0f)
            {
                t_actual = 1.0f;
                direction = -1;
                //m_Preview = false;
            }
            else if (t_actual < 0.0f)
            {
                t_actual = 0.0f;
                direction = 1;
               //m_Preview = false;
            }

            m_Rotation = Mathf.Lerp(m_Min, m_Max, _function.Evaluate(t_actual));
            m_Arm.rotation = Quaternion.Euler(0, m_Rotation, 0);

            yield return null;
        }

        m_Arm.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("RotateAnim");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            //print("You clicked the button!");
            StartCoroutine("RotateAnim");
        }
    }

}
