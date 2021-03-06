# World Domination
Remake of the great game Zarch (by David Braben, 1987) using the Unity game engine.

<p align="center"><img src="https://raw.githubusercontent.com/till213/WorldDomination/master/doc/img/WorldDomination-2018.04.png" alt="World Domination screenshot" width="640"></p>

# Introduction

Zarch - probably better known under the name Virus (Amiga, Atari ST, PC and others) - was one of the first 3D games featuring filled 3D polygon graphic. It featured a lander which was controlled by mouse movements, moving only up by a thruster and pulled down (very quickly so) by gravity. Or simply turn the lander upside down and "thrust downwards".

The mission was to prevent other drones from spreading their virus (illustrated by an impressive particle system) by shooting them down - before your fuel ran out or you simply hit the ground hard.

# Why a Remake?

Yes, that's not the first remake. But I had this one in mind since I graduated, and eventually got another motivation push by using Unity, a game engine which provides physics, input controls, sound and last but not least graphics, including shader support.

So my main motivation is to dig into compute shaders, graphic shaders in general, physics and - who knows - eventually a multiplayer network support.

# Progress

The project is at its very beginning. Very basic terrain rendering using a geometry shader and using a sinus-based terrain generation is in place. Very crude physics let you control the placeholder ship in any direction, teleporting you to the opposite border each time you "fly off" over the border.

Next up:

* Work on the terrain details and colours
* Replace the geometry shader by either a compute shader, or a "moving height map texture" based vertex shader (in order to get Metal support on Mac)

# Credits

The original game Zarch / Virus by David Braben, 1987.

# License

The code is under MIT license.

# References

* https://en.wikipedia.org/wiki/Zarch
* https://www.myabandonware.com/game/virus-jx

Other people porting code - including optimising the original Zarch code :)

* People optimising the original code :) http://stardot.org.uk/forums/viewtopic.php?f=29&t=10313

* Another developers discussion: https://forum.thegamecreators.com/thread/176733
* Actual game footage: https://www.youtube.com/watch?v=xrN2soK60bA
* More about the original Zarch: https://wikivisually.com/wiki/Zarch
