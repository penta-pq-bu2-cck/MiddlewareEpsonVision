using RCAPINet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiddlewareEpsonVision
{
    public partial class Form1 : Form
    {
        private Spel m_spel;
        private TcpServer _robotServer;
        string movecommand_ToRobot;


        public Form1()
        {
            InitializeComponent();
            UiLogger.Initialize(textBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ReadandWritePoint();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void ReadandWritePoint()
        {
            // 1️. Read raw data from file
           string filePath = Path.Combine(AppContext.BaseDirectory,"rawData.txt");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("File not found: " + filePath);
                return;
            }

            string rawData = File.ReadAllText(filePath).Trim();

            // 2️. Parse points
            List<RobotPoint> points = ParseRawData(rawData);

            PointListHandler handler = new PointListHandler();

            movecommand_ToRobot = handler.BuildCommand(points);

            Console.WriteLine(movecommand_ToRobot);

            UiLogger.Log($"MoveCommand generated: {movecommand_ToRobot}");

            // 3️. Write to .pts file
            string ptsFile = Path.Combine(AppContext.BaseDirectory, "robot1.pts");
            WritePointsToPts(points, ptsFile);

            UiLogger.Log($"Points parsed: {points.Count} Saved to: {ptsFile}");

            //UpdatetoRobotRC(points);
        }

        public List<RobotPoint> ParseRawData(string rawData)
        {
            string[] values = rawData.Split(',');

            if (values.Length < 7)
                throw new Exception("Raw data too short. Raw Data Receive: " +rawData.ToString());

            int numberOfPoints = int.Parse(values[3].Trim());
            int startIndex = 5;

            List<RobotPoint> points = new List<RobotPoint>();
            SafeGrid(() =>
            { 
                dataGridView1.Rows.Clear();
            
            });
            for (int i = 0; i < numberOfPoints; i++)
            {
                int idx = startIndex + i * 7;

                if (idx + 6 > values.Length)
                    throw new Exception("Not enough values for all points.");

                points.Add(new RobotPoint
                {
                    X = float.Parse(values[idx + 0], CultureInfo.InvariantCulture),
                    Y = float.Parse(values[idx + 1], CultureInfo.InvariantCulture),
                    Z = float.Parse(values[idx + 2], CultureInfo.InvariantCulture),
                    U = float.Parse(values[idx + 3], CultureInfo.InvariantCulture),
                    V = float.Parse(values[idx + 4], CultureInfo.InvariantCulture),
                    W = float.Parse(values[idx + 5], CultureInfo.InvariantCulture),
                    PointStatus = int.Parse(values[idx + 6], CultureInfo.InvariantCulture)
                });

                UiLogger.Log($"P{i}: X={points[i].X:F3}, Y={points[i].Y:F3}, Z={points[i].Z:F3}, " +
                    $"U={points[i].U:F3}, V={points[i].V:F3}, W={points[i].W:F3},SprayStatus={points[i].PointStatus}");
                SafeGrid(() =>
                    {
                    dataGridView1.Rows.Add(
                    $"P{i}",
                    $"{points[i].X:F3}",
                    $"{points[i].Y:F3}",
                    $"{points[i].Z:F3}",
                    $"{points[i].U:F3}",
                    $"{points[i].V:F3}",
                    $"{points[i].W:F3}",
                    $"{points[i].PointStatus}"
                    );
                });
            }

            return points;
        }

        static void WritePointsToPts(List<RobotPoint> points, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff");
                Console.WriteLine(timestamp);

                writer.WriteLine("ENVT0100,LM:" + DateTime.Now.ToString("yyyy\\/MM\\/dd HH:mm:ss:fff") +
                                 ",DS:0100000000,CS:03408,MA:C4-CB-E1-4C-13-97,EI:");
                writer.WriteLine("sVersion=\"2.0.0\"");
                writer.WriteLine("bDisplayR=False");
                writer.WriteLine("bDisplayS=False");
                writer.WriteLine("bDisplayT=False");
                writer.WriteLine("nNumberOfJoints=6");
                writer.WriteLine($"nNumberOfPoints={points.Count}");

                int pointIndex = 1;      // Point1, Point2, ...
                int spelNumber = 0;      // P0, P1, ...P999

                foreach (var p in points)
                {
                    writer.WriteLine($"Point{pointIndex} {{");
                    writer.WriteLine($"\tnNumber={spelNumber}");
                    writer.WriteLine($"\tsLabel=\"P{spelNumber}\"");
                    writer.WriteLine("\tsDescription=\"From Vision\"");
                    writer.WriteLine("\tnUndefined=0");

                    writer.WriteLine($"\trX={p.X:F3}");
                    writer.WriteLine($"\trY={p.Y:F3}");
                    writer.WriteLine($"\trZ={p.Z:F3}");
                    writer.WriteLine($"\trU={p.U:F3}");
                    writer.WriteLine($"\trV={p.V:F3}");
                    writer.WriteLine($"\trW={p.W:F3}");

                    writer.WriteLine("\trR=0");
                    writer.WriteLine("\trS=0");
                    writer.WriteLine("\trT=0");

                    writer.WriteLine("\tnLocal=0");
                    writer.WriteLine("\tnHand=1");
                    writer.WriteLine("\tnElbow=1");
                    writer.WriteLine("\tnWrist=1");

                    writer.WriteLine("\tnJ1Flag=0");
                    writer.WriteLine("\tnJ2Flag=0");
                    writer.WriteLine("\tnJ4Flag=0");
                    writer.WriteLine("\tnJ6Flag=0");

                    writer.WriteLine("\trJ1Angle=0");
                    writer.WriteLine("\trJ4Angle=0");

                    writer.WriteLine("\tbSimVisible=False");
                    writer.WriteLine("}");
                    writer.WriteLine();

                    pointIndex++;
                    spelNumber++;
                }
            }
        }

        #region EpsonApiDll
        public void UpdatetoRobotRC(List<RobotPoint> points)
        {
            m_spel = new Spel();
            UiLogger.Log("Robot RC Initializing");
            m_spel.Initialize();
            UiLogger.Log("Read Robot Project File C:\\EpsonRC70\\projects\\xxxxx and connect to it...");
            m_spel.Project = "C:\\EpsonRC70\\projects\\xxxxxx\\xxxxx.sprj";

            // Set number of points to Global Preserve Integer gPI_numberofvpoint
            UiLogger.Log("Set number of points to Global Preserve Integer gPI_numberofvpoint");
            m_spel.SetVar("gPI_numberofvpoint", points.Count);

            // Set a flag Global Boolean gB_vpointready to inform complete
            UiLogger.Log("Set a flag Global Boolen gB_vpointready to inform complete");
            m_spel.On("gB_vpointready");
        }
        #endregion 

        public void SendtoRobotRCMove(List<RobotPoint> points, TcpServer server)
        {
            for (int i = 0; i < points.Count; i++)
            {
                RobotPoint p = points[i];
                // Build command: move,px,py,pz,pu,pv,pw

                string command = string.Format(
                     CultureInfo.InvariantCulture,
                    "Save,{0:F3},{1:F3},{2:F3},{3:F3},{4:F3},{5:F3},{6}",
                     p.X, p.Y, p.Z, p.U, p.V, p.W,p.PointStatus
                      );
    

                UiLogger.Log($"→ Robot: {command.Trim()}");
                server.Send(command);

                // Wait for robot to confirm movement finished
                string response;
                bool tcpAckBool; 
                do
                {
                    response = server.Listen().Trim();
                    UiLogger.Log($"← Robot: {response}");

                    
                    decimal[] sent = ParseMoveToDecimals_SendCommand(command);
                    decimal[] recv = ParseMoveToDecimals_ReceiveCommand(response);

                     tcpAckBool =
                        sent[0] == recv[0] &&
                        sent[1] == recv[1] &&
                        sent[2] == recv[2] &&
                        sent[3] == recv[3] &&
                        sent[4] == recv[4] &&
                        sent[5] == recv[5];
                }
               //  while (!tcpAckBool);
                while (false) ;

            }

            UiLogger.Log("✔ All robot points sending finished");
            server.Send("End,1");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var client = new MechMindTcpClient();

                await client.ConnectAsync(txtBox_mmIP.Text, int.Parse(txtBox_mmPort.Text));
                await client.SendAsync(txtBox_mmCommand.Text);

                string rawData = await client.ReceiveAsync();
                client.Close();

                // TODO: parse rawData and update UI or other logic
                UiLogger.Log("Received data length: " + rawData.Length);
                UiLogger.Log("Raw data from vision: " + rawData.ToString());

                List<RobotPoint> points = ParseRawData(rawData);

                // Write to .pts file
                string ptsFile = Path.Combine(AppContext.BaseDirectory, "robot1_VP.pts");
                WritePointsToPts(points, ptsFile);

                UiLogger.Log($"Points parsed: {points.Count} Saved to: {ptsFile}");

               // UpdatetoRobotRC(points);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            backgroundWorker1.RunWorkerAsync();

            try
            {
                // changelog file named: changelog.txt
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "changelog.txt");
                if (System.IO.File.Exists(filePath))
                {
                    string changelogContent = System.IO.File.ReadAllText(filePath);
                    changelogDisplay.Text = changelogContent;
                }
                else
                {
                    changelogDisplay.Text = "Changelog file not found.";
                }
            }
            catch (Exception ex)
            {
                ;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _robotServer = new TcpServer();
                _robotServer.Start(410);

                UiLogger.Log("TCP Robot Server started on port 410");



                while (true)
                {
                    string msg = _robotServer.Listen(); // will throw if client disconnects
                    UiLogger.Log("Received: " + msg);

                    string[] cmdArray = msg.Trim().Split(',');

                    if (cmdArray.Length > 0 && cmdArray[0] == "gettoolpath")
                    {
                         HandleGetToolPath(cmdArray);
                        

                        while (true)
                        {
                            string msg1 = _robotServer.Listen(); // will throw if client disconnects

                            if (msg1 == "getmovecommand")
                            {
                                UiLogger.Log($"← Robot:{msg1}");
                                _robotServer.Send(movecommand_ToRobot);
                                UiLogger.Log($"→ Robot:{movecommand_ToRobot}");
                            }
                        }
                    }
                }
            }
            catch (IOException)
            {
                UiLogger.Log("Client disconnected.");
            }
            catch (Exception ex)
            {
                UiLogger.Log("Server error: " + ex.Message);
            }
            finally
            {
                _robotServer?.Dispose();
            }
        }

        private void HandleGetToolPath(string[] cmdArray)
        {
            _robotServer.Send("gettoolpath,1");

            try
            {
                //////////////////***Option 1: Test using Mindmech vision*******************************////////////////
                ///*************************************************************************************************
                //var client = new MechMindTcpClient();

                //client.ConnectAsync(
                //    txtBox_mmIP.Text,
                //    int.Parse(txtBox_mmPort.Text)
                //).GetAwaiter().GetResult();

                //client.SendAsync(
                //    $"100,0,0,1,{string.Join(",", cmdArray.Skip(1))}"
                //).GetAwaiter().GetResult();

                //string vision_ReplyStatus = client.ReceiveAsync()
                //                       .GetAwaiter().GetResult();

                //if the vision reply is success, then will need to read the output txt file (quaternion point) and send it to CAD Team
                //client.Close();
                ///*************************************************************************************************
                ///
                //////////////////***Option 2: Test using Raw Data File*******************************////////////////
                ///*************************************************************************************************

                string filePath = Path.Combine(AppContext.BaseDirectory, "rawData.txt");
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File not found: " + filePath);
                    return;
                }

                string rawData = File.ReadAllText(filePath).Trim();

                ///*************************************************************************************************                UiLogger.Log("Vision data length: " + rawData.Length);

                List<RobotPoint> points = ParseRawData(rawData);

                PointListHandler handler = new PointListHandler();

                movecommand_ToRobot = handler.BuildCommand(points);

                UiLogger.Log($"MoveCommand Generated: {movecommand_ToRobot}");

                string ptsFile = Path.Combine(AppContext.BaseDirectory, "robot1_VP.pts");

                WritePointsToPts(points, ptsFile);

                UiLogger.Log($"Points parsed: {points.Count}");

                // Robot motion happens here, sequentially
                SendtoRobotRCMove(points, _robotServer);
            }
            catch (Exception ex)
            {
                UiLogger.Log("ERROR: " + ex.Message);
            }
        }

        private decimal[] ParseMoveToDecimals_SendCommand(string msg)
        {
            string[] parts = msg.Trim().Split(',');

            if (parts.Length != 8 || !parts[0].Equals("Save", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid Save command: " + msg);

               return new decimal[]
                {
                decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                decimal.Parse(parts[6], CultureInfo.InvariantCulture)
                };
        }

        private decimal[] ParseMoveToDecimals_ReceiveCommand(string msg)
        {
            string[] parts = msg.Trim().Split(',');

            if (parts.Length != 8 || !parts[0].Equals("Ack", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid SAVE command: " + msg);

            return new decimal[]
             {
                decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                decimal.Parse(parts[6], CultureInfo.InvariantCulture),
                decimal.Parse(parts[7], CultureInfo.InvariantCulture)
             };
        }

        private void SafeGrid(Action action)
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UiLogger.Log("Service stopped. Restarting to wait for new client...");

            Thread.Sleep(500); // optional small delay

            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
