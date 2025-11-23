using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthSystem : MonoBehaviour
{
    public int MaxHP = 100;
    public int MaxStamina = 100;
    int currentStamina; 
    int currentHP;
    int HitCount = 0;
    void awake()
    {
        currentHP = MaxHP;
        currentStamina = MaxStamina;
    }
    public void HPUpdate(bool GotHit, int RegenAmount)
    {
        if (GotHit == true)
        {
            if (HitCount == 0)
            {
                currentHP = MaxHP * 1 / 3;
                HitCount++;
            }
            else if (HitCount == 1)
            {
                currentHP = MaxHP * 2 / 3;
                HitCount++;
            }
            else if (HitCount == 2)
            {
                currentHP = MaxHP * 3 / 3;
                //transition into the gameover screen
            }
        }
        else
        {
            if (RegenAmount == 1)
            {
                if (HitCount == 1)
                {
                    currentHP = MaxHP;
                }
                else
                {
                    currentHP = MaxHP * 1 / 3;
                }
            }
            else
            {
                currentHP = MaxHP;
            }
        }
    }
}
