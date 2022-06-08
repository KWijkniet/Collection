# Code Snippets
This document is the explanation behind each code snippet.

1.	Snippet-AxiosExtended.js
2.  Snippet-ConvertToInteractable.cs
3.	Snippet-GetCourseData.php
4.	Snippet-ItemData.cs
5.	Snippet-LootTables.cs
6.  Snippet-NodeEditor.cs
7.	Snippet-ProfileImageUploader.js
8.	Snippet-StateMachine.txt
9.	Snippet-TransporterManager.cs

## Snippet-AxiosExtended.js <br/>
In RexAcademy we use a lot of http requests. These requests are done using the tool “Axios”. In this snippet we use the axios request but extend its base functionality with our own. Added to the base functionality is a simple loader (that shows the user that the page is loading). This axiosRequest has been made available in all files no matter what parent-child level it is on. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-AxiosExtended.js)

## Snippet-ConvertToInteractable.cs <br/>
In the “The Last Stand” project there are interactable objects. Since these are networked my goal was to reduce the possible data it would need to send. This would require it to be separated from the visible model (so that when I'm unloading the networked entity it wouldn’t unload the visible model). This snippet is part of the tool that i created that allows me to create a copy of the model and remove all unnecessary components and game objects. It also automatically adds the required components to make it an interactable object. In this snippet it converts the selected gameobject(s) to a container (objects that have an inventory like chests and backpacks). <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-ConvertToInteractable.cs)

## Snippet-GetCourseData.php <br/>
In RexAcademy we often require the users course data. The snippet shows how we get the data from the database and then return a clean json object back to the frontend to handle. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-GetCourseData.php)

## Snippet-ItemData.cs <br/>
In the “The Last Stand” project I'm working on I need to handle a bunch of items. Each item's data is stored on an instance of the ScriptableObject shown in the snippet. This ScriptableObject contains data that is used throughout the project. The uuid is required to get the correct reference when networking items so it doesn’t need to send all the items data. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-ItemData.cs)

## Snippet-LootTables.cs <br/>
In the “The Last Stand” project I’m working on I use loot tables to generate randomized loot. These loot tables are Scriptable Objects that hold all information to generate loot using a weight value. The list of items that it returns contains only the reference uuid and the amount. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-LootTables.cs)

## Snippet-NodeEditor.cs <br/>
In the “The Last Stand” project I’m working on I needed a way to create dialogue. Doing this using just arrays would be confusing and hard to work with. A solution for this is a visual tool such as a node editor (A node editor is a tool where you can drag "nodes" around and connect them using lines. Similar to how flowcharts are made). There are both free and paid tools but i decided it would be a good learning experience to create one myself. This node editor script is a base script to create a node editor. It is made in such a way that anyone can extend this class to create your own logic. With this class i can create a dialogue system, skill tree system and more. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-NodeEditor.cs)

## Snippet-ProfileImageUploader.js <br/>
In RexAcademy we use components to build a page. This is good for organizing the code and keeping it clean and easy to maintain. One of these components is the ProfileImageUploader component. This component allows users to see, remove and upload images. The uploaded images are sent to the backend where they are validated and handled. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-ProfileImageUploader.js)

## Snippet-StateMachine.cs <br/>
In the “The Last Stand” project I have a custom editor window that allows anyone (without the requirement of knowing how to code or knowing how unity works) to manage most of the game. This window uses a state machine to show the correct window content based on the user's selections. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-StateMachine.cs)

## Snippet-TransporterManager.cs <br/>
In the “Settlers” project there is a transporter manager that manages all item transport requests. This manager holds a queue of orders and it will handle each order by finding the closest transporter (transporter is a settler with the job of transporting items). The closest transporter is found by checking the distance between the transporter and the source from which the item needs to be transported. <br/>
[Link](https://github.com/KWijkniet/Collection/blob/main/Code%20Snippets/Snippet-TransporterManager.cs)
