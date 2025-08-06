using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperText1 : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private GameObject paper;

    // It may be worthwhile to collapse the following variable.
    private readonly string[] contents = new string[19]
    {
        // Don't touch the ASCII text!
        @"A QUAD DAY SCAVENGER HUNT

from: the developer
              next page
                         |
                         | 
                        V",
        @"Hi! Before we begin, you should hold on
tight to this paper.

You're definitely going to need it later.",
        @"If you ever lose this piece of paper, don't worry! Come back to this table and the paper will respawn!

previous page
v",
        @"As you may have realized, VR Quad day is not 100% realistic.
	
For example, I assume you're not used to walking through trees in real life!",
        @"But enough intro; LET'S BEGIN THE SCAVENGER HUNT!!

Here's something cool: This paper is more than just a pamphlet to read.",
        @"it's also the invitation that will unlock PART 2 of the scavenger hunt!
	To use my invitation, listen to these instructions carefully:",
        @"There is a tree, in front of the illini union. 

When you're close by, TURN TO THE PAGE OF THE INVITATION, or it won't work!",
        @"Where is the invitation, you ask?

A: it's on the next page!",
        @"(-|--INVITATION--|-)

Please bring this invitation to the Illini Union Tree to be granted access to pass through.",
        @"

  (This page has been
   intentionally left 
          blank)",
        @"",
        @"",
        @"Hello! i see you noticed there was still more to this pamphlet.

Want a hint? keep reading for Hint 1!",
        @"Hint 1: Make sure you are turned to the page of the invitation when you think you have the right tree.
	If you can't pass through, you may have the wrong tree.",
        @"",
        @"Still stuck? Turn the page for Hint 2!",
        @"Hint 2: The tree you need is actually BEHIND the Illini union. At least, from the quad.
	To be fair, it IS in front of the green st. entrance. :)",
        @"",
        @"(If you're still stuck, why not ask a friend where they think the illini union tree is?)"
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
