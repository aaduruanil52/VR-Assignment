ViSNET XR - Meta Quest VR Application
A Meta Quest VR application built with Unity for ViSNET AI Technologies assignment.

📱 Platform

Meta Quest 2
Meta Quest 3
Meta Quest Pro


🚀 Features

✅ Login Screen with JWT authentication
✅ Project List Screen
✅ Floor Selection Screen
✅ REST API integration
✅ XR Interaction Toolkit
✅ OpenXR support
✅ Meta Quest System Keyboard


🔗 Links

Backend API: https://vr-assignment.onrender.com
GitHub: https://github.com/aaduruanil52/VR-Assignment


🔑 Test Credentials
UsernamePasswordtestuser123456adminadmin123

🛠️ Tech Stack
TechnologyVersionUnity2022.3.26f1OpenXR Plugin1.10.0XR Interaction Toolkit3.0.3Oculus XR Plugin4.2.0TextMeshPro3.0.6BackendNode.js + ExpressHostingRender.com

📡 API Endpoints
MethodEndpointDescriptionPOST/api/loginUser authenticationGET/api/projectsGet all projectsGET/api/projects/:id/floorsGet floors for a project

🏗️ Project Structure
VR-Assignment/
├── Assets/
│   ├── Scripts/
│   │   ├── API/
│   │   │   ├── APIManager.cs
│   │   │   ├── AuthAPI.cs
│   │   │   └── ProjectAPI.cs
│   │   ├── Managers/
│   │   │   ├── NavigationManager.cs
│   │   │   ├── SessionManager.cs
│   │   │   └── ToastManager.cs
│   │   ├── UI/
│   │   │   ├── LoginUI.cs
│   │   │   ├── ProjectListUI.cs
│   │   │   └── FloorDropdownUI.cs
│   │   └── XR/
│   │       └── XRUISetup.cs
│   └── Scenes/
│       ├── LoginScene
│       ├── ProjectListScene
│       └── FloorSelectionScene
└── backend/
    ├── index.js
    └── package.json

▶️ How to Run

Install APK on Meta Quest 2/3/Pro
Enable developer mode on Quest
Install via ADB:

adb install VR-Assignment.apk

Launch VR-Assignment from Unknown Sources
Login with test credentials


👤 Author
Anil Aaduru

Email: aaduruanil52@gmail.com
GitHub: https://github.com/aaduruanil52
