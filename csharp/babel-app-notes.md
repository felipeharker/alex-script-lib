# Babel App Notes

## Purpose

Babel will serve as an ultimate automated sign design tool for Rhino 8. Babel (the app, as it will be called) will take user input linework and provide some options which will generate a 3-dimensional digital model ready to render/visualize. The interface, workflow, and other application details will be outlined in this document.

## User Interface

The application should live in a panel. The app panel will appear similar to Named Views, Layer State Manager, or Layers in that the user will be able to create, update, delete, and select Babel model geometries. Outlined later in this document is a step-by-step interface/workflow for the app/plugin.

## Workflow

### Start

0. User will provide surfaces which will be used in creating the 3d model. This information may live in a layer called "linework" or "surfaces" for example. The user will select the relevant surfaces in the model before activating Babel.

1. User will select "Create New Model" in the Babel panel.

### Sidewalls

2a. App will prompt user to input the depth of the sign sidewalls. this will be a numeric value which corresponds to the distance of the extrusion of the provided surfaces.

// NOTE: A new layer will be created under this path model>sidewalls-a. Extrusions will live here. If the layer "model" is not present, create it and the "sidewalls-x" sublayer. x = the item number/letter for this particular model. when a user creates a new model using babel, the app will create sidewalls-b, -c, etc. etc.

2b. App will prompt user to input the color/material of the sidewalls. user will select a color from an rgb wheel.

// NOTE: the app will set the layer display color based on this user input, and the material will be a default plaster material type with the display color as the material color. the material will be named "sw-a" or "sw-x" depending on which item it is.

### Face

3. app will prompt user for color of the sign face using an rgb wheel. the app will create/use the provided surface (whichever is most sensible) as a face for the sign. the app will then place the face above the extruded sidewalls by 0.03125" (so if the sidewall is 4" the face will be at 4.3125")

// NOTE: app should provide an option for "no front face" where the face of the sign will just be the front of the extrusion. in the case the user selects this, the face prompts are skipped.

// NOTE: app will create a new layer named "face-a" (or -b, -c, etc.) and the display color shall be the user defined color. the material will once again be a default plaster with the user defined color. the material shall be named "face-x" x = item letter.

### Illumination

4. app will prompt user to select sign type. options are below:

a. front-illum
b. reverse-illum
c. non-illum

4a. user selects front-illum

- app will create a new layer named "f-illum-a" and place a new surface in front of/above the extrusion. the surface will be placed 0.0625" from the extrusion of the sign.

- app will prompt user to select a display color. this will be the layer's color. this layer does not have a material.

- app will create a v-ray mesh light named "f-illum-a" with the following settings:

- light color is user defined color
- light intensity = 5
- light is double sided
- light/surface is visible
- light is "on" in vray

4b. user selects reverse-illum

- app will create a new layer named "r-illum-a" and place a new surface behind/below the extrusion. the surface will be placed 0.0625" from the extrusion of the sign (behind it).

- app will prompt user to select a display color. this will be the layer's color. this layer does not have a material.

- app will create a vray mesh light named "r-illum-a" with the following settings:

- light color is user defined color
- light intensity = 5
- light is double sided
- light/surface is visible
- light is "on" in vray

4c. user selects non-illum

- all done here.

### Output

Babel will now have an item (which the user can name anything they want) which is essentially a "group" of the geometries (sidewalls, face, illuminated surfaces) which compose the design. The way this information is defined will be the most effective/efficient way possible within rhino.