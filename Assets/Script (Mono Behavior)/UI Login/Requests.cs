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
    public int roleId;
    public string doB;
    public string address;
    public string phone;
}

[Serializable]
public class Account
{
    public int id;
    public string name;
    public string email;
    public string password;
    public string gender;
    public DateTime doB;
    public string roleName;
    public string address;
    public string phone;
    public string status;
    public DateTime createDateTime;
    public string accessToken;
}

