using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalityManager : ManagerTemplate<PersonalityManager>
{
    Dictionary<int, PersonalityBase> personalities = new Dictionary<int, PersonalityBase>();

    // Load Data from local
    public void LoadPersonalityData()
    {

    }

    public void SavePersonalityData(PersonalityBase personality) 
    { 
    
    }

    // Save Data to server - call API
    public void SavePersonalityDataToServer() 
    {

    }

    // 

}
