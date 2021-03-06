using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour {
    protected AudioSource audioSource;
    protected CharacterAttributesClient charStatsClient;
    public AudioClip[] attackSounds;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip[] abilitySounds;
    public AudioClip deathSound;
    public AudioClip doubleJumpSound;
    public AudioClip onGroundSound;
    public AudioClip onWallSound;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        charStatsClient = GetComponent<CharacterAttributesClient>();
	}

    public virtual void HandState(string value)
    {
        if (value == "2")
        {
            try
            {
                if (charStatsClient.attackNumber <= attackSounds.Length)
                {
                    audioSource.clip = attackSounds[charStatsClient.attackNumber];
                    audioSource.Play();
                }
            }
            catch
            {

            }
        }
    }

    public virtual void BodyState(string value)
    {
        if (value == "2")
        {
            // Move Sound But Be carful cuz It Makes Move on Air!
        }
        else if (value == "3")
        {
            audioSource.clip = dashSound;
            audioSource.Play();
        }
    }


    public void LowEnergy()
    {
        // Low energy Sound
    }



    public void Root(string value)
    {
        // Root Sound
    }
    public virtual void HeadState(string value)
    {
        // Stun Sound
    }
    public virtual void FeetState(string value)
    {
        if (value == "1")
        {
            audioSource.clip = onGroundSound;
            audioSource.Play();
        }
        else if (value == "3")
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
        }
        else if (value == "5")
        {
            audioSource.clip = doubleJumpSound;
            audioSource.Play();
        }
        else if (value == "6")
        {
            if (onWallSound != null)
            {
                audioSource.clip = onWallSound;
                audioSource.Play();
            }
        }
        
    }

    public void DieSound()
    {
        audioSource.clip = deathSound;
        audioSource.Play();
    }
    public virtual void AbilityState(string value)
    {
        if (value == "1")
        {
            audioSource.clip = abilitySounds[0];
            audioSource.Play();
        }
        else if(value == "2")
        {
            // Ability 1 Finish
        }
        else if (value == "3")
        {
            audioSource.clip = abilitySounds[1];
            audioSource.Play();
        }
        else if (value == "4")
        {
            // Ability 2 Finish
        }
    }

    /*
 * public enum EHeadState { Conscious = 1, Stunned = 2 };
public enum EBodyState { Standing = 1,Moving = 2 , Dashing = 3};
public enum EHandState { Idle = 1, Attacking = 2, Casting = 3, Channeling = 4};
public enum EFeetState { Onground = 1, Falling = 2, Jumping = 3, NoGravity = 4 , DoubleJumping = 5,OnWall = 6,WallJumping = 8, Root = 9};
public enum EAttackMode { Ranged = 1, Melee = 2 };


public enum EAbility { Ability1Start = 1, Ability1Finish = 2, Ability2Start=3,Ability2Finish =4,Ability3Start = 5,Ability3Finish = 6};
 * */
}
