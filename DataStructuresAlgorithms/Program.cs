using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// public class Solution {
//     public string DefangIPaddr(string address) 
//     {
//         return address.Replace(".", "[.]");    
//     }
// }



public class Solution 
{

    static void Main(string[] args)
    {
        string[] strs = new string[] {"aac","ab"}; 
        Console.WriteLine(LongestCommonPrefix(strs));
    }
    public static string DefangIPaddr(string address) 
    {
        char [] add = address.ToCharArray();
        char [] result = new char [add.Length];
        int index = 0;
        int i = 0;
        foreach (char item in add)
        {
            if (item == '.')
            {
                Array.Resize(ref result, result.Length + 2);

                result[index] = '[';
                index++;
                result[index] = '.';
                index++;
                result[index] = ']';
                index++;
            }
            else
            {
                //Array.Resize(ref add, add.Length + 1);
                
                result[index] = item;
                index++;
            }
            i++;
        }
        return new string(result); 
    }

    public static int StrStr(string haystack, string needle) 
    {
        int needleLength = needle.Length;        
        for (int i = 0; i < haystack.Length - needle.Length; i++)
        {
            for (int j = 0; j >= needleLength; j++) 
            {
                if (haystack[i] != needle[i])
                {
                    break;
                }
                else if (j == needleLength - 1)
                {
                    if (haystack[i] == needle[i])
                    {
                        return i;
                    }
                }
            }
        }
        return -1;
    }
    public static string LongestCommonPrefix(string[] strs) 
    {
        Array.Sort(strs, (x, y) => x.Length.CompareTo(y.Length));
        string smallest = strs[0];
        int minLength = smallest.Length;
        int arrLength = strs.Length;
        string result = "";

        for (int i = 0; i < minLength; i++)
        {   
            string slice = smallest[0..(i+1)];
            for (int j = arrLength - 1; j >= 0; j--)
            {
                if (j == 0 && strs[j][0..(i+1)] == slice)
                {
                    result = slice;
                }
                else if (strs[j][0..(i+1)] != slice)
                {
                    break;
                }
                continue;
            }
        }
        return result;
    }    
}

