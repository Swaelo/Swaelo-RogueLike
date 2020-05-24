// ================================================================================================================================
// File:        ListFunctions.cs
// Description:	List Shuffle taken from https://www.programming-idioms.org/idiom/10/shuffle-a-list/1352/csharp
//              List Copy taken from https://stackoverflow.com/questions/15330696/how-to-copy-list-in-c-sharp
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

public static class ListFunctions
{
    //Shuffles the list
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random(UnityEngine.Random.Range(1, 1000000));
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static List<T> Copy<T>(this List<T> ListToCopy)
    {
        return ListToCopy.ToList();
    }
}
