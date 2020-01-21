using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public static class ControllerManager
{
    // -- Axis

    public static float MainHorizontal(int controllerNumber)
    {
        float r = 0.0f;
        r += Input.GetAxis("J_MainHorizontal" + controllerNumber);
        r += Input.GetAxis("K_MainHorizontal" + controllerNumber);
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static float MainVertical(int controllerNumber)
    {
        float r = 0.0f;
        r += Input.GetAxis("J_MainVertical" + controllerNumber);
        r += Input.GetAxis("K_MainVertical" + controllerNumber);
        return Mathf.Clamp(r, -1.0f, 1.0f);
    }

    public static Vector3 MainJoystick(int controllerNumber)
    {
        return new Vector3(MainHorizontal(controllerNumber), 0, MainVertical(controllerNumber));
    }

    // -- Buttons

    public static bool EatButton(int controllerNumber)
    {
        return Input.GetButtonDown("Eat_Button" + controllerNumber);
    }
    public static bool SprintButton(int controllerNumber)
    {
        //if(Input.GetButton("Sprint_Button" + controllerNumber))
        //Debug.Log(controllerNumber + "Sprint");
        return Input.GetButton("Sprint_Button" + controllerNumber);
    }
    public static bool TransformButton(int controllerNumber)
    {
        //Debug.Log(controllerNumber + "Transform");
        return Input.GetButtonDown("Transform_Button" + controllerNumber);
    }

    public static bool TransormButtonReleased(int controllerNumber)
    {
        return Input.GetButtonUp("Transform_Button" + controllerNumber);
    }

    public static float SelectTransformation1(int controllerNumber)
    {
        return Input.GetAxis("TransformSelect_Button1" + controllerNumber);
    }

    public static float SelectTransformation2(int controllerNumber)
    {
        return Input.GetAxis("TransformSelect_Button2" + controllerNumber);
    }
    //public static bool EatButton()
    //{
    //    return Input.GetButtonDown("Eat_Button");
    //}
}