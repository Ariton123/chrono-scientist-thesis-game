using System;
using UnityEngine;

[Serializable]
public class LocalizationEntry
{
    public string key;

    [TextArea] public string english;
    [TextArea] public string deutsch;
    [TextArea] public string francais;
    [TextArea] public string italiano;
}