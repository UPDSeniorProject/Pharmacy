using UnityEngine;
using System.Collections;

public class Util  {

	public static string RemoveHtmlCharacters(string st) 
    {
        return st.Replace("&apos;", "'");
    }
}
