using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour {
    private Image rageBar;
    private Image hp;
    private Image energy;
    private Image Icon1Black;
    private Image Icon2Black;
    private Image Icon3Black;
    private Image Icon4Black;
    private Image Icon5Black;
    private float timer1;
    private float timer2;
    private float timer3;
    private float coolDown1;
    private float coolDown2;
    private float coolDown3;
    private float coolDown4;
    private float coolDown5;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Energy")
                energy = transform.GetChild(i).GetComponent<Image>();
            else if (transform.GetChild(i).name == "HP")
                hp = transform.GetChild(i).GetComponent<Image>();
            else if (transform.GetChild(i).name == "Icon 1")
                Icon1Black = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
            else if (transform.GetChild(i).name == "Icon 2")
                Icon2Black = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
            else if (transform.GetChild(i).name == "Icon 3")
                Icon3Black = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
            else if (transform.GetChild(i).name == "Icon 4")
                Icon4Black = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
            else if (transform.GetChild(i).name == "Icon 5")
                Icon5Black = transform.GetChild(i).GetChild(0).GetChild(0).GetComponent<Image>();
            else if (transform.GetChild(i).name == "Rage Bar")
                rageBar = transform.GetChild(i).GetComponent<Image>();
        }
	}


    public void RageBarFill(float value)
    {
        rageBar.fillAmount = value;
        rageBar.color = new Color(1, 1 - value, 0);
        if (value == 1)
            Icon3Black.fillAmount = 0;
        else
            Icon3Black.fillAmount = 1;
    }
    public void HpChange(float value)
    {
        hp.fillAmount = value;
    }

    public void EnergyCHange(float value)
    {
        energy.fillAmount = value;
    }

    public void SetImage(Sprite image, int n)
    {
        switch (n)
        {
            case 1: Icon1Black.transform.parent.GetComponent<Image>().sprite = image; break;
            case 2: Icon2Black.transform.parent.GetComponent<Image>().sprite = image; break;
            case 3: Icon3Black.transform.parent.GetComponent<Image>().sprite = image; break;
            case 4: Icon4Black.transform.parent.GetComponent<Image>().sprite = image; break;
            case 5: Icon5Black.transform.parent.GetComponent<Image>().sprite = image; break;
            
        }
    }

    public void AbilityStarted(int n, float time)
    {
        switch (n)
        {
            case 1: Icon1Black.fillAmount = 1; coolDown1 = time; timer1 = 0; break;
            case 2: Icon2Black.fillAmount = 1; coolDown2 = time; timer2 = 0; break;
            case 3: Icon3Black.fillAmount = 1; coolDown3 = time;timer3 = 0; break;
            case 4: Icon4Black.fillAmount = 1; coolDown3 = time; timer3 = 0; break;
            case 5: Icon5Black.fillAmount = 1; coolDown3 = time; timer3 = 0; break;
        }
    }
    public void SetBlack(int n,float fill)
    {

        switch (n)
        {
            case 1: Icon1Black.fillAmount = fill;  break;
            case 2: Icon2Black.fillAmount = fill;  break;
            case 3: Icon3Black.fillAmount = fill;  break;
            case 4: Icon4Black.fillAmount = fill;   break;
            case 5: Icon5Black.fillAmount = fill;  break;
        }
    }
    void Update()
    {
        if (timer1 < coolDown1)
        {
            Icon1Black.fillAmount = (coolDown1 - timer1) / coolDown1;
            timer1 += Time.deltaTime;
        }
        else if (timer1 > coolDown1)
        {
            timer1 = coolDown1;
            Icon1Black.fillAmount = 0;
        }
        if (timer2 < coolDown2)
        {
            Icon2Black.fillAmount = (coolDown2 - timer2) / coolDown2;
            timer2 += Time.deltaTime;
        }
        else if (timer2 > coolDown2)
        {
            timer2 = coolDown2;
            Icon2Black.fillAmount = 0;
        }
        if(timer3 < coolDown3)
        {
            Icon3Black.fillAmount = (coolDown3 - timer3) / coolDown3;
            timer3 += Time.deltaTime;
        }
        else if (timer3 > coolDown3)
        {
            Icon3Black.fillAmount = 0;
            timer3 = coolDown3;
        }
    }


}
