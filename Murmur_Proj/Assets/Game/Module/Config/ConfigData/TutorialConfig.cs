using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStep
{
    public string on_start;
    public string on_end;
    public string dialog;
    public float camera_size;
    public Vector3 camera_pos;
    public int enter_delay;
    public string target;
    public float target_size;
    public int exit_delay;
    public bool save;
}

public class TutorialConfig
{
    public string id;
    public string trigger;
    public string string_trigger_param;
    public int int_trigger_param;
    public string check;
    public string string_check_param;
    public int int_check_param;
    public List<TutorialStep> steps; 
}
