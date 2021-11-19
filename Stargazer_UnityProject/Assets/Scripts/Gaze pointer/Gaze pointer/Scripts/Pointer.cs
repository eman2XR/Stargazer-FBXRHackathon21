using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointer : MonoBehaviour {

    public static Pointer instance; //singleton instance for easy access

    [SerializeField] Transform reticle; //the tip of the pointer
    
    [SerializeField] AudioSource onHoverAudioSource;
    [SerializeField] AudioSource onClickAudio;

    [SerializeField] Image crossImage;
    [SerializeField] Color onHoverColor; //color to change to when hovering a button
    Color initialColor;

    bool busy;
    bool exitedButton;
    GazeButton buttonUnderGaze;
    Vector3 initPointerPos;
   
    private void Awake()
    {
        instance = this;
    }

    //triggered by each button
    public void OnClickButton()
    {
        onClickAudio.Play();
    }

    private void Start()
    {
        //get refferences
        initPointerPos = reticle.localPosition;
        initialColor = crossImage.color;
    }

    void Update () {

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "gazeUI" || hit.collider.gameObject.tag == "button")
            { crossImage.color = initialColor; reticle.transform.position = hit.point; }
            else crossImage.color = Color.clear;

            if (hit.collider.gameObject.tag == "sky")
            {
                if (!busy)
                {
                    //print("enter");
                    busy = true;
                    exitedButton = false;

                    if(hit.collider.gameObject.tag == "button")
                        crossImage.color = onHoverColor;

                    buttonUnderGaze = hit.collider.GetComponent<GazeButton>();
                    StopAllCoroutines();
                    //StartCoroutine(GazingAtButton());

                    //rotation anim
                    //StartCoroutine(RotateGazeLoader());

                    //audio
                    onHoverAudioSource.Play();
                }
                //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                reticle.transform.position = hit.point;
            }

            else
            {
                busy = false;

                if (buttonUnderGaze != null)
                {
                    if (!exitedButton)
                        ExitedButton();
                }
            }
        }
        else
        {
            busy = false;

            if (buttonUnderGaze != null)
            {
                if (!exitedButton)
                    ExitedButton();
            }
        }
    }

    //check is button is being looked at for a period of time
    IEnumerator GazingAtButton()
    {
        //print("started gaze");
        buttonUnderGaze.OnHoverEnter();
        while (busy)
        {
            //print("gazing at button");
            buttonUnderGaze.OnHoverStay();
            yield return null;
        }
    }

    //reticle rotation
    IEnumerator RotateGazeLoader()
    {
        if (busy)
        {
            StartCoroutine(OneRotationGazeLoader(1f));
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(OneRotationGazeLoader(1f));
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(OneRotationGazeLoader(1f));
        }
    }
    IEnumerator OneRotationGazeLoader(float timeToMove)
    {
        var t = 0f;
        while (t < 1 && busy)
        {
            t += Time.deltaTime / timeToMove;

            reticle.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -180), t);
            yield return null;
        }
    }

    void ExitedButton()
    {
        StopAllCoroutines();

       // print("exited button");
        buttonUnderGaze.OnHoverExit();
        exitedButton = true;
        //mat.color = initColor;
        reticle.localPosition = initPointerPos;

        crossImage.color = Color.white;

        //hide the reticle
        //reticleRend.enabled = false;
        //reticleRend1.enabled = false;

        //audio
        onHoverAudioSource.Stop();
    }
}
