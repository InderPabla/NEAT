# NEAT : Neuroevolution of augmenting topologies
I wrote NEAT for C# and tested it in Unity3D enviroment. 

## Creature learns to to maximize distance travelled without dieing. 
- If creature touches the wall it does immediately
- If creature gets too near another creature it starts taking damage and does onces damage reaches 100 
- Creature is rewarded higher for traveling large distances (Position points are taken every second). 
- Creature is rewarded for having lower number of genetic connections (this proves better in the long run, simplest nets evolve)

![](https://github.com/InderPabla/NEAT/blob/master/Images/2.gif)
![](https://github.com/InderPabla/NEAT/blob/master/Images/3.gif)
![](https://github.com/InderPabla/NEAT/blob/master/Images/1.gif)

## Quadrupedal creature
- 12 inputs consisting of, 8 joint angles and 4 touch sensors
- 8 outputs which give motor control to the joints 
- Fitness is calculated based on (distance travelled * average speed)
![](https://github.com/InderPabla/NEAT/blob/master/Images/4.gif)

## Food Seekers
### Single sensor - food seeker 
- Hard coded closest food source as input 
- Flocking behaviour can be observed as the local group of food seekers share the same food source as input 
![](https://github.com/InderPabla/NEAT/blob/master/Images/5.gif)

### Nine sensor - food seeker 
- Network evolves to learn how use 9 sight sensors
- Lack of flocking behaviour due to the fact that food seeker can use multiple sensors to choose which direction to go
![](https://github.com/InderPabla/NEAT/blob/master/Images/7.gif)

### Nine sensor - advanced food seeker 
- Network learns to distinguish between food and wall 
- Touching the wall kills the seeker immediately and sets the fitness to 0, thus making it crucial to not get near the wall. 
- Networks must create an internal model of what it means to see a wall, another seeker, and food. 
![](https://github.com/InderPabla/NEAT/blob/master/Images/8.gif)

## Double pole balancing with velocities
![](https://github.com/InderPabla/NEAT/blob/master/Images/6.gif)

## Panel System 
- Fitness graph.
- Information display corresponding to the graph
- Traning progress bar
- Visual display of the neural network
![Alt text](https://github.com/InderPabla/NEAT/blob/master/Images/9.PNG "Panel System")


