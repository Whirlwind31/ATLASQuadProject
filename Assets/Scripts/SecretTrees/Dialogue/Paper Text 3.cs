using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperText3 : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameObject paper;
    private int checklistStage = 0;


    // It may be worthwhile to collapse the following variable.
    private string[] contents = new string[8]
    {
        @"THE FINAL LEG OF YOUR JOURNEY

        from: the game's developer",
        @"Good job solving that puzzle! Are you ready for one more round?

Complete this scavenger hunt, and you will discover...",
        @"...the location of the final secret of the Quad! 

Are you ready? Then turn the page to figure out what fun lies in store for you!",
        @"S C A V E N G E R
               H U N T

Your task: Unscramb- le the locations on the checklist. Enter them through the front.            More...",
        @"S C A V E N G E R
               H U N T

Once you get close enough to the CENTER of the building, turn to the checklist page to mark it off.",
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
        contents[5] = checklistIterations[checklistStage];

        // If end of scavenger hunt has been reached
        if (checklistStage >= 5)
            contents[7] = bigReveal;
    }


    private readonly string[] checklistIterations = new string[6]
    {
        @"S C A V E N G E R
               H U N T
-[ ] soyne batbolroy
-[ ] sherlow lahl
-[ ] goose statue
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[ ] sherlow lahl
-[ ] goose statue
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[x] wohlers hall
-[ ] goose statue
-[ ] collion tubs",
        @"S C A V E N G E R
               H U N T
-[x] noyes laboratory
-[x] wohlers hall
-[x] yep, goose statue
-[ ] collion tubs",
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

    