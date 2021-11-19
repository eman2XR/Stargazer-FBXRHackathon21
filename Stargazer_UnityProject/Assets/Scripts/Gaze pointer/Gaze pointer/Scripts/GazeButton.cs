
//component to make a button interactable with the gazePointer
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class GazeButton : MonoBehaviour
{
    [SerializeField] Transform buttonParent; //optional transform used for bringing the button forward and back on hover
    [SerializeField] float movementDistancee = 0.05f; //the ammount the button will move on hover
    [SerializeField] string axis = "z"; //the axis to move on 
    [SerializeField] float clickTime = 2; //the ammount of time in seconds the pointer needs to be hovered before the button gets clicked
    [SerializeField] Transform loader; //optional transform for showing a loading progress left to right
    [SerializeField] Transform circleLoader; //optional transform for showing a circle fill loading

    [Space(10)] //events to trigger 
    public UnityEvent onHoverEnter;
    public UnityEvent onHoverStay;
    public UnityEvent onHoverExit;
    public UnityEvent onClick;
    public UnityEvent onStateChanged; //an event for when the button is pressed again (a flip switch)

    public bool isInspectionButton;
    public bool inspected;
    bool isHovered;
    float timer;
    private void Start()
    {
        if (buttonParent == null)
            buttonParent = this.transform;

        //if a loader transform is used, scale it down to 0 on the x
        if (loader)
            loader.localScale = new Vector3(0, 1, 1);

        //if a circle loader transform is used, scale it down to 0 on all axis
        if (circleLoader)
            circleLoader.localScale = new Vector3(0, 0, 0);
    }

    public void OnHoverEnter()
    {
        onHoverEnter.Invoke();

        isHovered = true;
        StartCoroutine(CheckGazeTime());

        if (axis == "x")
            this.transform.localPosition += new Vector3(movementDistancee, 0, 0);
        else if (axis == "-x")
            this.transform.localPosition -= new Vector3(movementDistancee, 0, 0);
        else if (axis == "y")
            this.transform.localPosition += new Vector3(0, movementDistancee, 0);
        else if (axis == "-y")
            this.transform.localPosition -= new Vector3(0, movementDistancee, 0);
        else if (axis == "z")
            this.transform.localPosition += new Vector3(0, 0, movementDistancee);
        else if (axis == "-z")
            this.transform.localPosition -= new Vector3(0, 0, movementDistancee);
        // print("ented");
    }

    public void OnHoverStay()
    {
        onHoverStay.Invoke();
    }

    public void OnHoverExit()
    {
        onHoverExit.Invoke();

        isHovered = false;

        if(!isInspectionButton)
            timer = 0;

        if (axis == "x")
            this.transform.localPosition -= new Vector3(movementDistancee, 0, 0);
        else if (axis == "-x")
            this.transform.localPosition += new Vector3(movementDistancee, 0, 0);
        else if (axis == "y")
            this.transform.localPosition -= new Vector3(0, movementDistancee, 0);
        else if (axis == "-y")
            this.transform.localPosition += new Vector3(0, movementDistancee, 0);
        else if (axis == "z")
            this.transform.localPosition -= new Vector3(0, 0, movementDistancee);
        else if (axis == "-z")
            this.transform.localPosition += new Vector3(0, 0, movementDistancee);
        //print("exit");

        //scale down the loading bar
        if (loader)
            loader.transform.localScale = new Vector3(0, 1, 1);

        //scale down the circle loading bar
        if (circleLoader && !isInspectionButton)
            circleLoader.transform.localScale = new Vector3(0, 0, 0);
    }

    public void OnClick()
    {
        onClick.Invoke();
        Pointer.instance.OnClickButton();
        if(this.gameObject.activeSelf && isInspectionButton)
            StartCoroutine(ClickDelay());
    }

    IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(2f);
        this.transform.GetChild(0).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void OnStateChanged()
    {
        onStateChanged.Invoke();
        Pointer.instance.OnClickButton();
    }

    IEnumerator CheckGazeTime()
    {
        if(!isInspectionButton)
            timer = 0;
        while (isHovered)
        {
            timer += Time.deltaTime;

            //scale up the loding bar
            if (loader)
                loader.transform.localScale = new Vector3(Mathf.InverseLerp(0, clickTime, timer), 1, 1);
            //loader.transform.localScale = new Vector3(NormalizedFloat(timer, 0, timer), 1, 1);

            //scale up the circle loding bar
            if (circleLoader)
                circleLoader.transform.localScale = new Vector3(Mathf.InverseLerp(0, clickTime, timer), Mathf.InverseLerp(0, clickTime, timer), Mathf.InverseLerp(0, clickTime, timer));

            if (timer > clickTime)
            {
                OnClick();
                inspected = true;
                yield break;
            }
            yield return null;
        }
    }

    float NormalizedFloat(float inputValue, float min, float max)
    {
        //Calculate the normalized float;
        float normalizedFloat = (inputValue - min) / (max - min);
        //Clamp the "i" float between "min" value and "max" value
        float i = Mathf.Clamp(inputValue, min, max);
        //Clamp the normalized float between 0 and 1
        return normalizedFloat = Mathf.Clamp(normalizedFloat, 0, 1);
    }
}
