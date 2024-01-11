using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollandPersonality : PersonalityBase
{
    private float realistic;
    private float investigative;
    private float artistic;
    private float social;
    private float enterprising;
    private float conventional;

    public HollandPersonality()
    {

    }
    public HollandPersonality(float realistic, float investigative, float artistic, float social, float enterprising, float conventional)
    {
        this.Realistic = realistic;
        this.Investigative = investigative;
        this.Artistic = artistic;
        this.Social = social;
        this.Enterprising = enterprising;
        this.Conventional = conventional;
    }

    public float Realistic { get => realistic; set => realistic = value; }
    public float Investigative { get => investigative; set => investigative = value; }
    public float Artistic { get => artistic; set => artistic = value; }
    public float Social { get => social; set => social = value; }
    public float Enterprising { get => enterprising; set => enterprising = value; }
    public float Conventional { get => conventional; set => conventional = value; }
}

public enum HollandType
{
    REALISTIC = 1,
    INVESTIGATIVE,
    ARTISTIC,
    SOCIAL,
    ENTERPRISING,
    CONVENTIONAL
}
