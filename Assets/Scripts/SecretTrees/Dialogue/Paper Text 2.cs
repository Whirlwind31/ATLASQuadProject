using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperText2 : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameObject paper;

    // It may be worthwhile to collapse the following variable.
    private readonly string[] contents = new string[12]
    {
        @"THE NEXT PART 
        of this secret quest

        from: the game's developer",
        @"Welcome! I see you had an invitation from the previous tree.

If you weren't already aware, this is a multi part scavenger hunt. Just for you.",
        @"To find your next task, you'll have to solve a small puzzle.

Nothing much.",
        @"""Out of the six buildings currently on the side of the Quad, HALF of them are not actually on the Quad.

They're elsewhere in the U of Illinois.",
        @"Your job: Find the building, out of these three, that is the FARTHEST NORTH in its normal, geograph-ical location.",
        @"Either do a quick Google Search, or guess and check. I prefer the former, but both are fine approaches.""",
        @"[(--)-INVITATION-(--)]

Bring this invitation close to the correct building, and you will be able to walk inside it. :)",
        @"",
        @"No hints this time! Check your preferred map apps to find buildings with a similar shape.
...Actually guessing and checking is most likely easier. try it.",
        @"",
        @"",
        @"The buildings are Wohlers Hall, the Colonel Wolfe school, and the Main Library. One of them being more obscure than the other two."
    };


//(If this message is appearing, the developer messed up the code somewhere.)

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
