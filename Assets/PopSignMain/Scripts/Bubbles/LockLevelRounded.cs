﻿using UnityEngine;
using System.Collections;

public class LockLevelRounded : MonoBehaviour
{
    public static LockLevelRounded Instance;
    Vector2 dir;
    Vector2 ballPos;
    float angle;
    Quaternion newRot;
  	// Use this for initialization
  	void Start ()
    {
          Instance = this;
          newRot = Quaternion.identity;
  	}

    public void Rotate( Vector2 _dir, Vector2 _ballPos )
    {
        _dir = mainscript.Instance.boxCatapult.GetComponent<Grid>().transform.position;
        angle = Vector2.Angle( _dir-_ballPos, _ballPos - (Vector2)transform.position )/4f;
        if( transform.position.x < _ballPos.x ) angle *= -1;
        // newRot = transform.rotation*Quaternion.AngleAxis( angle, Vector3.back );
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot( SoundBase.Instance.kreakWheel );
    }

    void Update()
    {
        if( transform.rotation != newRot )
            transform.rotation = Quaternion.Lerp( transform.rotation, newRot, Time.deltaTime );
    }

}
