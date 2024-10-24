# Project 2 Report

Read the [project 2
specification](https://github.com/feit-comp30019/project-2-specification) for
details on what needs to be covered here. You may modify this template as you see fit, but please
keep the same general structure and headings.

Remember that you must also continue to maintain the Game Design Document (GDD)
in the `GDD.md` file (as discussed in the specification). We've provided a
placeholder for it [here](GDD.md).

## Table of Contents

- [Evaluation Plan](#evaluation-plan)
- [Evaluation Report](#evaluation-report)
- [Shaders and Special Effects](#shaders-and-special-effects)
- [Summary of Contributions](#summary-of-contributions)
- [References and External Resources](#references-and-external-resources)

## Evaluation Plan

**Evaluation Techniques**

Which evaluation techniques will you use and why? What tasks will you ask participants to perform?
- In-person
  - Observational technique - cooperative evaluation
  - Tasks need to cover various areas of the game
    - Level design/navigation – can the user find their way through the levels; do they understand what they are expected to do?
    - Controls – can the user understand and use the controls properly; are they intuitive?
    - Mechanics – does the user understand how the different objects and items in the game function; do they need to be explained?
  - Initially simple tasks
    - Switch through inventory
    - Use weapon
    - Open door
    - Walk forward
    - Etc.
  - Progressing to more complex tasks as the evaluation continues
    - Use the sacrifice circle to open the door
    - Use the crossbow to kill an enemy
    - Use the GobEye to view into the next room
    - Etc.
  - Eventually, hopefully the player can play through with less input from experimenter and just describe their experience

- Online
  - Post-gameplay questionnaire
  - Ask players to complete a single, complete play-through before filling out survey
  - Provide a walkthrough/explanation video if they get stuck
  - Some fixed and some open-ended questions for participants
  - Fixed/quantitative
    - Likert scale (1-5, strongly disagree to strongly agree)
  - Open-ended/qualitative
    - Favourites and least favourites
    - Improvement suggestions
    - Etc.


**Participants**

How will you recruit participants? What qualifying criteria will you use to ensure that they are representative of your target audience?
- Some participants recruited from personal connections (friends and family)
  - Due to time and location limitations these will likely be the in-person test group
- Prioritise people who are unfamiliar with the game, but familiar with games in general, especially stealth/fantasy games
- Target audience is:
  - Age: 16-35
  - Enjoys fantasy and stealth games
  - Intermediate to advanced skill level
  - Familiar with puzzle games/mechanics
  - Likes challenging puzzle/stealth games


**Data collection**

What sort of data is being collected? How will you collect the data? What tools will you use?
- In-person
  - Initially, entire conversation + gameplay recorded
    - Record video of gameplay and audio of player/experimenter conversation
  - OBS Studio used for video recording
  - Audacity for audio recording
  - Get the player to jump and say “jump” verbally to sync recordings

- Online
  - Use surveys (Google Forms) on online forums (r/unimelb subreddit, r/gaming, etc.) as well as posted on notice boards around the University
  - Personal information (to verify target audience)
    - Age
    - Gaming experience
    - Favourite genre of games
    - Etc.
  - Quantitative
    - Likert scale rating (rate from strongly disagree (1) - strongly agree (5))
    - Questions are actually statements, users rate how strongly they agree
    - E.g. The controls were intuitive and easy to use, the mechanics were well-fleshed out and interesting, etc.
  - Qualitative
    - What was your favourite feature of the game?
    - What was your least favourite feature of the game?
    - What is the most important thing you think we should fix/improve?
    - Were the controls easy to understand?
    - Were there any sections of the game you found overly difficult? If so, where and how come?
  - Survey: https://forms.gle/3ku3bfxjhWBWn1sA9


**Data analysis**

How will you analyse the data? What metrics will you use to evaluate your game, and provide a basis for making changes?
- In-person playthrough
  - Separate feedback/issues into groups by
    - Topic/component of game
    - Level design
    - Mechanics
    - Controls
    - Character/environment design
  - Location
    - Level 1 - spawn room
    - Level 2 - final room
    - Etc.
  - Compare these groups for multiple users to identify repeating issues/trends for users

- Online survey
  - Simple statistical measures can be used for the quantitative questions to observe trends in the data
    - Filter to look at target audience vs non-target audience
    - Calculate mean/median/range etc. for our Likert scale ratings (1 = strongly disagree, 5 = strongly agree) and look for consistent trends across surveys and questions
  - Group and analyse similar open-ended responses to identify key trends in the qualitative data
    - Same as in-person, group by component/section
    - Note the key feedback from each survey response


**Timeline**

What is your timeline for completing the evaluation? When will you make changes to the game?
- Evaluation will begin as soon as the evaluation demo is complete (4-7 Oct)
- Due to time constraints, work on the provided feedback will begin at latest by the 16th of October to ensure that there is time to implement the changes
- Feedback/evaluation process is 100% finished by 20 Oct
- Work on game feedback is 100% finished in time for submission by 27th Oct


**Responsibilities**

Who is responsible for each task? How will you ensure that everyone contributes equally?
- Every member of the group needs to recruit AT LEAST 2 people for an in-person cooperative evaluation (we need at least 5 participants)
- The online survey will be created by Zach and reviewed by all members before going live
- Frequent meetings through Discord where everyone shares and discusses their evaluation work to ensure equal contribution


## Evaluation Report


## Shaders and Special Effects

**Shader 1: Horror Post Processing Shader**
- [Link to ShaderCode](Assets/Shaders/HorrorPostProcessingShader.shader)
- [Link to Associated Script Code](Assets/Shaders/HorrorPostProcessingScript.cs)

Note: The specific implementation of this shader (i.e. what each parameter is used for) is explained in the code in comments (mainly in the shader code).

Shader Effects:
<p align="center">
  <img src="Images/Shaders_ParticleEffects/HorrorShaderGif.gif" width="600">
</p>

This shader aims to produce ‘horror’ effects reminiscent of surveillance camera footage commonly seen in horror films. The shader creates visual distortions, glitch effects, RGB splitting and other effects, which mimic the eerie, unreliable look of a malfunctioning camera. This enhances the atmosphere and adds an element of dread and tension, immersing the player in a creepy, unsettling environment.

How the shader effects fit into the rendering pipeline:
The shader processes the image that the camera has finished rendering, but before it is actually shown on the screen. It adds ‘special effects’ to the image.

Some effects are done in the vertex shader (distortion, jitter, tearing) while others (RGB split, Noise overlay, scanning bars, glitch effect, fog effect) are done in the fragment shader.
This is because:
1. The vertex shader modifies the vertex positions, and effects like distortion and others change the shapes or positions of objects.
2. The fragment shader handles pixel-level details. The effects mentioned above do not change the shapes or positions of objects. They only affect the appearance of each pixel; their colour, transparency, or overlays.

How the parameters are set:
Parameters such as wave distortion, fog density, and RGB split are set in the shader’s associated C# script using material.SetFloat() or material.SetTexture(). These parameters are set each time OnRenderImage() method is called (i.e. just before displaying the image on screen).

Why parameters are set this way (i.e. from a script):
These parameters allow real-time adjustments from the Unity Inspector (during runtime), so it is much easier to tweak these values to the desired amount.


Shader 2: ?


Particle Effect: ?


## Summary of Contributions

TODO (due milestone 3) - see specification for details

## References and External Resources

TODO (to be continuously updated) - see specification for details
