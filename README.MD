# GeoViz
## 2D/3D/4D Microparticle Flow Visualization

GeoViz is an application developed by Breda University of Applied Sciences" within the context of the European-funded project ILIAD: Digital Twins of the Ocean.

![Iliad Logo](README_RESOURCES/Iliad.png)



## Features
Within the scope and applications ecosystem of ILIAD, we allow the representation of particles in water in 2D and 3D, with the possibility of controlling the flow of time of the visualization.
By providing your particles' propagation data through a NetCDF file adhering to the accepted structure parsed in [DataLoader.cs](https://github.com/BredaUniversityResearch/MicroparticlesVisualization-ILIAD/blob/dev/Assets/Scripts/Data/DataLoader.cs), you will be able to visualize your particles' simulation and control the time flow.

## Development Setup
If you'd like to contribute to the project, you can do so by creating Pull Requests on the Dev branch.
In order to have a working local development setup, the following requirements must be met:

- [The user must have Unity 2022.3.20f1 installed to open and edit the project.](https://unity.com/releases/editor/archive)
- [The user must set up their API token for Cesium](https://cesium.com/platform/cesium-ion/)
- [The user must have their API token in “Assets/Secrets/OpenWeatherAPIKey.txt” in order to have the weather widget working.](https://openweathermap.org/)
- [The user must clone and pull the submodules to retrieve the Cesium plugin that is being used by the application.]

Two paid Unity plugins are being used in the project, and for their license, their code can’t be made available; for this reason, the user must buy the following plugins and put them in the “Assets/Plugins/GeoViz_Private_Plugins” folder of the project:

- [Odin Inspector and Serializer](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)
- [File Browser PRO](https://assetstore.unity.com/packages/tools/utilities/file-browser-pro-98713)
