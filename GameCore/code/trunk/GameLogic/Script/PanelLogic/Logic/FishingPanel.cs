using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingPanel : UIBase
{
    public FishingUI _FishingUI;

    //Field
    private float catchingPercentage;

    //Fish
    private bool waiting = false;
    private Vector3 currentDestination;
    private float minWaitTime = 0.5f;
    private float maxWaitTime = 1f;
    private float moveSpeed = 1f;
    private bool inTrigger = false;
    private float catchPercentage = 0f;
    private float catchMultiplier = 10f;
    private float catchingForce = 200000;

    protected override void Init()
    {
        _FishingUI = new FishingUI();
        _FishingUI._fishingPanel = this;
        _FishingUI.Init();
        currentDestination = RandomDestination(); //Give the fish a random direction to go to

    }

    protected override void OnUpdate()
    {
        //CatchingBar
        if (InputManager.Instance.GetInteractPressed())
        { //If we press space
            _FishingUI._CatchingBar.AddForce(Vector2.up * catchingForce * Time.deltaTime, ForceMode2D.Force); //Add force to lift the bar
        }
        //Fish
        CheckCollisions(_FishingUI._Fish);

        _FishingUI._Fish.position = Vector3.Lerp(_FishingUI._Fish.position, currentDestination, moveSpeed * Time.deltaTime); //Lerp towards the fishes current destination
        if (Vector3.Distance(_FishingUI._Fish.position, currentDestination) <= 1f && !waiting)
        { //If we get close and arent already waiting then get a new destination
            UIStartCoroutine(Wait());
        }

        if (inTrigger)
        {
            catchPercentage += catchMultiplier * Time.deltaTime;
        }
        else
        {
            catchPercentage -= catchMultiplier * Time.deltaTime;
        }

        catchPercentage = Mathf.Clamp(catchPercentage, 0, 100);
        _FishingUI._CatchPercentage.value = catchPercentage;

        if (catchPercentage >= 100)
        { //Fish is caught if percentage is full
            catchPercentage = 0;
            FishCaught();
        }
    }

    public void FishCaught()
    {
        SceneData scenedata = SceneDataManager.currentScene._sceneData;
        scenedata.AddItem(1);
        this.gameObject.SetActive(false);
    }

    IEnumerator Wait()
    {
        waiting = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(minWaitTime, maxWaitTime));
        currentDestination = RandomDestination();
        waiting = false;
    }

    private Vector3 RandomDestination()
    {
        //Pick a random height to go to, between the top and bottom but they are offset using the height of the fish so it doesnt overlpa
        var rectT = _FishingUI._Fish;
        var maxUp = _FishingUI._TopBar.position.y - rectT.sizeDelta.y / 2;
        var maxDown = _FishingUI._BottomBar.position.y + rectT.sizeDelta.y / 2;
        var newHeight = UnityEngine.Random.Range(maxUp, maxDown);

        return new Vector3(_FishingUI._Fish.position.x, newHeight, _FishingUI._Fish.position.z);
    }

    protected void CheckCollisions(Transform transform)
    {
        // Perform your collision checks here
        // Example: Check for collisions with all colliders in the scene
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (Collider2D collider in colliders)
        {
            if(collider.name.Equals("CatchingBar"))
            {
                inTrigger = true;
                return;
            }
        }
        inTrigger = false;
    }
}
