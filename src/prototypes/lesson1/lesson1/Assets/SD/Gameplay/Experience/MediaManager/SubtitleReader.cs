﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.IO;

public class SubtitleReader : MonoBehaviour
{
    private Hashtable subtitlesLastSeconds;
    private string pathSubs = "/lesson1-data/subs/";

    private Hashtable dialogs;

    public SubtitleReader()
    {
        subtitlesLastSeconds = new Hashtable();
        dialogs = new Hashtable();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RestFileReader(string videoName, string videoPath)
    {
        try
        {
            //VRExperience experience = VRExperience.Instance;
            string subtitleText = null;
            subtitlesLastSeconds = new Hashtable();
            dialogs = new Hashtable();
            string fileName = videoName.Replace("mp4", "txt");
            StreamReader subtitles = new StreamReader(videoPath + fileName);
            int lineCounter = 0;
            //guardo todos los ultimos segundos de los subtitulos y su texto
            while (!subtitles.EndOfStream)
            {
                DialogType dialogType = new DialogType();
                string line = subtitles.ReadLine();
                int firstBracket = line.IndexOf("[") + 1;
                int lastSquareBracket = line.IndexOf("]") - 1;
                int lastInterval = int.Parse(line.Substring(firstBracket, lastSquareBracket));
                // Add to hasmap last second interval. 

                dialogType.Start = lastInterval;
                subtitlesLastSeconds.Add(lineCounter, lastInterval);

                //agregar texto a otro hashMap
                int parent = line.IndexOf("]") + 1;
                int dash = line.IndexOf("|");
                int between = dash - parent;
                subtitleText = line.Substring(parent, between);

                if (subtitleText.Contains("&I"))
                {
                    int iInput = subtitleText.IndexOf("&I")+2;
   
                    string[] answers = subtitleText.Substring(iInput, subtitleText.Length- iInput).Split('#');
                    
                    foreach (string answer in answers){
                        Debug.Log("SubtitleReader. answer" + answer);
                        dialogType.Answers.Add(answer);
                    }
                    dialogType.RequiredInput = true;
                    dialogType.Text = subtitleText.Split('&')[0];
                   // experience.CountQuestionsToAnswer += 1;
                }
                else if (subtitleText.Contains("&P"))
                {
                    dialogType.Listen = true;
                    dialogType.Text = subtitleText.Split('&')[0];
                }
                else if (subtitleText.Contains("&E"))
                {
                    dialogType.Finish = true;
                    dialogType.Text = subtitleText.Split('&')[0];
                }
                else
                {
                    dialogType.Text = subtitleText;
                }

                dialogs.Add(lineCounter, dialogType);
                lineCounter++;
            }
            subtitles.Close();

        }
        catch (System.Exception ex)
        {
            Debug.Log("SubtitleReader. ERROR_OBTENIENDO_SUBTITULOS: " + ex.Message);
        }

    }


    /*public void RestFileReader(string videoName)
    {
        try
        {
            subtitlesLastSeconds = new Hashtable();
            dialogs = new Hashtable();
            string fileName = videoName.Replace("mp4", "json");
            StreamReader subtitles = new StreamReader(Application.persistentDataPath + pathSubs + fileName);
  
            //guardo todos los ultimos segundos de los subtitulos y su texto
            while (!subtitles.EndOfStream)
            {
                DialogType dialogType = new DialogType();
                string line = subtitles.ReadToEnd();
                Debug.Log("SubtitleReader. json: " + line);
                JSONObject j = new JSONObject(line);
                accessData(j);
            }
            subtitles.Close();

        }
        catch (System.Exception ex)
        {
            Debug.Log("SubtitleReader. ERROR_OBTENIENDO_SUBTITULOS: " + ex.Message);
        }

    }*/

    /*public static System.String toJSon(DialogType dialog)
    { 
            // Here we convert Java Object to JSO
            JSONObject jsonObj = new JSONObject();
            jsonObj.AddField("text", dialog.Text); // Set the first name/pair
            jsonObj.AddField("surname", dialog.Start);
           
            return jsonObj.Print();
    }*/
    
    /*void accessData(JSONObject obj)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    Debug.Log("SubtitleReader. " + key);
                    accessData(j);
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    accessData(j);
                }
                break;
            case JSONObject.Type.STRING:
                Debug.Log("SubtitleReader. " +obj.str);
                break;
            case JSONObject.Type.NUMBER:
                Debug.Log("SubtitleReader. " + obj.n);
                break;
            case JSONObject.Type.BOOL:
                Debug.Log("SubtitleReader. " + obj.b);
                break;
            case JSONObject.Type.NULL:
                Debug.Log("SubtitleReader. " + "NULL");
                break;

        }
    }*/

    /* public Hashtable ReadSubtitleLine(long duration)
     {
         Hashtable subtitleToReturn = new Hashtable ();
         int intDuration = (int)duration;
         ICollection hashValuesLast = subtitlesLastSeconds.Values;
         int howMany = hashValuesLast.Count;
         int[] lastSeconds = new int[howMany];
         hashValuesLast.CopyTo(lastSeconds, 0);

         ICollection hashValuesSubs = dialogs.Values;
         string[] subs = new string[howMany];
         hashValuesSubs.CopyTo(subs, 0);
         // search if duration is in last subtitle second of the hash
         for (int i = 0; i < howMany; i++)
         {

             if (lastSeconds[i] < intDuration)
             {
                 //devuelvo subtitulo
                 if (i == howMany - 1) {
                     if (subtitleToReturn.ContainsKey (1)) {
                         subtitleToReturn [1] = subs [i];
                     } else {
                         subtitleToReturn.Add (1, subs [i]);
                     }
                 } else {
                     if (subtitleToReturn.ContainsKey (0)) {
                         subtitleToReturn [0] = subs [i];
                     } else {
                         subtitleToReturn.Add (0, subs [i]);
                     }
                 }
             }
         }

         return subtitleToReturn;
     }*/

    public DialogType ReadSubtitleLine(long duration)
    {
        try
        {
            DialogType subToReturn = null;
            int intDuration = (int)duration;
            ICollection hashValuesLast = subtitlesLastSeconds.Values;
            int howMany = hashValuesLast.Count;
            int[] lastSeconds = new int[howMany];
            hashValuesLast.CopyTo(lastSeconds, 0);

            ICollection hashValuesSubs = dialogs.Values;
            DialogType[] subs = new DialogType[howMany];
            hashValuesSubs.CopyTo(subs, 0);
            // search if duration is in last subtitle second of the hash
            for (int i = 0; i < howMany; i++)
            {

                if (lastSeconds[i] < intDuration)
                {
                    //devuelvo subtitulo
                    subToReturn = subs[i];
                }
            }

            return subToReturn;

        }
        catch (System.Exception ex)
        {
            Debug.Log("ERROR_OBTENIENDO_SUBTITULOS: " + ex.Message);
        }
        return null;
    }

    public bool IsLastSubtitle(Hashtable hashsub)
    {
        return hashsub.ContainsKey(1);
    }

}