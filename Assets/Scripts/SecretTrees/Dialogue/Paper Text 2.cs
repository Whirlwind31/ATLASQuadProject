using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperText2 : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameObject paper;

    // It may be worthwhile to collapse the following variable.
    private readonly string[] contents = new string[17]
    {
        @"THE NEXT PART 
        of this multi part scavenger hunt

        from: the game's developer",
        @"Welcome! Nice job solving the previous puzzle.

Are you ready for one more puzzle? A little bit harder this time.",
        @"Before we begin, quick reminders that:
- If you ever lose this piece of paper, come back to this table.
- There are some hints later in this pamphlet if you need them!",
        @"Here's the puzzle:

""As you might have noticed, THREE of the buildings in this vr quad...are not on the quad in real life.""",
        @"I will reveal ONE of these buildings to you.

The building with the GATED ENTRANCE, for ""children's safety,"" is your next destination.""",
        @"And what will you bring to this building? 

Why, another invitation of course! It's right on the next page.",
        @"[(--)-INVITATION-(--)]

Bring this invitation close to the correct building, and you will be able to walk inside it. :)",
        @"",
        @"",
        @"Need a hint? Turn the page for HINT 1!",
        @"HINT 1:

How does the developer know that such an oddly specific detail (for ""children's safety"") is on the gate of the building?",
        @"",
        @"Still stuck? Turn the page for HINT 2!",
        @"HINT 2:

The building you're looking for is pretty tiny. relatively speaking, of course.",
        @"",
        @"The next page contains the answer.

once you've solved the puzzle, or if you truly give up, turn the page!",
        @"in real life, the building's name is the colonel wolfe school.

It's the building right next to that big lincoln bust."
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
