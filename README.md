# speg_code
Let's go boys. Sick project on the way. We're designing a crazy RPG game rn, gonna be unreal.

Unity Version: Get your unity on 6000.3.10f1
- Let's try to keep it that way for the rest of the project

## NPC Dialogue Controller Setup and Guide
  # Step 1 — Attach the Script
  
  Select your NPC GameObject (e.g. Old Wizard) in the Hierarchy
  In the Inspector, click Add Component and search for NPCController
  Attach the script
  
  
  # Step 2 — Add the Trigger Collider
  
  With the NPC selected, click Add Component → search for Capsule Collider
  Check Is Trigger
  Set the Radius to around 3 to define the interaction range
  Make sure your Player GameObject has the tag "Player" (click the Player → Tag dropdown at the top of the Inspector)
  
  
  # Step 3 — Create the Interact Prompt UI ("Press E to Talk")
  This floats above the NPC's head in world space.
  
  Right-click your NPC in the Hierarchy → UI → Canvas
  Rename it InteractPromptUI
  In the Inspector, configure the Canvas component:
  
  Render Mode → World Space
  Event Camera → drag in your Main Camera
  
  
  Configure the Rect Transform:
  
  Pos X: 0, Pos Y: 2.5, Pos Z: 0
  Width: 200, Height: 50
  Scale: X: 0.005, Y: 0.005, Z: 0.005
  
  ** these are the base ones that I used, you might need to change them to make it fit
  
  Right-click the Canvas → UI → Text - TextMeshPro
  Configure the Text (TMP):
  
  Text content: [E] - Talk
  Font Size: 36
  Color: White (or any visible color, I did white)
  Alignment: Center horizontally and vertically
  Overflow: set to Overflow so text is never clipped
  
  
  # Step 4 — Create the Dialogue UI
  This is the screen-space panel that shows dialogue lines when talking.
  
  Right-click in the Hierarchy (NOT on the NPC) → UI → Canvas
  Rename it DialogueUI
  Set Render Mode → Screen Space - Overlay
  Right-click the Canvas → UI → Panel
  Right-click the Panel → UI → Text - TextMeshPro
  Configure the Text (TMP):
  
  Make sure you set the background of the panel to as dark as possible
  
  Font Size: 24
  Color: White
  Alignment: Center
  Stretch it to fill the panel using the anchor presets (Alt+click the stretch-all preset in Rect Transform)
  
  
  # Step 5 — Wire Up the Script Fields
  Select your NPC and look at the NPCController component in the Inspector. Fill in every field:
  FieldWhat to drag ininteractPromptUIThe InteractPromptUI Canvas you made in Step 3dialogueUIThe DialogueUI Canvas you made in Step 4dialogueTextThe Text (TMP) inside the DialogueUI panel
  Then fill in the Dialogue Lines array:
  
  Set Size to however many lines you want
  Fill in each element with a line of dialogue, e.g:
  
  "Greetings, traveller."
  "I have been waiting for someone like you."
  "Press E to continue..."
  
  
  
