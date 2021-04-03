using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Easing
{
    Linear = 0,
    EaseIn,
    EaseOut,
    EaseInOut
}
public enum Axis
{
    X = 0,
    Y,
    Z,
    XY,
    XZ,
    YZ,
    XYZ
}
public enum Format
{
    Standard,       // 0 - 255
    Scalar,         // 0 - 1
    Percentage      // 0% - 100%
}

public class Ease
{
    #region //Fill Amount
    /// <summary>
    /// Ease the fill amount of an image (bar) from its start value to final value
    /// </summary>
    /// <param name="image">The GameObject used to change its position</param>
    /// <param name="startVal">The initial fill amount</param>
    /// <param name="finalVal">The final fill amount</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator FillAmountEase(Image image, float startVal, float finalVal, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        float SV = startVal;
        float FV = finalVal;

        while (time <= duration)
        {
            float currentVal = SV;
            currentVal = CalculateFillAmount(ease, SV, FV, time / duration, power);

            image.fillAmount = currentVal;
            yield return null;
            time += Time.deltaTime;
        }
        image.fillAmount = FV;
    }

    /// <summary>
    /// Calculate fill amount.
    /// </summary>
    /// <param name="ease">Easing type</param>
    /// <param name="SV">Start value</param>
    /// <param name="FV">Final value</param>
    /// <param name="timeRatio">The current time over the duration (time/duration)</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    /// <returns>Fill amount</returns>
    private static float CalculateFillAmount(Easing ease, float SV, float FV, float timeRatio, uint power)
    {
        float value = SV;

        switch (ease)
        {
            case Easing.Linear:
                value = Mathf.Lerp(SV, FV, timeRatio);
                break;
            case Easing.EaseIn:
                value = Mathf.Lerp(SV, FV, EaseIn(timeRatio, power));
                break;
            case Easing.EaseOut:
                value = Mathf.Lerp(SV, FV, EaseOut(timeRatio, power));
                break;
            case Easing.EaseInOut:
                value = Mathf.Lerp(SV, FV, EaseInOut(timeRatio, power));
                break;
        }
        return value;
    }
    #endregion

    #region //Translation
    /// <summary>
    /// Translate GameObject from its intial position to its final position.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its position</param>
    /// <param name="target">The final position</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator TranslateTo(GameObject Obj, Vector3 target, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 startPos = new Vector3(Obj.transform.position.x, Obj.transform.position.y, Obj.transform.position.z);
        Vector3 targetPos = new Vector3(target.x, target.y, Obj.transform.position.z);

        Vector3 SP = startPos;
        Vector3 TP = targetPos;

        Obj.transform.position = SP;

        while (time <= duration)
        {
            Vector3 currentPosition = SP;
            currentPosition = CalculateVector(ease, SP, TP, time / duration, power, true, true, true);

            Obj.transform.position = currentPosition;
            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.position = new Vector3(TP.x, TP.y, Obj.transform.position.z);
    }

    public static IEnumerator AnchoredTranslateTo(Image Obj, Vector2 target, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector2 startPos = new Vector2(Obj.rectTransform.anchoredPosition.x, Obj.rectTransform.anchoredPosition.y);
        Vector2 targetPos = new Vector2(target.x, target.y);

        Vector2 SP = startPos;
        Vector2 TP = targetPos;

        Obj.rectTransform.anchoredPosition = SP;

        while (time <= duration)
        {
            Vector3 currentPosition = SP;
            currentPosition = CalculateVector(ease, SP, TP, time / duration, power, true, true, true);

            Obj.rectTransform.anchoredPosition = currentPosition;
            yield return null;
            time += Time.deltaTime;
        }
        Obj.rectTransform.anchoredPosition = new Vector2(TP.x, TP.y);
    }

    /// <summary>
    /// Translates GameObject along the x-axis.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its position</param>
    /// <param name="posX">The final x position</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator TranslateXTo(GameObject Obj, float posX, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 startPos = Obj.transform.position;
        Vector3 targetPos = new Vector3(posX, Obj.transform.position.y, Obj.transform.position.z);

        Vector3 SP = startPos;
        Vector3 TP = targetPos;

        Obj.transform.position = SP;

        if (duration == 0)
        {
            Obj.transform.position = new Vector3(TP.x, Obj.transform.position.y, Obj.transform.position.z);
            yield break;
        }
        while (time <= duration)
        {
            Vector3 currentPosition = SP;
            currentPosition.x = CalculateVector(ease, SP, TP, time / duration, power, true, false, false).x;

            Obj.transform.position = new Vector3(currentPosition.x, Obj.transform.position.y, Obj.transform.position.z);
            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.position = new Vector3(TP.x, Obj.transform.position.y, Obj.transform.position.z);
    }
    /// <summary>
    /// Translates GameObject along the y-axis.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its position</param>
    /// <param name="posY">The final y position</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator TranslateYTo(GameObject Obj, float posY, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 startPos = Obj.transform.position;
        Vector3 targetPos = new Vector3(Obj.transform.position.x, posY, Obj.transform.position.z);

        Vector3 SP = startPos;
        Vector3 TP = targetPos;

        Obj.transform.position = SP;

        while (time <= duration)
        {
            Vector3 currentPosition = SP;
            currentPosition.y = CalculateVector(ease, SP, TP, time / duration, power, false, true, false).y;

            Obj.transform.position = new Vector3(Obj.transform.position.x, currentPosition.y, Obj.transform.position.z);
            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.position = new Vector3(Obj.transform.position.x, TP.y, Obj.transform.position.z);
    }
    #endregion

    #region //Scale
    /// <summary>
    /// Scale GameObject.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its scale</param>
    /// <param name="finalScale">The final scale of the object</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator ScaleTo(GameObject Obj, Vector2 finalScale, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 origScl = new Vector3(Obj.transform.localScale.x, Obj.transform.localScale.y, Obj.transform.localScale.z);
        Vector3 finalScl = new Vector3(finalScale.x, finalScale.y, Obj.transform.localScale.z);

        Vector3 OS = origScl;
        Vector3 FS = finalScl;

        Obj.transform.localScale = OS;

        while (time <= duration)
        {
            Vector3 currentScale = OS;
            currentScale = CalculateVector(ease, OS, FS, time / duration, power, true, true, true);

            Obj.transform.localScale = new Vector3(currentScale.x, currentScale.y, Obj.transform.localScale.z);
            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.localScale = FS;
    }

    /// <summary>
    /// Scale GameObject horizontally (x-axis).
    /// </summary>
    /// <param name="Obj">The GameObject used to change its scale</param>
    /// <param name="scaleX">Width of the object</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator ScaleXTo(GameObject Obj, float scaleX, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 OS = Obj.transform.localScale;
        Vector3 FS = new Vector3(scaleX, Obj.transform.localScale.y);

        Obj.transform.localScale = OS;

        while (time <= duration)
        {
            Vector3 currentScale = OS;
            currentScale.x = CalculateVector(ease, OS, FS, time / duration, power, true, false, false).x;

            Obj.transform.localScale = currentScale;

            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.localScale = FS;
    }
    /// <summary>
    /// Scale GameObject vertically (y-axis).
    /// </summary>
    /// <param name="Obj">The GameObject used to change its scale</param>
    /// <param name="scaleY">Height of the object</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator ScaleYTo(GameObject Obj, float scaleY, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 OS = Obj.transform.localScale;
        Vector3 FS = new Vector3(Obj.transform.localScale.x, scaleY);

        Obj.transform.localScale = OS;

        while (time <= duration)
        {
            Vector3 currentScale = OS;
            currentScale.y = CalculateVector(ease, OS, FS, time / duration, power, false, true, false).y;

            Obj.transform.localScale = currentScale;

            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.localScale = FS;
    }
    #endregion

    #region //Rotation
    /// <summary>
    /// Rotate GameObject at an absolute angle.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its rotation</param>
    /// <param name="angle">Angle in degrees</param>
    /// <param name="axis">Vector axis on a coordinate plane</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator RotateTo(GameObject Obj, float angle, Axis axis, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 OS = Obj.transform.rotation.eulerAngles;
        Vector3 FS = GetAxisVector(axis);

        FS *= angle;
        Obj.transform.rotation = Quaternion.Lerp(Obj.transform.rotation, Quaternion.Euler(OS), 1f);

        while (time <= duration)
        {
            Vector3 currentAngle = OS;
            currentAngle = CalculateVector(ease, OS, FS, time / duration, power, true, true, true);

            Obj.transform.rotation = Quaternion.Lerp(Obj.transform.rotation, Quaternion.Euler(currentAngle), time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.rotation = Quaternion.Euler(FS);
    }

    /// <summary>
    /// Rotate GameObject relative to the GameObject's current rotation.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its rotation</param>
    /// <param name="angle">Angle in degrees</param>
    /// <param name="axis">Vector axis on a coordinate plane</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator RelRotateTo(GameObject Obj, float angle, Axis axis, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 OS = Obj.transform.rotation.eulerAngles;
        Vector3 FS = GetAxisVector(axis);

        FS *= angle;
        FS += OS;
        Obj.transform.rotation = Quaternion.Lerp(Obj.transform.rotation, Quaternion.Euler(OS), 1f);

        while (time <= duration)
        {
            Vector3 currentAngle = OS;
            currentAngle = CalculateVector(ease, OS, FS, time / duration, power, true, true, true);

            Obj.transform.rotation = Quaternion.Lerp(Obj.transform.rotation, Quaternion.Euler(currentAngle), time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        Obj.transform.rotation = Quaternion.Euler(FS);
    }

    /// <summary>
    /// Fully rotate (360 degrees) GameObject relative to the GameObject's current rotation in a set number of cycles.
    /// </summary>
    /// <param name="Obj">The GameObject used to change its rotation</param>
    /// <param name="numOfCycles">Number of cycles the object will rotate 360 degrees</param>
    /// <param name="axis">Vector axis on a coordinate plane</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator RelRotateCycles(GameObject Obj, int numOfCycles, Axis axis, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;
        float angle = (numOfCycles != 0 ? 360f * numOfCycles : 360f);

        Vector3 OS = Obj.transform.rotation.eulerAngles;
        Vector3 FS = GetAxisVector(axis);

        FS *= angle;
        FS += OS;
        Obj.transform.rotation = Quaternion.Lerp(Obj.transform.rotation, Quaternion.Euler(OS), 1f);
        do
        {
            while (time <= duration)
            {
                Vector3 currentAngle = OS;
                currentAngle = CalculateVector(ease, OS, FS, time / duration, power, true, true, true);

                Obj.transform.rotation = Quaternion.Lerp(Obj.transform.rotation, Quaternion.Euler(currentAngle), time / duration);
                yield return null;
                time += Time.deltaTime;
            }
            Obj.transform.rotation = Quaternion.Euler(FS);
            time = 0f;
        }
        while (numOfCycles == 0);
    }

    /// <summary>
    /// Rotate text at an absolute angle.
    /// </summary>
    /// <param name="mainText">The text used to change its rotation</param>
    /// <param name="angle">Angle in degrees</param>
    /// <param name="axis">Vector axis on a coordinate plane</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator TextRotateTo(TMP_Text mainText, float angle, Axis axis, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 OS = mainText.transform.rotation.eulerAngles;
        Vector3 FS = GetAxisVector(axis);

        FS *= angle;
        mainText.transform.rotation = Quaternion.Lerp(mainText.transform.rotation, Quaternion.Euler(OS), 1f);

        while (time <= duration)
        {
            Vector3 currentAngle = OS;
            currentAngle = CalculateVector(ease, OS, FS, time / duration, power, true, true, true);

            mainText.transform.rotation = Quaternion.Lerp(mainText.transform.rotation, Quaternion.Euler(currentAngle), time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        mainText.transform.rotation = Quaternion.Euler(FS);
    }

    /// <summary>
    /// Rotate text relative to the text's current rotation.
    /// </summary>
    /// <param name="mainText">The text used to change its rotation</param>
    /// <param name="angle">Angle in degrees</param>
    /// <param name="axis">Vector axis on a coordinate plane</param>
    /// <param name="duration">Completion time</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="ease">Easing type</param>
    public static IEnumerator TextRelRotateTo(TMP_Text mainText, float angle, Axis axis, float duration, uint power = 2, Easing ease = Easing.Linear)
    {
        float time = 0f;

        Vector3 OS = mainText.transform.rotation.eulerAngles;
        Vector3 FS = GetAxisVector(axis);

        FS *= angle;
        FS += OS;
        mainText.transform.rotation = Quaternion.Lerp(mainText.transform.rotation, Quaternion.Euler(OS), 1f);

        while (time <= duration)
        {
            Vector3 currentAngle = OS;
            currentAngle = CalculateVector(ease, OS, FS, time / duration, power, true, true, true);

            mainText.transform.rotation = Quaternion.Lerp(mainText.transform.rotation, Quaternion.Euler(currentAngle), time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        mainText.transform.rotation = Quaternion.Euler(FS);
    }
    #endregion

    #region //Color Change
    /// <summary>
    /// Change the color of a GameObject (RGBA).
    /// </summary>
    /// <param name="Obj">The GameObject used to change its color</param>
    /// <param name="color">The color of the GameObject</param>
    /// <param name="duration">Change the image's color to the final color in (time) seconds</param>
    /// <param name="colorFormat">The format use for converting color values (RGBA)</param>
    public static IEnumerator ColorChangeFromRGBA(GameObject Obj, Color color, float duration, Format colorFormat = Format.Scalar)
    {
        SpriteRenderer SPR = Obj.GetComponent<SpriteRenderer>();
        float time = 0f;
        int multiplier = 255;

        color = GetFormattedColor(color, ref multiplier, colorFormat);

        Color OS = SPR.color;
        Color FS = color;

        SPR.color = OS;

        while (time <= duration)
        {
            Color currentColor = OS;

            currentColor.r = Mathf.Lerp(OS.r, FS.r, time / duration);
            currentColor.g = Mathf.Lerp(OS.g, FS.g, time / duration);
            currentColor.b = Mathf.Lerp(OS.b, FS.b, time / duration);
            currentColor.a = Mathf.Lerp(OS.a, FS.a, time / duration);

            SPR.color = currentColor;
            yield return null;
            time += Time.deltaTime;
        }
        SPR.color = FS;
    }

    /// <summary>
    /// Change the color of a GameObject (Hex Values) Eg. "#009474"
    /// </summary>
    /// <param name="Obj">The GameObject used to change its color</param>
    /// <param name="hexString">Hex code (Eg. "#4287f5")</param>
    /// <param name="duration">Change the text's color to the final color in (time) seconds</param>
    /// <param name="alpha">The final opacity (0-1)</param>
    public static IEnumerator ColorChangeFromHex(GameObject Obj, string hexString, float duration, float alpha = 1f)
    {
        SpriteRenderer SPR = Obj.GetComponent<SpriteRenderer>();
        if (!CheckHexFormat(hexString)) { yield break; }
        float time = 0f;

        Color OS = SPR.color;
        Color FS = ConvertHexToRGB(hexString);
        FS.a = alpha;

        SPR.color = OS;

        while (time <= duration)
        {
            Color currentColor = OS;

            SPR.color = currentColor;

            yield return null;
            time += Time.deltaTime;
        }
        SPR.color = FS;
    }

    /// <summary>
    /// Change the TextMeshPro text color.
    /// </summary>
    /// <param name="mainText">The text used to change its color</param>
    /// <param name="color">Text color</param>
    /// <param name="duration">Change the text's color to the final color in (time) seconds</param>
    /// <param name="colorFormat">The format use for converting color values (RGBA)</param>
    public static IEnumerator ChangeTextColor(TMP_Text mainText, Color color, float duration, Format colorFormat = Format.Scalar)
    {
        float time = 0f;
        int multiplier = 255;

        color = GetFormattedColor(color, ref multiplier, colorFormat);

        Color OS = mainText.color;
        Color FS = color;

        mainText.color = OS;

        while (time <= duration)
        {
            Color currentColor = OS;

            currentColor.r = Mathf.Lerp(OS.r, FS.r, time / duration);
            currentColor.g = Mathf.Lerp(OS.g, FS.g, time / duration);
            currentColor.b = Mathf.Lerp(OS.b, FS.b, time / duration);
            currentColor.a = Mathf.Lerp(OS.a, FS.a, time / duration);

            mainText.color = currentColor;
            yield return null;
            time += Time.deltaTime;
        }
        mainText.color = FS;
    }

    /// <summary>
    /// Change the color of an Image (RGBA).
    /// </summary>
    /// <param name="img">The image used to change its color</param>
    /// <param name="color">Image color</param>
    /// <param name="duration">Change the image's color to the final color in (time) seconds</param>
    /// <param name="colorFormat">The format use for converting color values (RGBA)</param>
    public static IEnumerator ChangeImageColor(Image img, Color color, float duration, Format colorFormat = Format.Scalar)
    {
        float time = 0f;
        int multiplier = 255;

        color = GetFormattedColor(color, ref multiplier, colorFormat);

        Color OS = img.color;
        Color FS = color;

        img.color = OS;

        while (time <= duration)
        {
            Color currentColor = OS;

            currentColor.r = Mathf.Lerp(OS.r, FS.r, time / duration);
            currentColor.g = Mathf.Lerp(OS.g, FS.g, time / duration);
            currentColor.b = Mathf.Lerp(OS.b, FS.b, time / duration);
            currentColor.a = Mathf.Lerp(OS.a, FS.a, time / duration);

            img.color = currentColor;
            yield return null;
            time += Time.deltaTime;
        }
        img.color = FS;
    }

    /// <summary>
    /// Get the color from the specified format.
    /// </summary>
    /// <param name="color">Initial color</param>
    /// <param name="multiplier">Multiplier used for converting color values based on format</param>
    /// <param name="colorFormat">The format use for converting color values (RGBA)</param>
    private static Color GetFormattedColor(Color color, ref int multiplier, Format colorFormat)
    {
        switch (colorFormat)
        {
            case Format.Standard:
                multiplier = 255;
                break;
            case Format.Scalar:
                multiplier = 1;
                break;
            case Format.Percentage:
                multiplier = 100;
                break;
        }

        Color newColor = new Color(color.r / multiplier, color.g / multiplier, color.b / multiplier, color.a / multiplier);
        return newColor;
    }

    /// <summary>
    /// Method to convert hex code to RGB.
    /// </summary>
    /// <param name="hexCode">Hex code (Eg. "#4287f5")</param>
    /// <returns>Color in RGB format</returns>
    private static Color ConvertHexToRGB(string hexCode)
    {
        int rVal = 0;
        int gVal = 0;
        int bVal = 0;

        if (hexCode.StartsWith("#")) { hexCode = hexCode.Remove(0, 1); }

        string rCode = string.Copy(hexCode).Substring(0, 2);
        string gCode = string.Copy(hexCode).Substring(2, 2);
        string bCode = string.Copy(hexCode).Substring(4, 2);

        // Calculate the actual color value.
        // result = (hex value * 16^1) + (hex value * 16^0)
        for (int index = 0, power = 1; index < 2; index++, power--)
        {
            rVal += CalculateDecimalValue(rCode[index], power);
            gVal += CalculateDecimalValue(gCode[index], power);
            bVal += CalculateDecimalValue(bCode[index], power);
        }
        Color color = new Color(rVal / 255f, gVal / 255f, bVal / 255f);

        return color;
    }

    /// <summary>
    /// Method to check if hex string is written in the correct hex format.
    /// </summary>
    /// <param name="hexString">Hex code (Eg. "#4287f5")</param>
    /// <returns>TRUE if hex code is valid, Otherwise, FALSE</returns>
    private static bool CheckHexFormat(string hexString)
    {
        string validChars = "0123456789abcdef";

        if (hexString.StartsWith("#")) { hexString = hexString.Remove(0, 1); }
        hexString = hexString.ToLower();

        if (hexString.Length == 6)
        {
            for (int i = 0; i < hexString.Length; i++)
            {
                if (validChars.IndexOf(hexString[i]) == -1)
                {
                    Debug.LogError("ERROR: Hex code is not formatted properly!");
                    return false;
                }
            }
            return true;
        }

        Debug.LogError("ERROR: Hex code is too long or too short!");
        return false;
    }

    /// <summary>
    /// Method to calculate the decimal value based on the given hex character.
    /// </summary>
    /// <param name="hexLetter">Hex letter (012345678ABCDEF)</param>
    /// <param name="powerValue">Exponential power</param>
    /// <returns>Decimal value</returns>
    private static int CalculateDecimalValue(char hexLetter, int powerValue)
    {
        hexLetter = char.ToLower(hexLetter);

        switch (hexLetter)
        {
            case '0': return (int)(0 * Math.Pow(16, powerValue));
            case '1': return (int)(1 * Math.Pow(16, powerValue));
            case '2': return (int)(2 * Math.Pow(16, powerValue));
            case '3': return (int)(3 * Math.Pow(16, powerValue));
            case '4': return (int)(4 * Math.Pow(16, powerValue));
            case '5': return (int)(5 * Math.Pow(16, powerValue));
            case '6': return (int)(6 * Math.Pow(16, powerValue));
            case '7': return (int)(7 * Math.Pow(16, powerValue));
            case '8': return (int)(8 * Math.Pow(16, powerValue));
            case '9': return (int)(9 * Math.Pow(16, powerValue));
            case 'a': return (int)(10 * Math.Pow(16, powerValue));
            case 'b': return (int)(11 * Math.Pow(16, powerValue));
            case 'c': return (int)(12 * Math.Pow(16, powerValue));
            case 'd': return (int)(13 * Math.Pow(16, powerValue));
            case 'e': return (int)(14 * Math.Pow(16, powerValue));
            case 'f': return (int)(15 * Math.Pow(16, powerValue));
            default: return '0';
        }
    }


    #endregion

    #region //Easing
    /// <summary>
    /// Method to calculate floating values via Ease-In.
    /// </summary>
    /// <param name="f">float value</param>
    /// <param name="powInt">Exponential power</param>
    /// <returns>Float value</returns>
    private static float EaseIn(float f, uint powInt)
    {
        float result = 1f;
        while (powInt-- != 0)
            result *= f;

        return result;
    }

    /// <summary>
    /// Method to calculate floating values via Ease-Out.
    /// </summary>
    /// <param name="f">float value</param>
    /// <param name="powInt">Exponential power</param>
    /// <returns>Float value</returns>
    private static float EaseOut(float f, uint powInt) => (1 - Mathf.Pow((1 - f), powInt));

    /// <summary>
    /// Method to calculate floating values via Ease-InOut.
    /// </summary>
    /// <param name="f">float value</param>
    /// <param name="powInt">Exponential power</param>
    /// <returns>Float value</returns>
    private static float EaseInOut(float f, uint powInt = 2) => Mathf.Lerp(EaseIn(f, powInt), EaseOut(f, powInt), f);
    #endregion

    #region //Miscellaneous

    /// <summary>
    /// Calculate the vector (used for calculating position, rotation, and scale).
    /// </summary>
    /// <param name="ease">Easing type</param>
    /// <param name="OV">Original vector</param>
    /// <param name="FV">Final vector</param>
    /// <param name="timeRatio">The current time over the duration (time/duration)</param>
    /// <param name="power">Exponential power for easing</param>
    /// <param name="changeX">Allow changes the X value</param>
    /// <param name="changeY">Allow changes to the Y value</param>
    /// <param name="changeZ">Allow changes the Z value</param>
    private static Vector3 CalculateVector(Easing ease, Vector3 OV, Vector3 FV, float timeRatio, uint power, bool changeX, bool changeY, bool changeZ)
    {
        Vector3 vector = OV;

        switch (ease)
        {
            case Easing.Linear:
                if (changeX) { vector.x = Mathf.Lerp(OV.x, FV.x, timeRatio); }
                if (changeY) { vector.y = Mathf.Lerp(OV.y, FV.y, timeRatio); }
                if (changeZ) { vector.z = Mathf.Lerp(OV.z, FV.z, timeRatio); }
                break;
            case Easing.EaseIn:
                if (changeX) { vector.x = Mathf.Lerp(OV.x, FV.x, EaseIn(timeRatio, power)); }
                if (changeY) { vector.y = Mathf.Lerp(OV.y, FV.y, EaseIn(timeRatio, power)); }
                if (changeZ) { vector.z = Mathf.Lerp(OV.z, FV.z, EaseIn(timeRatio, power)); }
                break;
            case Easing.EaseOut:
                if (changeX) { vector.x = Mathf.Lerp(OV.x, FV.x, EaseOut(timeRatio, power)); }
                if (changeY) { vector.y = Mathf.Lerp(OV.y, FV.y, EaseOut(timeRatio, power)); }
                if (changeZ) { vector.z = Mathf.Lerp(OV.z, FV.z, EaseOut(timeRatio, power)); }
                break;
            case Easing.EaseInOut:
                if (changeX) { vector.x = Mathf.Lerp(OV.x, FV.x, EaseInOut(timeRatio, power)); }
                if (changeY) { vector.y = Mathf.Lerp(OV.y, FV.y, EaseInOut(timeRatio, power)); }
                if (changeZ) { vector.z = Mathf.Lerp(OV.z, FV.z, EaseInOut(timeRatio, power)); }
                break;
        }
        return vector;
    }

    /// <summary>
    /// Get vector coordinates based on axis.
    /// </summary>
    /// <param name="axis">Vector axis on a coordinate plane</param>
    private static Vector3 GetAxisVector(Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return Vector3.right;
            case Axis.Y: return Vector3.up;
            case Axis.Z: return Vector3.forward;
            case Axis.XY: return new Vector3(1, 1, 0);
            case Axis.XZ: return new Vector3(1, 0, 1);
            case Axis.YZ: return new Vector3(0, 1, 1);
            case Axis.XYZ: return new Vector3(1, 1, 1);
            default: return Vector3.zero;
        }
    }
    #endregion
}