# GeoViz
## 2D/3D/4D Microparticle Flow Visualization

GeoViz is an application developed by Breda University of Applied Sciences1" within the context of the European-funded project ILIAD: Digital Twins of the Ocean.

![Iliad Logo](README_RESOURCES/Iliad.png)



## Features
Within the scope and applications' ecosystem of ILIAD, we allow the representation of particles in water in 2D and 3D, with the possibility of controlling the flow of time of the visualization.
By providing the propagation data of your particles (we currently adhere to the output of OpenDrift, providing Latitude, Longitude, Depth and Diameter of the particles in time-series), using the Ocean Information Model developed within ILIAD, adhering to the json format provided by said model, you will be able to visualize the simulation of your particles and control the time flow.

## Notes:

- This application is still a work in progress.
- The intended date of release of a first MVP will probably be around the end of February.


## Development Setup
If you want to contribute to the project you can by creating Pull Requests on the Dev branch.
This project uses paid plugins from the Unity Asset Store, that, of course, we're not allowed to share publicly into a public repository.

In order to have the project loading without errors in Unity you'll have to buy the following plugins:
- [Odin Inspector and Serializer](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)

and put it inside Assets/Plugins/Geoviz_Private_plugins.