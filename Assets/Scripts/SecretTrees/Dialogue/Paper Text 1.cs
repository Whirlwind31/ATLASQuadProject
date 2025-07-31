using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperText1 : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameObject paper;

    // It may be worthwhile to collapse the following variable.
    private readonly string[] contents = new string[14]
    {
        // Don't touch the ASCII text!
        @"SECRET TEXT

        from: the game's developer
              next page
                         |
                         | 
                        V",
        @"Hi! You should probably hold on tight to this piece of paper.
       
Don't want to accidentally lose it, do we?",
        @"What brings you here?

I assume you're not used to walking through trees?

previous page
v",
        @"Since you made it all the way here, I don't want to leave you empty-handed!

The following page contains an invitation.",
        @"Once you reach the tree near the Illini Union, make sure to TURN TO THE PAGE OF THE INVITATION and presto! The tree will unlock for you.",
        @"(-|--INVITATION--|-)

Please bring this invitation to the Illini Union Tree to be granted access to pass through.",
        @"

  (This page has been
   intentionally left 
          blank)",
        @"",
        @"",
        @"",
        @"Curious, are we? Sorry, the hint from before is the only one you get.",
        @"",
        @"",
        @"(Okay, the 'Illini Union Tree' isn't actually where you think it is.)"
    };

    // Update is called once per frame
    void Update()
    {
        PageNo scr = paper.GetComponent<PageNo>();

        if (scr.pageNumber > 0 && scr.pageNumber <= contents.Length)
        {
            dialogue.text = contents[scr.pageNumber - 1];
        }
        else
        {
            dialogue.text = "";
        }
    }

    
}
