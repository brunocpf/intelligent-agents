# Environments

## [E001]: Switch-triggered Door

### Description
* Type: PPO Reinforcement Learning
* Keywords: pathfinding, collision, switch
* Scenes:
    * Training: SwitchTriggeredDoor_Training
* Set-up: A movement task where an agent must reach a desired position, and that position might be blocked or not by a door. If it is, the agent must first trigger a randomly placed switch to open the door and reach the goal.
* Goal: Reach the desired position.
* Agents: One agent linked to a brain.
* Current Agent Reward Function (ARF): 
    1. +1.0 for arriving at the desired goal.
    2. +0.1 for triggering the switch while in the opposite side of the goal.
    3. -0.001 every step of the simulation (existential penalty).
* Brains: One brain with the following observation/action space.
    * Vector Observation Space (O) - Continuous:
        1.  One variable indicating whether or not the agent is on the side of the door with the goal.
        2.  One variable indicating whether or not the switch was triggered (and the door is open).
        3.  One variable corresponding to the distance between the agent and the switch.
        4.  One variable corresponding to the distance between the agent and the goal.
    * Vector Action Space (A) - Continuous: 
        1. Two variables corresponding to the movement of the agent in the X and Z directions.
    * Visual Observations: None
* Reset Parameters: None
* Notes: None

### Findings
