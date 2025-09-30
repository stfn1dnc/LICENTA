# Augmented Reality Application with AI Support for Maintenance Procedures

## Overview
This project was developed as part of my Bachelor's Thesis at the Technical University of Cluj-Napoca, Faculty of Automation and Computer Science.  
It is an experimental mobile application that combines Augmented Reality (AR) and Artificial Intelligence (AI) to support maintenance procedures for industrial equipment.

The solution was implemented with Unity as the main development environment for the front-end, connected to a Node.js + Express backend, MongoDB Atlas, and Firebase services, while also integrating the OpenAI API for conversational assistance.

## Objectives
- Develop a mobile AR-based tool for equipment maintenance in industrial and educational contexts.  
- Enable real-time equipment recognition using Vuforia image tracking.  
- Provide AI-driven technical support through a chat module powered by the OpenAI API.  
- Allow photo capture, annotation, and cloud storage for documentation and traceability.  
- Ensure secure user authentication and access control via Firebase and JWT.  
- Deliver an intuitive, reliable, and responsive interface optimized for Android devices.

## Technologies Used
### Frontend
- Unity 2022.3 LTS – main development framework  
- Vuforia SDK – image tracking and AR overlay  
- TextMeshPro and Unity UI Toolkit – user interface components  

### Backend
- Node.js and Express – REST API for managing equipment data and AI requests  
- MongoDB Atlas – NoSQL cloud database for metadata, logs, and users  
- Firebase – authentication and media storage  

### Artificial Intelligence
- OpenAI API (ChatGPT) – natural language assistant for contextual maintenance support  

### Hosting and Development Tools
- Render.com – backend hosting with HTTPS access for Unity client  
- Git and GitHub – version control and collaboration  
- Visual Studio Code – primary code editor  

## Features
- User login and registration with Firebase Authentication  
- AR-based equipment recognition with Vuforia image tracking  
- AI chat panel for real-time technical support  
- Capture, annotate, and upload images to Firebase Storage  
- Photo history viewer for tracking inspected components  
- Secure backend routes protected with JWT authentication  

## Project Structure
-Assets -> Unity scripts, prefabs and UI panels
-Packages -> Unity package dependencies (manifest.json)
-Project_Settings -> Unity project configuration
-Backend -> Node.js + Express server(CRUD, AI integration)
-Databaase -> MongoDB Atlas (equipment, logs, users)

## How It Works
1. The user registers or logs in through Firebase.  
2. The app uses AR tracking to recognize industrial equipment.  
3. Relevant information is retrieved from MongoDB Atlas and displayed in the interface.  
4. The user can capture and annotate photos, which are stored in Firebase Storage.  
5. The AI chat panel allows the user to ask questions and receive contextual guidance.  
6. All actions are logged and secured through the backend.

## Security
- Authentication and authorization via JWT  
- Rate limiting for AI requests  
- Input validation and sanitization  
- Robust error handling and logging  

## Results
- Fully functional prototype tested on Android devices with PLC equipment.  
- Demonstrates seamless integration of Unity, AR technologies, AI, and cloud services.  
- Provides a scalable foundation for future industrial AR applications.

This project was created as part of my Bachelor's Thesis:  
*“Experimental Augmented Reality Application with AI Support for Maintenance Procedures”*  
© 2025 Ștefan Danci – Technical University of Cluj-Napoca