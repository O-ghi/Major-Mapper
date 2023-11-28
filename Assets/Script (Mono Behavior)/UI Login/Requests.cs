using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Requests
{

}

[Serializable]
public class Register
{
    public string name;
    public string email;
    public string password;
    public string confirmPassword;
    public string gender;
    public int role;
    public string doB;
    public string address;
    public string phone;
}

[Serializable]
public class LogIn
{
    public int role;
    public string access_token;
    public string token_type;
    public string expires_in;
    public string username;

    public string Issued;

    public string Expires;
}
