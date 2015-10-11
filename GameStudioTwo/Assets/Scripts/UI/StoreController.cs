﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum Equipment { Item_Handle = 0, Item_BasicEngine, Item_Spike, Item_Flipper, Item_Booster, Item_MetalShield, Item_CircularSaw, Item_Hammer, Item_PlasmaShield, EMPTY };

public enum Socket { EMPTY = 0, Socket_Left, Socket_Right, Socket_Front, Socket_Back, Socket_Top };

public enum StoreState {STATE_GARAGE, STATE_MODEL, STATE_ITEM, STATE_STORE};


public class StoreController : MonoBehaviour {

	public GameObject player;
	public GameObject storeUI;
	public GameObject uiPlayerModels;
	GameObject selectedModel;

	private Vector3 rgbColor = new Vector3(200f/255f, 50f/255f, 50f/255f);
	private Color sliderColour;
	public Color selectedItemColour = new Color(255f/255f, 200f/255f, 0f/255f);

	public StoreState current_state = StoreState.STATE_MODEL;

    persistentStats playerChoice;
    
	int storeItemPos = 0;

	//bool hasSpike = false;
    bool colourChange = false;

	//
	// Selected items on the model
	//
	private static int MAX_SOCKETS = 5;
	public static int TOTAL_ITEMS = 9;
	private Equipment[] itemSocketArray = new Equipment[MAX_SOCKETS];
	public Equipment selectedEquipment = Equipment.EMPTY;
	private Socket selectedSocket = Socket.EMPTY;

	public Equipment[] AvailableItems = new Equipment[TOTAL_ITEMS];

	public int DOLLADOLLABILLSYALL;
	public Text moneyText;


	// Use this for initialization
	void Start () {
        Time.timeScale = 1;

        if (GameObject.FindGameObjectWithTag ("Persistent Stats").GetComponent<persistentStats> () != null) 
		{
			playerChoice = GameObject.FindGameObjectWithTag ("Persistent Stats").GetComponent<persistentStats> ();
		}


		// equipment initializers=
		for (int i = 0; i < MAX_SOCKETS; i++)
		{
			itemSocketArray[i] = Equipment.EMPTY;
		}


		if (playerChoice != null){
			for (int i = 0; i < MAX_SOCKETS; i++)
			{
				playerChoice.playerItems[i] = itemSocketArray[i];
			}

			//get the bought items
			for (int i = 0; i < TOTAL_ITEMS; i++)
			{
				AvailableItems[i] = playerChoice.boughtItems[i];
			}

			DOLLADOLLABILLSYALL = playerChoice.playerMoney;

		}

		sliderColour = new Color(rgbColor.x, rgbColor.y, rgbColor.z);
	}


	// Update is called once per frame
	void Update () {

            if (Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (current_state == StoreState.STATE_MODEL)
                {
                    HandleModelSelection(hit);
                }
                else if (current_state == StoreState.STATE_ITEM)
                {
                    HandleItemSelection(hit);
                    HandleSocketSelection(hit);
                }

				if (selectedEquipment != Equipment.EMPTY){
					foreach (Transform child in hit.transform)
                    {
                        child.GetComponent<Renderer>().material.color = selectedItemColour;
                        child.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(63.75f / 255f, 50f / 255f, 0f / 255f));
                    }
					if (selectedSocket != Socket.EMPTY){
						FillItemSocketArray();
					}

				}

				if (playerChoice != null){
					for (int i = 0; i < MAX_SOCKETS; i++)
					{
						playerChoice.playerItems[i] = itemSocketArray[i];
					}
				}
			}
		}


		HandleBoughtItems();

		moneyText.text = "Money: " + DOLLADOLLABILLSYALL;


        if (colourChange)
		{
			sliderColour = new Color(rgbColor.x, rgbColor.y, rgbColor.z);
			GameObject.FindGameObjectWithTag("Player Model").GetComponent<Renderer>().material.color = sliderColour;
			GameObject.FindGameObjectWithTag("Player Engine").GetComponent<Renderer>().material.color = sliderColour;
			colourChange = false;
        }

        if (playerChoice != null) 
		{
			playerChoice.playerColor = sliderColour;
		}
	}


    void HandleModelSelection(RaycastHit hit)
    {
        switch (hit.transform.tag)
        {
            case "LawnMowerRed":
            case "LawnMowerGreen":
            case "LawnMowerBlue":
                player.GetComponent<Transform>().position = new Vector3(7.5f, 2.35f, 7.4f);

                current_state = StoreState.STATE_ITEM;
                GetComponent<Animator>().SetTrigger("toItemSelection");
                break;
        }
    }


        void HandleItemSelection(RaycastHit hit) {
		selectedSocket = Socket.EMPTY;

		switch (hit.transform.tag){
			
			case "Phone_Model":
				if (current_state == StoreState.STATE_ITEM)
				{
					GetComponent<Animator>().SetTrigger("toStoreSelection");
				    current_state = StoreState.STATE_STORE;
				}
				break;

			case "Item_Handle":
				selectedEquipment = Equipment.Item_Handle;
				break;

            case "Item_BasicEngine":
                selectedEquipment = Equipment.Item_BasicEngine;
                break;

            case "Item_Spike":
				selectedEquipment = Equipment.Item_Spike;
				break;
				
			case "Item_Flipper":
				selectedEquipment = Equipment.Item_Flipper;
				break;

			case "Item_Booster":
				selectedEquipment = Equipment.Item_Booster;
				break;

            case "Item_MetalShield":
                selectedEquipment = Equipment.Item_MetalShield;
                break;

            case "Item_CircularSaw":
                selectedEquipment = Equipment.Item_CircularSaw;
                break;

            case "Item_Hammer":
                selectedEquipment = Equipment.Item_Hammer;
                break;

            case "Item_PlasmaShield":
                selectedEquipment = Equipment.Item_PlasmaShield;
                break;
        }
    }

    void HandleSocketSelection(RaycastHit hit)
    {
        switch (hit.transform.tag)
        {
            case "Socket_Left":
                selectedSocket = Socket.Socket_Left;
                break;

            case "Socket_Right":
                selectedSocket = Socket.Socket_Right;
                break;

            case "Socket_Front":
                selectedSocket = Socket.Socket_Front;
                break;

            case "Socket_Back":
                selectedSocket = Socket.Socket_Back;
                break;

            case "Socket_Top":
                selectedSocket = Socket.Socket_Top;
                break;
        }


        if (selectedEquipment == Equipment.Item_Handle && (selectedSocket != Socket.Socket_Back && selectedSocket != Socket.EMPTY))
        {
            selectedSocket = Socket.EMPTY;
        }
        if (selectedEquipment == Equipment.Item_Spike && selectedSocket == Socket.Socket_Top)
        {
            selectedSocket = Socket.EMPTY;
        }
        if (selectedEquipment == Equipment.Item_Flipper && (selectedSocket == Socket.Socket_Top || selectedSocket == Socket.Socket_Back))
        {
            selectedSocket = Socket.EMPTY;
        }

        if (selectedEquipment == Equipment.Item_Booster && (selectedSocket == Socket.Socket_Top || selectedSocket == Socket.Socket_Front))
        {
            selectedSocket = Socket.EMPTY;
        }

        if (current_state != StoreState.STATE_ITEM)
        {
            selectedEquipment = Equipment.EMPTY;
            selectedSocket = Socket.EMPTY;
        }


    }





        // instantiate at sockets change rotation based on what socket. make this a separate script on the vehicle prefab

        void FillItemSocketArray(){
		switch (selectedSocket) {
			case Socket.Socket_Left:
				itemSocketArray[0] = selectedEquipment;
				break;

			case Socket.Socket_Right:
				itemSocketArray[1] = selectedEquipment;
				break;

			case Socket.Socket_Front:
				itemSocketArray[2] = selectedEquipment;
				break;

			case Socket.Socket_Back:
				itemSocketArray[3] = selectedEquipment;
				break;

			case Socket.Socket_Top:
				itemSocketArray[4] = selectedEquipment;
				break;

			default:
				selectedEquipment = Equipment.EMPTY;
				selectedSocket = Socket.EMPTY;
				return;

		}
		selectedEquipment = Equipment.EMPTY;
		selectedSocket = Socket.EMPTY;

		player.GetComponent<SocketEquipment>().SocketItems(itemSocketArray);

	}


	public void startLevel(string levelName){
        if (playerChoice != null)
		{
			for (int i = 0; i < MAX_SOCKETS; i++)
			{
				playerChoice.playerItems[i] = itemSocketArray[i];
			}
			for (int i = 0; i < TOTAL_ITEMS; i++)
			{
				playerChoice.boughtItems[i] = AvailableItems[i];
			}
		}
        
        Application.LoadLevel(levelName);
	}


	public void HandleBoughtItems()
	{
		for (int i = 0; i < TOTAL_ITEMS; i++) 
		{
			switch (AvailableItems [i]) 
			{
				case Equipment.EMPTY:
					break;

				case Equipment.Item_Handle:
					break;


			}
		}
	}
	

    public void changeR(float slider)
    {
        rgbColor.x = slider / 255;
		colourChange = true;
    }

    public void changeG(float slider)
    {
		rgbColor.y = slider / 255;
		colourChange = true;
    }

    public void changeB(float slider)
    {
		rgbColor.z = slider / 255;
		colourChange = true;
    }


    public void BackToItems()
    {
        if (current_state == StoreState.STATE_STORE)
        {
            current_state = StoreState.STATE_ITEM;
            GetComponent<Animator>().SetTrigger("toItemSelection");
        }
    }
}
