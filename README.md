# Rubber Factory Management System - WPF Application

## Overview

This WPF application is designed for a real rubber factory to manage various operational aspects such as customer information, RFID cards, sales records, and administrative accounts. The core functionality of the app revolves around processing and verifying the **Weight** and **Density** values received from an Arduino board via MQTT, along with capturing real-time images from factory cameras to ensure accuracy.

## Key Features

- **Customer and RFID Management**: Easily manage customer data and RFID card assignments.
- **Sales Management**: Capture sales data, including weight and density information sent through MQTT from the Arduino board.
- **Real-Time Image Capture**: Automatically capture images from factory cameras (such as weight and micro scales) to verify the recorded values.
- **MQTT Integration**: A built-in MQTT broker that communicates with the Arduino board to receive weight and density data in real-time.
- **Data Visualization**: View a live-updating data grid displaying incoming data from the IoT board.
- **Admin Account Management**: Manage access through admin accounts for system administration and security.

## Setup Instructions

1. **Run the Application**:
   - Open the app.
   - Press the **Start** button on the initial screen to create a broker host for the Arduino board to connect to via MQTT.

2. **Login**:
   - After starting the broker, navigate to the **Login Screen**.
   - Enter your credentials to log into the system.
   
3. **Main Screen**:
   - After login, you are directed to the **Main Screen**, where you can:
     - View a **data grid** that updates whenever the Arduino board sends data through the MQTT connection.
     - Capture real-time images from the factory cameras for weight and density verification.
   
## MQTT Integration

- The application acts as an MQTT broker for real-time communication with an Arduino board in the factory.
- The Arduino sends **Weight** and **Density** information through the MQTT protocol, which is displayed in real-time on the data grid.
- Images from the factory camera are captured automatically alongside the incoming data for validation.

## Usage Instructions

1. **Starting the MQTT Broker**: 
   - Launch the app and press the **Start** button on the welcome screen to initiate the MQTT broker, which will allow the Arduino to connect.

2. **Login**:
   - Once the broker is running, proceed to the **Login Screen**. 
   - Enter your user credentials to access the system.
   
3. **Viewing Data**:
   - After logging in, you will be taken to the **Main Screen**.
   - Here, you can view real-time data updates in the grid whenever the IoT board sends **Weight** and **Density** information through MQTT.

4. **Image Verification**:
   - The system captures live images from the factory camera whenever new data is received.
   - These images help verify the weight and density recorded in the system.

## Technical Details

- **Backend**: The application is powered by a WPF-based frontend with MQTT integration for communication between the factory's Arduino and the app.
- **Real-Time Image Capture**: Images are captured from factory cameras to cross-verify the incoming IoT data.
- **RFID Management**: Allows the assignment of RFID cards to customers and tracks their usage.

## Requirements

- **WPF Application** running on a Windows environment.
- **Arduino Board** with MQTT integration for sending weight and density data.
- **Factory Cameras** connected to the system for real-time image capture.
- **Internet Connection** for proper MQTT communication between devices.

---

For any issues or questions, please refer to the project documentation or contact the system administrator.
