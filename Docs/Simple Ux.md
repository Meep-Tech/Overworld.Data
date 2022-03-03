# Simple Ux
Simple Ux is a simple User Experience/Interface and Form Builder built in to Overworld.

## Features
- Quickly make intractable and complex User Interfaces and Experiences with a builder in C#
- Get and Set functionality for all Fields in the UX. 
- Lists of Fields. 
- Key Indexed Lists of Fields. 
- Simpe Rows and Columns layout Elements available. 
- Reflection based auto-generation of UX for simple Objects and X-Bam Model Components.
- C# Field and Property Attributes for use with auto-generation of UXes. 
### Planned
- [ ] OWS Builder, Display, and Variable Retrieval support for Views. 
- [ ] Custom Pannel and View Bakcgroud images or Colors. Sliced, Stretched, and Absolute positioning available. 
- [ ] GetValueFromListByKry function for string and int based lists in C#
- [ ] Implementation of Rows within Columns

## Elements
Elements are the puzzle pieces that make up a Simple Ux View. 
### View
A view is a container for holding other simple UX elements. 
You can think of it as the window for the UX itself. 

Views themselves can only contain Pannels, and each view contains at least one Pannel. 
Pannels are displayed by default as vertical Tabs with labels on the left side of the View, and which Pannel is active can be changed though these tabs. 
Views can have a main Title placed at the top of the View window.
### Pannel
Pannels are containers for elements within a view. A pannel takes up the whole area of the view except for the Pannel Tabs area on the left. The Pannel Tabs area is use to change which Pannel's contents are actively displayed by the View. 

Pannels themselves can only contain Columns, and by default always contain at least one column with all of their contents. 

A pannel can have, at most, 3 Columns, and will display dividers between columns whenever there is more than one. 
#### Pannel Tab
A Pannel Tab is a Title and Button like element used to change the active Pannel in a View. They are displayed on the left of a View in the Pannel Tab area in a vertical list. 

If there is only one pannel in the View, the Pannel Tab area is hidden by default.
### Column
Columns seperate Pannels into vertical slices. A Pannel can have up to three Columns, and always has at least one by default.
Columns can contain any number of Rows and Fields.
A Column can have a Title at the top as well.
### Row
Rows seperate Columns into horizontal slices. A Column can have any number of Rows, and a Row can have any number of Field Elements, though no more than 3 is usually recomended.
Rows can have a Title that appears as a label to the left of the Row's contents as well.
### Field
#### Title 
#### Field List
#### Key Value Field List