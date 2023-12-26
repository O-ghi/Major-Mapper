using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;

public static class FWUtilities
{
    private static int SECOND = 1000;
    private static int MINUTE = 60 * SECOND;
    private static int HOUR = 60 * MINUTE;
    private static int DAY = 24 * HOUR;
    public static string ConvertTimeToString(this int time)
    {
        int minute = time / 60;
        int secon = time % 60;
        return (minute < 10 ? "0" + minute : minute + "") + ":" + (secon < 10 ? "0" + secon : secon + "");
    }
    public static string ConvertTimeToDay(long ms, int maxLenghTime = 2)//second
    {
        int countTime = 0;
        StringBuilder text = new StringBuilder("");
        if (ms > DAY)
        {
            long day = (long)(ms / DAY);
            //Debug.Log("day " + day.ToString() + " DAY "+ DAY + " ms "+ ms);
            if (day > 0 && countTime < maxLenghTime)
            {
                text.Append(day).Append(" " + "day" + " ");
                ms %= DAY;
                countTime++;
                return text.ToString();
            }
        }
        if (ms > HOUR)
        {
            long hour = (long)ms / HOUR;
            //Debug.Log("hour " + hour.ToString());

            if (hour > 0 && countTime < maxLenghTime)
            {
                text.Append(hour).Append(" " + "huor" + " ");
                ms %= HOUR;
                countTime++;
            }
        }
        if (ms > MINUTE)
        {
            long minute = (long)ms / MINUTE;
            //Debug.Log("minute " + minute.ToString());

            if (minute > 0 && countTime < maxLenghTime)
            {
                text.Append(minute).Append(" " + "minute" + " ");
                ms %= MINUTE;
                countTime++;
            }
        }
        if (ms > SECOND)
        {
            long second = (long)ms / SECOND;
            //Debug.Log("second " + second.ToString());

            if (second > 0 && countTime < maxLenghTime)
            {
                text.Append(second).Append(" " + "second" + "");
                ms %= SECOND;
                countTime++;
            }
        }
        //text.Append(ms + " ms");
        return text.ToString();
    }
    public static string ConvertTimeToString(float ms)//second
    {
        //int maxLenghTime = 2;
        StringBuilder text = new StringBuilder("");
        if (ms > HOUR)
        {
            long hour = (long)ms / HOUR;
            string hourStr = hour > 9 ? hour + "" : "0" + hour;
            Debug.Log("hour " + hour.ToString());
            if (hour > 0)
            {
                text.Append(hourStr).Append(":");
                ms %= HOUR;
            }
        }
        if (ms > MINUTE)
        {
            long minute = (long)ms / MINUTE;
            Debug.Log("minute " + minute.ToString());
            string minuteStr = minute > 9 ? minute + "" : "0" + minute;
            if (minute > 0)
            {
                text.Append(minuteStr).Append(":");
                ms %= MINUTE;
            }
        }
        if (ms > SECOND)
        {
            long second = (long)ms / SECOND;
            string secondStr = second > 9 ? second + "" : "0" + second;
            Debug.Log("second " + secondStr);
            if (second > 0)
            {
                text.Append(secondStr);
                ms %= SECOND;
            }
        }
        //text.Append(ms + " ms");
        return text.ToString();
    }
    public static long ConvertNumberThousand(long value)
    {
        return value = ((long)(value / 10)) * 10;
        //return value;
    }


    public static void SetNativeSizeRuntime(Image img)
    {
        img.rectTransform.sizeDelta = new Vector2(img.sprite.bounds.size.x, img.sprite.bounds.size.y);

    }
    public static void ChangeImage(Transform btn, bool isEnable, float alpha = 0.5f)
    {

        if (isEnable)
        {
            alpha = 1;
        }
        if (btn.transform.childCount > 0)
        {
            foreach (Transform child in btn.transform)
            {
                if (child.GetComponent<Image>() != null)
                {
                    child.GetComponent<Image>().color = ChangeAlphaColor(child.GetComponent<Image>().color, alpha);
                }
                else if (child.GetComponent<Text>() != null)
                {
                    child.GetComponent<Text>().color = ChangeAlphaColor(child.GetComponent<Text>().color, alpha);
                }
                if (child.childCount > 0)
                {
                    ChangeImage(child, isEnable);
                }
            }
        }
    }

    public static void ChangeWidthPanelFollowTextNotChange(RectTransform imgDialogChat, Text txtMessage, float paddingSize,
        float maxSize = 0)
    {
        float anchorY = imgDialogChat.anchoredPosition.x;
        float deltaY = imgDialogChat.sizeDelta.y;
        //chiều cao ban đầu
        float fristDeltaX = imgDialogChat.sizeDelta.x;
        float maxWidth = (txtMessage.preferredWidth + paddingSize);
        maxWidth = maxSize > maxWidth ? maxSize : maxWidth;
        Vector2 sizeDelta = new Vector2(maxWidth, deltaY);
        imgDialogChat.sizeDelta = sizeDelta;
    }

    public static void ChangeWidthPanelFollowText(RectTransform imgDialogChat, Text txtMessage, float paddingSize, float maxSize = 0)
    {
        float anchorY = imgDialogChat.anchoredPosition.x;
        float deltaY = imgDialogChat.sizeDelta.y;
        //chiều cao ban đầu
        float fristDeltaX = imgDialogChat.sizeDelta.x;
        float maxWidth = (txtMessage.preferredWidth + paddingSize);
        maxWidth = maxSize > maxWidth ? maxSize : maxWidth;
        Vector2 sizeDelta = new Vector2(maxWidth, deltaY);
        imgDialogChat.sizeDelta = sizeDelta;
        //chiều cao mới
        float newDeltaX = imgDialogChat.sizeDelta.x;
        //tính delta lấy chiều cao ban đầu trừ chiều cao đã có text
        float deltaX = (fristDeltaX - newDeltaX) / 2;
        //vị trí mới
        float newAnchorPos = deltaX + imgDialogChat.anchoredPosition.x;
        Vector2 anchorPos = new Vector2(anchorY, newAnchorPos);
        imgDialogChat.anchoredPosition = anchorPos;
    }
    public static void ChangeHeightPanelFollowText(RectTransform imgDialogChat, Text txtMessage, float paddingSize, bool isAnchorTop = true)//bien cuoi cung cho phep can top hay ko
    {
        float anchorX = imgDialogChat.anchoredPosition.x;
        float deltaX = imgDialogChat.sizeDelta.x;
        //chiều cao ban đầu
        float fristDeltaY = imgDialogChat.sizeDelta.y;
        Vector2 sizeDelta = new Vector2(deltaX, (txtMessage.preferredHeight + paddingSize));
        imgDialogChat.sizeDelta = sizeDelta;
        //chiều cao mới
        float newDeltaY = imgDialogChat.sizeDelta.y;
        //tính delta lấy chiều cao ban đầu trừ chiều cao đã có text
        float deltaY = (fristDeltaY - newDeltaY) / 2;
        //vị trí mới
        if (isAnchorTop)
        {
            float newAnchorPos = deltaY + imgDialogChat.anchoredPosition.y;
            Vector2 anchorPos = new Vector2(anchorX, newAnchorPos);
            imgDialogChat.anchoredPosition = anchorPos;
        }
    }
    public static bool CheckPrecisionTwoPos(Vector2 pos1, Vector2 pos2, float offset)
    {
        if (Mathf.Abs(pos1.x - pos2.x) < offset && Mathf.Abs(pos1.y - pos2.y) < offset)
        {
            return true;
        }
        return false;
    }
    public static void ChangeBrightnessColor(Text text, float a)
    {
        Color color = text.color;
        float alpha = color.a;
        color = color / 2;
        color.a = alpha;
        text.color = color;
    }
    public static Color hexColorParseToColor(string hexstring)
    {
        Color c = new Color();
        ColorUtility.TryParseHtmlString(hexstring, out c);
        return c;
    }
    public static void ChangeBrightnessColor(Image img, float a, float dis)
    {
        Color color = img.color;
        float alpha = color.a;
        color = color / dis;
        color.a = alpha;
        img.color = color;
    }
    public static void ChangeAlphaColor(Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }
    public static void ChangeAlphaColor(Image img, float alpha)
    {
        if (!img.IsDestroyed() && img != null)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
        }
    }
    public static Color ChangeAlphaColor(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
    /*
    //Rut ngan text khi do dai vuot qua textfield : 
       vd : 1234567 = 1234...
    */
    public static bool setTextOverflowbySize(Text text, String message)
    {
        text.text = message;
        if (message.Length < 3)
        {
            return false;
        }
        RectTransform rect = (RectTransform)text.transform;
        Vector2 extents = rect.rect.size;
        var settings = text.GetGenerationSettings(extents);
        text.cachedTextGenerator.Populate(text.text, settings);
        string textUpdate = string.Empty;
        float scale = extents.x / text.preferredWidth;

        if (scale < 1 && text.cachedTextGenerator.characterCount > 2)
        {
            Debug.LogWarning("text.text " + text.text + " text.cachedTextGenerator.characterCount " + text.cachedTextGenerator.characterCount + " settings.fontSize " + settings.fontSize);
            textUpdate = text.text.Substring(0, text.cachedTextGenerator.characterCount - 2);
            textUpdate += "...";
            text.text = textUpdate;
            return true;
        }
        else
        {
            text.text = message;
        }
        return false;

        //Debug.Log("textUpdate " + textUpdate + " scale " + scale);
    }
    public static void setOffsetPopup(Transform popup)
    {
        RectTransform rect = (RectTransform)popup.transform;
        rect.offsetMax = Vector2.zero;
        rect.offsetMin = Vector2.zero;
    }

    public static string getUrlFacebookAvatar(string userId)
    {
        return "https://graph.facebook.com/" + userId + "/picture?type=large";
    }
    public static float getFloat(float value)
    {
        return getFloat(value, 2); //lay 2 chu so
    }
    public static float getFloat(float value, int n)
    {
        return (float)(Math.Round(value * Mathf.Pow(10f, n), MidpointRounding.AwayFromZero)) / (float)Mathf.Pow(10f, n);
    }

    public static long parsefloatToLong(float value)
    {
        return (long)Math.Round(value, MidpointRounding.AwayFromZero);
    }
    public static int parsefloatToInt(float value)
    {
        Debug.LogError("parsefloatToInt " + value);
        int intRound = (int)Math.Round(value, MidpointRounding.AwayFromZero);
        if (intRound < value)
        {
            intRound++;
        }

        return intRound;
    }
    //public static List<T> sortListByT<T>(List<T> list)
    //{
    //    //list = list.OrderBy(T.)
    //}
    public static void setWidthObject(RectTransform obj, RectTransform content)
    {
        obj.sizeDelta = new Vector2(content.sizeDelta.x, obj.rect.height);
    }
    public static List<T> OrderBy<T, P>(IEnumerable<T> collection, Func<T, P> propertySelector)
    {
        return (from item in collection
                orderby propertySelector(item)
                select item).ToList();
    }

    public static List<T> OrderByDesc<T, P>(IEnumerable<T> collection, Func<T, P> propertySelector)
    {
        return (from item in collection
                orderby propertySelector(item) descending
                select item).ToList();
    }
    public static string GetIPString()
    {
#if !UNITY_EDITOR
        try
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            //Debug.LogError("IP "+ addr[addr.Length - 1].ToString());
            return addr[addr.Length - 1].ToString();
        }
        catch
        {
        }
#endif
        Debug.LogError("IP NULL");
        return "";
    }

    public static string getAllCardInt(IList<int> cardList)
    {
        string pocketHand = "";
        if (cardList.Count == 0)
        {
            return pocketHand;
        }
        foreach (int card in cardList)
        {
            string typeCard = getTypeCard(card);
            pocketHand += typeCard + " ";
        }
        return pocketHand.Remove(pocketHand.Length - 1);
    }
    public static string getTypeCard(int indexCard)
    {
        int cardRank = indexCard / 4;
        int cardSuit = indexCard % 4;
        string typeRank = "";
        string typeSuit = "";
        if (cardRank <= 7)
        {
            typeRank = (cardRank + 2).ToString();
        }
        else
        {
            switch (cardRank)
            {
                case 8:
                    typeRank = "T";
                    break;
                case 9:
                    typeRank = "J";
                    break;
                case 10:
                    typeRank = "Q";
                    break;
                case 11:
                    typeRank = "K";
                    break;
                case 12:
                    typeRank = "A";
                    break;
            }
        }
        switch (cardSuit)
        {
            case 0:
                typeSuit = "S";
                break;
            case 1:
                typeSuit = "C";
                break;
            case 2:
                typeSuit = "D";
                break;
            case 3:
                typeSuit = "H";
                break;
        }
        return typeRank + typeSuit;
    }
    public static string getValueOfCard(int cardInt)
    {
        int cardRank = cardInt / 4;
        string typeRank = "";
        //string typeSuit = "";
        if (cardRank <= 7)
        {
            typeRank = (cardRank + 2).ToString();
        }
        else
        {
            switch (cardRank)
            {
                case 8:
                    typeRank = "10";
                    break;
                case 9:
                    typeRank = "J";
                    break;
                case 10:
                    typeRank = "Q";
                    break;
                case 11:
                    typeRank = "K";
                    break;
                case 12:
                    typeRank = "A";
                    break;
            }
        }
        return typeRank;
    }
    public static string getTypeOfCard(int cardInt)
    {
        int cardSuit = cardInt % 4;
        string typeSuit = "";
        switch (cardSuit)
        {
            case 0:
                typeSuit = "S";
                break;
            case 1:
                typeSuit = "C";
                break;
            case 2:
                typeSuit = "D";
                break;
            case 3:
                typeSuit = "H";
                break;
        }
        return typeSuit;
    }
    public static string HexAlphaColor(int intNumber)
    {
        string hexValue = intNumber.ToString("X");
        if (hexValue.Length == 1)
        {
            hexValue = "0" + hexValue;
        }
        return hexValue;
    }
    //
    private static string SLIP_AVATAR;
    public static void WriteTextureToPlayerPrefs(string tag, string version, Texture2D tex)
    {
        // if texture is png otherwise you can use tex.EncodeToJPG().
        byte[] texByte = tex.EncodeToPNG();

        // convert byte array to base64 string
        string base64Tex = Convert.ToBase64String(texByte);
        string data = version + SLIP_AVATAR + base64Tex;
        Debug.LogError("base64Tex " + data);
        // write string to playerpref
        PlayerPrefs.SetString(tag, data);
        PlayerPrefs.Save();
    }

    public static Texture2D ReadTextureFromPlayerPrefs(string tag)
    {
        // load string from playerpref
        string base64Tex = PlayerPrefs.GetString(tag, null);
        string[] dataResource = base64Tex.Split(new string[] { SLIP_AVATAR }, StringSplitOptions.None);
        Debug.LogError("dataResource " + dataResource);
        //if (dataResource.Length >= 2)
        //{
        if (!string.IsNullOrEmpty(dataResource[1]))
        {
            // convert it to byte array
            byte[] texByte = Convert.FromBase64String(dataResource[1]);
            Texture2D tex = new Texture2D(2, 2);

            //load texture from byte array
            if (tex.LoadImage(texByte))
            {
                return tex;
            }
        }
        //}
        return null;
    }
    //TODO : SCREEN 
    #region
    public static Vector2 GetMainGameViewSize()
    {
        //		#if UNITY_EDITOR
        //        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        //        System.Reflection.MethodInfo getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //        System.Object res = getSizeOfMainGameView.Invoke(null, null);
        //        return (Vector2)res;
        //		#endif
        return new Vector2(Screen.width, Screen.height);
    }
    #endregion


    #region Get MD5
    public static string GetMd5Hash(string input)
    {
        //Debuger.Log("server list input " + input);
        MD5 md5 = new MD5CryptoServiceProvider();

        //compute hash from the bytes of text  
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(input));


        //get hash result after compute it  
        byte[] result = md5.Hash;

        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            //change it into 2 hexadecimal digits  
            //for each byte  
            strBuilder.Append(result[i].ToString("x2"));
        }

        return strBuilder.ToString();
    }
    public static string getMd5(string str)
    {
        byte[] md5 = new MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(str));
        return BitConverter.ToString(md5);
    }

    public static string getMd5str(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();

        //compute hash from the bytes of text  
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(str));


        //get hash result after compute it  
        byte[] result = md5.Hash;

        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            //change it into 2 hexadecimal digits  
            //for each byte  
            strBuilder.Append(result[i].ToString("x2"));
        }

        return strBuilder.ToString();
    }


    /// <summary>
    /// 计算MD5
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string getMd5(byte[] bytes)
    {
        byte[] md5 = new MD5CryptoServiceProvider().ComputeHash(bytes);
        return BitConverter.ToString(md5).Replace("-", "");
    }

    public static byte[] getMd5Byte(byte[] bytes)
    {
        return new MD5CryptoServiceProvider().ComputeHash(bytes);
    }

    /// <summary>
    /// 计算文件MD5，需要外部关闭流
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public static string getMd5(FileStream fs)
    {
        string ret = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(fs));
        return ret.Replace("-", "");
    }

    #endregion


    //Check Resolution
    public static float IsScaleRatioFOW()
    {
        float ratio_3_2 = 3f / 2f;
        float ratio_4_3 = 4f / 3f;
        Vector2 size = FWUtilities.GetMainGameViewSize();
        float ratioScreen = size.x / size.y;
        //Debug.Log("ratioScreen " + ratioScreen);
        if (Mathf.Abs(ratio_3_2 - ratioScreen) < 0.01f)
        {
            return 1.15f;
        }
        if (Mathf.Abs(ratio_4_3 - ratioScreen) < 0.01f)
        {
            return 1.2f;
        }
        return 1;
    }
}
