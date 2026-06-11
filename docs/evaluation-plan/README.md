# Thesis Report Context and Evaluation Plan

## Overview

This document summarizes the current report context, chapter structure, design framework, and evaluation plan for the master thesis project **Chrono-Scientist: Bones of Meaning**.

The thesis is structured as a Computer Science master thesis with interdisciplinary content from archaeology, history, biology, anatomy, Human-Computer Interaction, educational technology, and game-based learning.

The main contribution lies in the design, implementation, logging, deployment, and evaluation of an interactive educational game prototype for children in a museum learning context.

## Thesis Report Structure

The current thesis report is planned around the following structure:

1. **Introduction**
   Presents the motivation, problem statement, research aims, contributions, and interdisciplinary positioning of the work.

2. **Educational and Technological Context**
   Discusses educational games, museum learning, gamification, child-oriented interaction, and comparable systems.

3. **The Interdisciplinary Knowledge Framework**
   Explains the biological/anatomical and archaeological/historical foundations of the project. The astragalus/talus bone is presented as a central educational object connecting movement, anatomy, history, symbolism, and cultural use.

4. **Audience-Centered Design and Requirements**
   Defines the target audience of children aged 8–12, museum constraints, educational goals, functional requirements, and usability expectations.

5. **Experience and Interaction Design**
   Describes the story, aesthetics, user interface, interaction flow, feedback, reward system, and child-oriented design decisions.

6. **System and Gameplay Implementation**
   Presents the Computer Science implementation of the prototype, including Unity, C#, system architecture, drag-and-drop mechanics, reward logic, data logging, asset production pipeline, and deployment strategy.

7. **Evaluation Design and Methodology**
   Defines the evaluation strategy through system-centered and user-centered metrics, including task completion, completion time, mistakes, retries, Time Completion Ratio, observation data, pre-/post-game questionnaires, and Game Experience Questionnaire data.

8. **Evaluation Results and Analysis**
   Presents the results from the user-testing rounds, system logs, questionnaires, and observational data. It also includes comparative positioning against a similar educational or museum-game system as a secondary part of the evaluation.

9. **Discussion and Implications**
   Interprets the results, discusses limitations, and explains implications for educational museum games.

10. **Conclusion and Future Work**
    Summarizes the contribution and outlines future development directions.

## Design Framework: Game Design Tetrad

The thesis uses the **game design tetrad** as a design framework.

The tetrad consists of:

* story
* aesthetics
* game mechanics
* technology

In this project, the tetrad is used to explain how the narrative of Darwin's laboratory and the Chrono Engine, the 2.5D child-friendly visual style, the drag-and-drop educational mechanics, and the Unity-based implementation work together as one coherent learning experience.

## Evaluation Plan

The evaluation is designed as an iterative three-round study. Its main purpose is to validate the prototype from both a user-centered and system-centered perspective.

Across all rounds, the evaluation is expected to include approximately 40 participants or play sessions in total.

## Main Evaluation Focus

The evaluation combines:

* observation sheets
* pre-game questionnaires
* post-game questionnaires
* Game Experience Questionnaire items
* CSV interaction logs
* qualitative comments from participants
* comparison of results across evaluation rounds

## User-Centered Evaluation

The user-centered evaluation focuses on:

* engagement
* attention
* smiling or positive reactions
* visible confusion
* asking for help
* willingness to continue or replay
* reaction to the reward card
* educational learning through pre-/post-game comparison

The learning component includes key concepts such as:

* scapula
* femur
* pelvis
* astragalus/talus
* amulet
* Charles Darwin

## System-Centered Evaluation

The system-centered evaluation uses CSV logs from the Unity prototype.

The logs record:

* stage completion
* completion time
* mistakes
* retries
* stage failures
* reward rank
* Golden Astragalos achievement
* interaction behavior during gameplay

These logs allow the prototype to be analyzed not only as an educational experience, but also as an implemented interactive system.

## Three-Round Testing Plan

### Round 1 — Formative Museum Pilot

The first round was conducted at the museum trial in La Tour-de-Peilz.

It included 10 play sessions, mainly with children. Data was collected through observation sheets, pre-/post-game questionnaires, Game Experience Questionnaire responses, and CSV logs.

The goal was to identify usability, learning, timing, UI, and reward-system issues.

### Round 2 — Controlled Test with Family and Close Circle

The second round will continue the planned evaluation procedure with approximately 10–15 participants from the family and close circle.

Participants will be considered across different age groups, including children and adults, in order to observe both target-user feedback and broader usability feedback.

The goal of this round is to collect additional user-centered and system-centered data before the final museum validation round.

### Round 3 — Final Museum Validation

The third round is planned as the final validation round at the Natural History Museum in Fribourg, once the date is confirmed.

It is expected to include approximately 15 additional sessions, mainly with children in the intended museum context.

Further improvements identified through the evaluation process may be discussed afterward as limitations, design implications, and future work.

## Additional Comparative Evaluation

A smaller part of the evaluation will compare the prototype with related educational games, museum interaction systems, or comparable platforms.

This comparison is secondary to the three user-testing rounds, but it helps position the thesis contribution in relation to existing work.

## Status

This document represents the current thesis report context and evaluation plan.

It is preserved in the repository as part of thesis documentation, evaluation planning, and final project traceability.
