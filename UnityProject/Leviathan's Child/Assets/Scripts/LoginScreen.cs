using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    [SerializeField]
    private InputField nickInput;
    [SerializeField]
    private InputField passwordInput;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnPress_LoginButton()
    {
        if (!NickInputIsValid())
        {
            SetInputBackgroundColor(nickInput, new Color(1f, 0.7f, 0.7f));
            return;
        }
        else SetInputBackgroundColor(nickInput, new Color(1f, 1f, 1f));

        if (!PasswordInputIsValid())
        {
            SetInputBackgroundColor(passwordInput, new Color(1f, 0.7f, 0.7f));
            return;
        }
        else SetInputBackgroundColor(passwordInput, new Color(1f, 1f, 1f));

        User logedUser = UserService.instance.Authentication(nickInput.text, passwordInput.text);
    }

    private bool NickInputIsValid() => nickInput.text.Length >= 4;

    private void SetInputBackgroundColor(InputField input, Color color) => input.image.color = color;

    private bool PasswordInputIsValid() => passwordInput.text.Length >= 8;
}
