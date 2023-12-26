using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFullScreenFix : MonoBehaviour
{
    enum ModeFullScreen
    {
        WIDTH,
        HEIGHT
    }
    #region PARAMS
    //private
    private const float RATIO_SCREEN_BASE = 16f / 9f;
    private float _ratioScale = 1;

    private Vector3 beginScale;
    private ModeFullScreen modeScreen = ModeFullScreen.WIDTH;

    #endregion
    //TODO : CTOR
    #region

    void Awake()
    {
        beginScale = this.transform.localScale;

    }
    void OnEnable()
    {
        ScalePanelFollowRatioScreen();
    }
#if UNITY_EDITOR
    void Update()
    {
        ScalePanelFollowRatioScreen();
    }
#endif

    #endregion
    //TODO : VIEW 
    #region

    public void ScalePanelFollowRatioScreen()//thay doi scale panel dua theo ti le man hinh hien tai
    {
        float ratioScale = RatioScale;
        //Debuger.Log("beginScale " + beginScale);
        //Debuger.Log("ratioScale " + ratioScale);
        transform.localScale = new Vector3(ratioScale * beginScale.x, ratioScale * beginScale.y, ratioScale * beginScale.z);
    }
    #endregion
    //TODO : GET SET
    #region
    public float RatioScale
    {
        get
        {
            Vector2 size = FWUtilities.GetMainGameViewSize();
            float screenRatioWidth = size.x / size.y;
            if (screenRatioWidth > RATIO_SCREEN_BASE)
            {
                modeScreen = ModeFullScreen.WIDTH;
            }
            else
            {
                modeScreen = ModeFullScreen.HEIGHT;
            }

            switch (modeScreen)
            {
                case ModeFullScreen.HEIGHT:
                    float screenRatioHeight2 = size.x / size.y;
                    float ratioWidth2 = 1 / RATIO_SCREEN_BASE;
                    _ratioScale = ratioWidth2 > screenRatioHeight2 ? ratioWidth2 / screenRatioHeight2 : 1;
                    break;
                case ModeFullScreen.WIDTH:
                    float screenRatioHeight = size.y / size.x;
                    float ratioWidth = 1 / RATIO_SCREEN_BASE;
                    //_ratioScale = ratioWidth > screenRatioHeight ? ratioWidth / screenRatioHeight : 1;
                    _ratioScale = (float)(ratioWidth / screenRatioHeight);
                    break;
            }


            return _ratioScale;
        }
        set { _ratioScale = value; }
    }
    #endregion
}
