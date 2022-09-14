using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PauseMenu : NetworkBehaviour
{
    [SerializeField]
    private GameObject pauseCanvas;
    [SerializeField]
    private Slider slider;
    private MouseLook mouseLook;
    private bool isMenuOpen = false;

    public Text sens;

    private void Start()
    {
        if(isLocalPlayer)
        {
            mouseLook = GetComponent<MouseLook>();
        }
    }

    private void Update()
    {
        sens.text = "Sensitivity  " + mouseLook.GetSens().ToString();
        if(isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isMenuOpen = !isMenuOpen;
                if (isMenuOpen)
                {
                    Debug.Log("Open menu");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    pauseCanvas.SetActive(true);
                    slider.value = mouseLook.GetSens();
                }else if(!isMenuOpen)
                {
                    Debug.Log("Close menu");
                    pauseCanvas.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            if (isMenuOpen)
            {
                mouseLook.setSens(slider.value);
            }
        }
    }
}
