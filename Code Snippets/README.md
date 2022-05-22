# Code Snippets
This document is the explanation behind each code snippet.

1.	Snippet-AxiosExtended.js
2.  Snippet-ConvertToInteractable.txt
3.	Snippet-GetCourseData.php
4.	Snippet-ItemData.txt
5.	Snippet-LootTables.txt
6.	Snippet-ProfileImageUploader.js
7.	Snippet-StateMachine.txt
8.	Snippet-TransporterManager.txt

## Snippet-AxiosExtended.js
In RexAcademy we use a lot of http requests. These requests are done using the tool “Axios”. In this snippet we use the axios request but extend its base functionality with our own. Added to the base functionality is a simple loader (that shows the user that the page is loading). This axiosRequest has been made available in all files no matter what parent-child level it is on.

## Snippet-ConvertToInteractable.cs
In the “The Last Stand” project there are interactable objects. Since these are networked my goal was to reduce the possible data it would need to send. This would require it to be separated from the visible model (so that when I'm unloading the networked entity it wouldn’t unload the visible model). This snippet is part of the tool that i created that allows me to create a copy of the model and remove all unnecessary components and game objects. It also automatically adds the required components to make it an interactable object. In this snippet it converts the selected gameobject(s) to a container (objects that have an inventory like chests and backpacks).

## Snippet-GetCourseData.php
In RexAcademy we often require the users course data. The snippet shows how we get the data from the database and then return a clean json object back to the frontend to handle. 

## Snippet-ItemData.cs
In the “The Last Stand” project I'm working on I need to handle a bunch of items. Each item's data is stored on an instance of the ScriptableObject shown in the snippet. This ScriptableObject contains data that is used throughout the project. The uuid is required to get the correct reference when networking items so it doesn’t need to send all the items data.

## Snippet-LootTables.cs
In the “The Last Stand” project I’m working on I use loot tables to generate randomized loot. These loot tables are Scriptable Objects that hold all information to generate loot using a weight value. The list of items that it returns contains only the reference uuid and the amount.

## Snippet-ProfileImageUploader.js
In RexAcademy we use components to build a page. This is good for organizing the code and keeping it clean and easy to maintain. One of these components is the ProfileImageUploader component. This component allows users to see, remove and upload images. The uploaded images are sent to the backend where they are validated and handled.

## Snippet-StateMachine.cs
In the “The Last Stand” project I have a custom editor window that allows anyone (without the requirement of knowing how to code or knowing how unity works) to manage most of the game. This window uses a state machine to show the correct window content based on the user's selections.

## Snippet-TransporterManager.cs
In the “Settlers” project there is a transporter manager that manages all item transport requests. This manager holds a queue of orders and it will handle each order by finding the closest transporter (transporter is a settler with the job of transporting items). The closest transporter is found by checking the distance between the transporter and the source from which the item needs to be transported.
