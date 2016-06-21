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
![](https://github.com/InderPabla/NEAT/blob/master/Images/4.gif)

## Food Seekers
### Single sensor - food seeker 
- Hard coded closest food source as input 
![](https://github.com/InderPabla/NEAT/blob/master/Images/5.gif)

### Nine sensor - food seeker 
- Neural network evolves to learn how use 9 sight sensors
![](https://github.com/InderPabla/NEAT/blob/master/Images/7.gif)

## Double pole balancing with velocities
![](https://github.com/InderPabla/NEAT/blob/master/Images/6.gif)

## Panel System 
- Shows graph, traning completion informer, and the image of the neural net.
![Alt text](https://github.com/InderPabla/NEAT/blob/master/Images/Panel.PNG "Panel System")


