using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {
    public GameObject[] potions;
    public int maxHP = 10;
    public int currentLevel = 1;
    public int currentHP;
    public int currentExp;
    public int currentPotions;
    public int currentGold;
    public int levelUpReq;
    private int maxPotions;

	void Start () {
        currentLevel = 1;
        currentHP = maxHP;
        currentExp = 0;
        maxPotions = potions.Length;
        currentPotions = 0;
        levelUpReq = 10;
	}
	
	void Update () {
	
	}

    public void IncreaseExp(int exp){
        currentExp += exp;
        RecalculateLevel();
    }

    void RecalculateLevel(){
        int tempLevel = (int)decimal.Round((currentExp / levelUpReq) + 1, 0);
        if(tempLevel > currentLevel){
            //Do something to indicate level up!
            currentLevel = tempLevel;
        }
    }

    public void DamagePlayer(int damage){
        currentHP -= damage;
        Debug.Log("Took Damage");
        UsePotion();
        if(currentHP <= 0){
            gameObject.GetComponent<Player>().Die();
        }
    }

    void UsePotion(){
        if(currentPotions > 0){
            if((currentHP + currentPotions) <= maxHP){
                currentHP += currentPotions;
                currentPotions = 0;
            } else {
                currentHP = maxHP;
                currentPotions = currentPotions + currentHP - maxHP; 
            }
            ShowHidePotionIcons();
        }
    }

    public void IncreasePotions(int potion){
        currentPotions += potion;
        if(currentPotions > maxPotions){
            currentPotions = maxPotions;
            ShowHidePotionIcons();
        }
    }

    void ShowHidePotionIcons(){
    //enable potions
        for(int i = 0; i < currentPotions; i++){
            if(!potions[i].activeSelf){
                potions[i].SetActive(true);
            }
        }
    //disable potions
        for(int i = currentPotions; i < maxPotions; i++){
            if(potions[i].activeSelf){
                potions[i].SetActive(false);
            }            
        }
    }

    public void IncreaseGold(int gold){
        currentGold += gold;
    }
}
