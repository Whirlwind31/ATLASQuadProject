using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using TMPro;
using UnityEngine;

public class PaperText3 : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameObject paper;
    private int checklistStage = 0;


    // It may be worthwhile to collapse the following variable.
    private string[] contents = new string[11]
    {
        @"THE FINAL LEG OF YOUR JOURNEY

        from: the game's developer",
        @"Good job solving that puzzle! you know the drill by now. 
	find the correct building and reach the next part of my scavenger hunt.",
        @"But this is the final round, and so it has a TWIST.

All of the location names...have been SCRAMBLED! ",
        @"Complete THIS scavenger hunt, and you'll unlock...


...the location of the FINAL SECRET of the quad!!",
        @"Are you ready? Then turn the page to figure out what fun lies in store for you!",
        @"S C A V E N G E R
               H U N T

Your task: Unscramb- le the locations on the checklist. Then, go to that specific location.",
        @"S C A V E N G E R
               H U N T

For larger buildings, you may have to search around a little bit to find the correct room!",
        @"S C A V E N G E R
               H U N T

The next page has a checklist. Once you find a red box, TURN TO the checklist page to mark it off!",
        @"S C A V E N G E R
               H U N T
-[ ] soyne batbolroy
-[ ] sherlow lahl
-[ ] goose statue
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T

Once you enter all four locations, the next page will unlock the FINAL SECRET of the quad! :)",
        @" Secret of the quad:


          ????"
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


    // Advances the checklist stage when moving close to particular locations.
    // Returns: Whether checklist is complete.
    public void AdvanceChecklist()
    {
        if (checklistStage < 5)
            checklistStage++;
        contents[8] = checklistIterations[checklistStage];

        // If end of scavenger hunt has been reached
        if (checklistStage >= 5)
            contents[10] = bigReveal;
    }


    private readonly string[] checklistIterations = new string[6]
    {
        @"S C A V E N G E R
               H U N T
-[ ] soyne batolroy (hint: it's a building)
-[ ] sherlow lahl
-[ ] goose statue
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[ ] sherlow lahl (hint: it's not on the quad in real life)
-[ ] goose statue
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[x] wohlers hall
-[ ] goose statue (hint: i wonder if this needs unscrambling.)
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[x] wohlers hall
-[x] yep, goose statue
-[ ] collion tubs (hint: the big one)",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[x] wohlers hall
-[x] yep, goose statue
-[?] lincoln bust
almost there!!",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[x] wohlers hall
-[x] yep, goose statue
-[x] lincoln bust
HUNT COMPLETE! Now turn the page...",
    };

    private readonly string bigReveal = @"

     YOU HEARD A
          CLICK
      FROM INSIDE
  THE MAIN LIBRARY!";

}

    