# MiddlewareEpsonVision

Middleware application that integrates a Vision System (Mech-Mind) with an Epson RC+ robot controller.

This application:
- Receives toolpath data from vision (or local file)
- Parses position and orientation data
- Generates Epson-compatible `.pts` files
- Updates Epson RC+ global variables via RCAPINet
- Communicates with the robot controller via TCP
- Executes sequential robot motion with acknowledgment verification

---

## System Overview

Mech-Mind Vision → MiddlewareEpsonVision → Epson RC+ (RC70)

The middleware acts as a bridge between the vision system and the robot controller.

---

## Features

- Parse comma-separated raw vision data
- Convert vision output into structured robot points
- Automatically generate Epson `.pts` files
- Update Epson RC+ project variables
- TCP server for robot communication (Port 410)
- Sequential robot move execution with ACK validation
- UI logging and DataGrid visualization
- Local test mode using `rawData.txt`

---

## Input Data Format

The middleware expects comma-separated values:

200,0,0,<numberOfPoints>,...,X,Y,Z,U,V,W,X,Y,Z,U,V,W,...

Where:
- `values[3]` = number of points
- Each point consists of 6 values:
  - X, Y, Z (Position)
  - U, V, W (Orientation)

Example:

200,0,0,2,0,
100.0,200.0,300.0,0.0,180.0,0.0,
110.0,210.0,300.0,0.0,180.0,0.0


---

## Generated Output

The system generates Epson-compatible point files:

robot1.pts
robot1_VP.pts


Each point is written in Epson RC+ format:

Point1 {
nNumber=0
sLabel="P0"
rX=100.000
rY=200.000
rZ=300.000
rU=0.000
rV=180.000
rW=0.000
}


---

## TCP Communication

### TCP Server
- Port: 410
- Waits for robot command:

gettoolpath


### Move Command Sent to Robot

Move,X,Y,Z,U,V,W


Example:
Move,100.000,200.000,300.000,0.000,180.000,0.000

### Expected Robot Acknowledgment

Ack,Move,X,Y,Z,U,V,W


The middleware verifies:
- Sent values match received values
- Continues only when values match

After all points are executed:

End,1


---

## Epson RC+ Integration

Uses:

RCAPINet.dll

Configured RC+ project path:

C:\EpsonRC70\projects\XXX\XXX.sprj


Global variables used in RC+:

| Variable Name        | Type     | Purpose                       |
|----------------------|----------|-------------------------------|
| gPI_numberofvpoint   | Integer  | Number of vision points       |
| gB_vpointready       | Boolean  | Vision data ready flag        |

---

## How to Run

1. Install Epson RC+ 7.0
2. Ensure `RCAPINet.dll` is referenced
3. Update the RC+ project path in code if needed
4. Place test data inside `rawData.txt`
5. Run the application
6. Use:
   - Button1 → Load from local file
   - Button2 → Load from vision system

---

## Test Modes

### Option 1 – Vision System
Uses `MechMindTcpClient` to receive real vision data.

### Option 2 – Local File
Reads toolpath data from:

rawData.txt


Configured inside:
HandleGetToolPath()


---

## Dependencies

- .NET Framework (Windows Forms)
- Epson RC+ 7.0
- RCAPINet.dll
- Mech-Mind Vision (optional)
- TCP-enabled Epson robot controller

---

## Error Handling

The application handles:
- Invalid raw data format
- Missing values
- TCP communication errors
- Invalid Move/Ack messages
- Missing files

---

## Notes

- Decimal parsing uses InvariantCulture
- Move validation requires exact decimal match
- Port 410 must be open in firewall
- Application must have permission to access RC+

---

## Future Improvements

- Add tolerance for floating-point comparison
- Add ACK timeout handling
- Move configuration values to config file
- Add persistent logging to file
- Add reconnect logic
- Remove hardcoded project path

---

## Author

CK Developed for Vision-Guided Epson Robot integration.
