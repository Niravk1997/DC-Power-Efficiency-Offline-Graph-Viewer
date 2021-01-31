using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Offline_Graph_Viewer
{

    public partial class MainWindow : Window
    {
        List<double> Test_Efficiency = new List<double>();

        List<double> Sink_Voltage = new List<double>();

        List<double> Sink_Current = new List<double>();

        List<double> Source_Voltage = new List<double>();

        List<double> Source_Current = new List<double>();

        List<double> Output_Power = new List<double>();

        List<double> Input_Power = new List<double>();

        List<double> Test_power_Loss = new List<double>();

        List<double> CircuitResistance = new List<double>();

        List<double> SourceResistance = new List<double>();

        List<double> SinkResistance = new List<double>();

        Queue<string> Colours = new Queue<string>();
        Random randomNum = new Random();
        string[] line_Parts;

        Graph_Window PowerEfficiency_OutputCurrent;
        Graph_Window PowerEfficiency_OutputPower;
        Graph_Window OutputPower_InputPower;
        Graph_Window InputVoltage_InputCurrent;
        Graph_Window OutputVoltage_OutputCurrent;
        Graph_Window InputCurrent_InputVoltage;
        Graph_Window OutputCurrent_OutputVoltage;
        Graph_Window PowerLoss_OutputCurrent;
        Graph_Window PowerLoss_OutputPower;

        Table DataTable;
        bool AddTest_Table = true;
        //Various Codes to set Output Log Color
        int Success_Code = 0;
        int Error_Code = 1;
        int Warning_Code = 2;
        int Message_Code = 3;

        string FilePath = "";
        string testName = "";
        string red_value = "";
        string green_value = "";
        string blue_value = "";

        string Choosen_Color = "255, 255, 255";


        public MainWindow()
        {
            InitializeComponent();
            Color_Palette();
            color_choose();
            insert_Log("Click on the Graphs menu and then show graph. Click on whichever graph(s) you like to see.", Message_Code);
            insert_Log("Click on the Table menu and then click show table to open the table window.", Message_Code);
            insert_Log("Drag and drop a final results text file below on the file drop rectangle.", Message_Code);
            insert_Log("You may also browse for the final results text file by clicking the browse button.", Message_Code);
            insert_Log("Only the Final Results text file's data can be added to the graphs.", Warning_Code);
            insert_Log("When you are ready click the Add button to add the graph to any graph window that is open.", Message_Code);
        }

        private void color_choose() 
        {
            if (Colours.Count() < 1)
            {
                Choosen_Color = randomNum.Next(0, 255).ToString() + "," + randomNum.Next(0, 255).ToString() + "," + randomNum.Next(0, 255).ToString();
                string[] color_parts = Choosen_Color.Split(',');
                Color_Preview.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)int.Parse(color_parts[0]), (byte)int.Parse(color_parts[1]), (byte)int.Parse(color_parts[2])));
                Red_color.Text = color_parts[0];
                Green_color.Text = color_parts[1];
                BLue_color.Text = color_parts[2];
                insert_Log("Color Set to Red: " + color_parts[0] + "  Green: " + color_parts[1] + "  Blue: " + color_parts[2], Message_Code);
                insert_Log("Ran out of predefined Color Palettes, random colors will be chosen.", Warning_Code);
            }
            else
            {
                Choosen_Color = Colours.Dequeue();
                string[] color_parts = Choosen_Color.Split(',');
                Color_Preview.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)int.Parse(color_parts[0]), (byte)int.Parse(color_parts[1]), (byte)int.Parse(color_parts[2])));
                insert_Log("Color Set to Red: " + color_parts[0] + "  Green: " + color_parts[1] + "  Blue: " + color_parts[2], Message_Code);
                Red_color.Text = color_parts[0];
                Green_color.Text = color_parts[1];
                BLue_color.Text = color_parts[2];
            }
        }

        private void ClearOutputLog_Click(object sender, RoutedEventArgs e)
        {
            Output_Log.Text = String.Empty;
            Output_Log.Inlines.Clear();
        }

        private void insert_Log(string Message, int Code)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt");
            SolidColorBrush Color = Brushes.Black;
            string Status = "";
            if (Code == Error_Code) //Error Message
            {
                Status = "[Error]";
                Color = Brushes.Red;
            }
            else if (Code == Success_Code) //Success Message
            {
                Status = "[Success]";
                Color = Brushes.Green;
            }
            else if (Code == Warning_Code) //Warning Message
            {
                Status = "[Warning]";
                Color = Brushes.Orange;
            }
            else if (Code == Message_Code)//Standard Message
            {
                Status = "";
                Color = Brushes.Blue;
            }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
            {
                Output_Log.Inlines.Add(new Run("[" + date + "]" + " " + Status + " " + Message + "\n") { Foreground = Color });
                Output_Log_Scroll.ScrollToBottom();
            }));
        }

        private void Color_Palette()
        {
            try
            {
                Colours.Clear();
                Colours = new Queue<string>(new string[] { "220,20,60", "34,139,34", "0,191,255", "255,20,147", "255,165,0", "255,255,0", "128,0,128", "240,230,140", "124,252,0", "233,150,122", "255,0,255", "0,0,205", "255,69,0", "85,107,47", "127,0,0", "72,61,139", "0,139,139", "210,105,30", "154,205,50", "143,188,143", "176,48,96", "0,255,127", "0,255,255", "218,112,214", "176,196,222", "30,144,255", "123,104,238", "105,105,105", "0,0,128", "144,238,144" });
            }
            catch (Exception)
            {
                insert_Log("Could not initialized/reset color palette.", Error_Code);
            }
        }

        private void Reset_Colors_Click(object sender, RoutedEventArgs e)
        {
            Color_Palette();
            insert_Log("Colour Palette Reset.", Warning_Code);
        }

        private void App_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                InitialDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                bool fileExists = openFileDialog.CheckFileExists;
                bool filePathExists = openFileDialog.CheckPathExists;
                string fileExtension = System.IO.Path.GetExtension(openFileDialog.FileName);
                string filePath = openFileDialog.FileName;
                string fileName = openFileDialog.SafeFileName;
                if (fileExists == true & filePathExists == true)
                {
                    if (fileExtension == ".txt")
                    {
                        insert_Log("File Path: " + filePath, Message_Code);
                        File_Path.Text = filePath;
                        FilePath = filePath;
                        try
                        {
                            int NameFrom = fileName.IndexOf("");
                            int NameTo = fileName.LastIndexOf("-");
                            string TestName = fileName.Substring(NameFrom, NameTo);
                            Test_Name.Text = TestName.Trim();
                            testName = TestName.Trim();
                        }
                        catch (Exception)
                        {
                            insert_Log("Cannot read test name. Please enter it manually.", Error_Code);
                            Test_Name.Text = string.Empty;
                        }
                    }
                    else
                    {
                        insert_Log("File is invalid. Must be a text file.", Error_Code);
                    }
                }
                else
                {
                    insert_Log("File not found or file path not valid. Try again.", Error_Code);
                }
            }
        }

        private void Test_Add_Click(object sender, RoutedEventArgs e)
        {
            if (insertGraphData(FilePath) == true) 
            {
                Output_Collected_Data();
                addDataToTable();
                Clear_Output_Collected_Data();
                insert_Log("Graph data added successfully.", Success_Code);
                File_Path.Text = string.Empty;
                Test_Name.Text = string.Empty;
                FilePath = string.Empty;
                color_choose();
            }
        }

        private void addDataToTable() 
        {
            if (AddTest_Table == true & DataTable != null)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                {
                    for (int i = 0; i < Source_Voltage.Count; i++)
                    {
                        DataTable.addTest(testName, Source_Voltage[i], Source_Current[i], Sink_Voltage[i], Sink_Current[i], Input_Power[i], Output_Power[i], Test_Efficiency[i], Output_Power[i], SourceResistance[i], SinkResistance[i], CircuitResistance[i]);
                    }
                }));
            }
        }

        private void Color_Set_Click(object sender, RoutedEventArgs e)
        {
            bool redOK = convertToint(Red_color.Text);
            bool GreenOK = convertToint(Green_color.Text);
            bool BlueOK = convertToint(BLue_color.Text);
            if (redOK == true & GreenOK == true & BlueOK == true) 
            {
                red_value = Red_color.Text;
                green_value = Green_color.Text;
                blue_value = BLue_color.Text;
                Color_Preview.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)int.Parse(red_value), (byte)int.Parse(green_value), (byte)int.Parse(blue_value)));
                insert_Log("Color Set to Red: " + red_value + "  Green: " + green_value + "  Blue: " + blue_value, Message_Code);
                Choosen_Color = red_value + "," + green_value + "," + blue_value;
            }
        }

        private bool convertToint(string value)
        {
            bool isValueValid = int.TryParse(value, out int Value);
            if (isValueValid == true)
            {
                if (Value <= 255 & Value >= 0)
                {
                    return true;
                }
                else 
                {
                    insert_Log("Color value must be an integer <= 255 and >= 0.", Error_Code);
                    Red_color.Text = string.Empty;
                    Green_color.Text = string.Empty;
                    BLue_color.Text = string.Empty;
                    return false; 
                }
            }
            else
            {
                insert_Log("Color value must be an integer <= 255 and >= 0.", Error_Code);
                Red_color.Text = string.Empty;
                Green_color.Text = string.Empty;
                BLue_color.Text = string.Empty;
                return false;
            }
        }

        private bool insertGraphData(string path)
        {
            try
            {
                using (var readFile = new StreamReader(path))
                {
                    readFile.ReadLine().Trim();
                    string line;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        line_Parts = line.Split(',');
                        Source_Voltage.Add(double.Parse(line_Parts[0]));
                        Source_Current.Add(double.Parse(line_Parts[1]));
                        Sink_Voltage.Add(double.Parse(line_Parts[2]));
                        Sink_Current.Add(double.Parse(line_Parts[3]));
                        Input_Power.Add(double.Parse(line_Parts[4]));
                        Output_Power.Add(double.Parse(line_Parts[5]));
                        Test_Efficiency.Add(double.Parse(line_Parts[6]));
                        Test_power_Loss.Add(double.Parse(line_Parts[7]));
                        SourceResistance.Add(double.Parse(line_Parts[8]));
                        SinkResistance.Add(double.Parse(line_Parts[9]));
                        CircuitResistance.Add(double.Parse(line_Parts[10]));
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                insert_Log("File read failed. Try again.", Error_Code);
                return false;
            }
        }

        private void PE_OC_Click(object sender, RoutedEventArgs e)
        {
            if (PowerEfficiency_OutputCurrent == null)
            {
                PowerEfficiency_OutputCurrent = new Graph_Window();
                PowerEfficiency_OutputCurrent.Closed += (a, b) => PowerEfficiency_OutputCurrent = null;
                PowerEfficiency_OutputCurrent.setWindowTitle("Power Efficiency (%) vs [Sink] Output Load Current (A)");
                PowerEfficiency_OutputCurrent.setXYLabels("Output Load Current (A)", "Power Efficiency (%)");
                PowerEfficiency_OutputCurrent.setXYUnits("A", "%");
                PowerEfficiency_OutputCurrent.Show();
            }
            else
            {
                PowerEfficiency_OutputCurrent.Show();
                insert_Log("PowerEfficiency vs Output Current Window is already open.", Warning_Code);
            }
        }

        private void PE_OP_Click(object sender, RoutedEventArgs e)
        {
            if (PowerEfficiency_OutputPower == null)
            {
                PowerEfficiency_OutputPower = new Graph_Window();
                PowerEfficiency_OutputPower.Closed += (a, b) => PowerEfficiency_OutputPower = null;
                PowerEfficiency_OutputPower.setWindowTitle("Power Efficiency (%) vs [Sink] Output Load Power (W)");
                PowerEfficiency_OutputPower.setXYLabels("Output Load Power (W)", "Power Efficiency (%)");
                PowerEfficiency_OutputPower.setXYUnits("W", "%");
                PowerEfficiency_OutputPower.Show();
            }
            else
            {
                PowerEfficiency_OutputPower.Show();
                insert_Log("Power Efficiency vs Output Power Window is already open.", Warning_Code);
            }
        }

        private void IP_OP_Click(object sender, RoutedEventArgs e)
        {
            if (OutputPower_InputPower == null)
            {
                OutputPower_InputPower = new Graph_Window();
                OutputPower_InputPower.Closed += (a, b) => OutputPower_InputPower = null;
                OutputPower_InputPower.setWindowTitle("[Sink] Output Load Power (W) vs [Source] Input Power (W)");
                OutputPower_InputPower.setXYLabels("Input Power (W)", "Output Load Power (W)");
                OutputPower_InputPower.setXYUnits("W", "W");
                OutputPower_InputPower.Show();
            }
            else
            {
                OutputPower_InputPower.Show();
                insert_Log("Input Power vs Output Power Window is already open.", Warning_Code);
            }
        }

        private void IC_IV_Click(object sender, RoutedEventArgs e)
        {
            if (InputCurrent_InputVoltage == null)
            {
                InputCurrent_InputVoltage = new Graph_Window();
                InputCurrent_InputVoltage.Closed += (a, b) => InputCurrent_InputVoltage = null;
                InputCurrent_InputVoltage.setWindowTitle("[Source] Input Current (A) vs [Source] Input Voltage (V)");
                InputCurrent_InputVoltage.setXYLabels("Input Voltage (V)", "Input Current (A)");
                InputCurrent_InputVoltage.setXYUnits("V", "A");
                InputCurrent_InputVoltage.Show();
            }
            else
            {
                InputCurrent_InputVoltage.Show();
                insert_Log("Input Current vs Input Voltage Window is already open.", Warning_Code);
            }
        }

        private void OC_OV_Click(object sender, RoutedEventArgs e)
        {
            if (OutputCurrent_OutputVoltage == null)
            {
                OutputCurrent_OutputVoltage = new Graph_Window();
                OutputCurrent_OutputVoltage.Closed += (a, b) => OutputCurrent_OutputVoltage = null;
                OutputCurrent_OutputVoltage.setWindowTitle("[Sink] Output Load Current (A) vs [Sink] Output Voltage (V)");
                OutputCurrent_OutputVoltage.setXYLabels("Output Voltage (V)", "Output Load Current (A)");
                OutputCurrent_OutputVoltage.setXYUnits("V", "A");
                OutputCurrent_OutputVoltage.Show();
            }
            else
            {
                OutputCurrent_OutputVoltage.Show();
                insert_Log("Output Current vs Output Voltage Window is already open.", Warning_Code);
            }
        }

        private void IV_IC_Click(object sender, RoutedEventArgs e)
        {
            if (InputVoltage_InputCurrent == null)
            {
                InputVoltage_InputCurrent = new Graph_Window();
                InputVoltage_InputCurrent.Closed += (a, b) => InputVoltage_InputCurrent = null;
                InputVoltage_InputCurrent.setWindowTitle("[Source] Input Voltage (V) vs [Source] Input Current (A)");
                InputVoltage_InputCurrent.setXYLabels("Input Current (A)", "Input Voltage (V)");
                InputVoltage_InputCurrent.setXYUnits("A", "V");
                InputVoltage_InputCurrent.Show();
            }
            else
            {
                InputVoltage_InputCurrent.Show();
                insert_Log("Input Current vs [Source] Input Voltage Window is already open.", Warning_Code);
            }
        }

        private void OV_OC_Click(object sender, RoutedEventArgs e)
        {
            if (OutputVoltage_OutputCurrent == null)
            {
                OutputVoltage_OutputCurrent = new Graph_Window();
                OutputVoltage_OutputCurrent.Closed += (a, b) => OutputVoltage_OutputCurrent = null;
                OutputVoltage_OutputCurrent.setWindowTitle("[Sink] Output Voltage (V) vs [Sink] Output Load Current (A)");
                OutputVoltage_OutputCurrent.setXYLabels("Output Load Current (A)", "Output Voltage (V)");
                OutputVoltage_OutputCurrent.setXYUnits("A", "V");
                OutputVoltage_OutputCurrent.Show();
            }
            else
            {
                OutputVoltage_OutputCurrent.Show();
                insert_Log("Output Voltage vs Output Current Window is already open.", Warning_Code);
            }
        }

        private void PL_OC_Click(object sender, RoutedEventArgs e)
        {
            if (PowerLoss_OutputCurrent == null)
            {
                PowerLoss_OutputCurrent = new Graph_Window();
                PowerLoss_OutputCurrent.Closed += (a, b) => PowerLoss_OutputCurrent = null;
                PowerLoss_OutputCurrent.setWindowTitle("Power Loss (%) vs Output Load Current (A)");
                PowerLoss_OutputCurrent.setXYLabels("Output Load Current (A)", "Power Loss (%)");
                PowerLoss_OutputCurrent.setXYUnits("A", "%");
                PowerLoss_OutputCurrent.Show();
            }
            else
            {
                PowerLoss_OutputCurrent.Show();
                insert_Log("Power Loss vs Output Current Window is already open.", Warning_Code);
            }
        }

        private void PL_OP_Click(object sender, RoutedEventArgs e)
        {
            if (PowerLoss_OutputPower == null)
            {
                PowerLoss_OutputPower = new Graph_Window();
                PowerLoss_OutputPower.Closed += (a, b) => PowerLoss_OutputPower = null;
                PowerLoss_OutputPower.setWindowTitle("Power Loss (%) vs Output Load Power (W)");
                PowerLoss_OutputPower.setXYLabels("Output Load Power (W)", "Power Loss (%)");
                PowerLoss_OutputPower.setXYUnits("W", "%");
                PowerLoss_OutputPower.Show();
            }
            else
            {
                PowerLoss_OutputPower.Show();
                insert_Log("Power Loss vs Output Load Power Window is already open.", Warning_Code);
            }
        }

        private void showTable_Click(object sender, RoutedEventArgs e)
        {
            if (DataTable == null)
            {
                DataTable = new Table();
                DataTable.Closed += (a, b) => DataTable = null;
                DataTable.Show();
            }
            else
            {
                DataTable.Show();
                insert_Log("Table is already open.", Warning_Code);
            }
        }

        private void addtoTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTest_Table = !AddTest_Table;
                if (AddTest_Table == true)
                {
                    addtoTable.IsChecked = true;
                }
                else
                {
                    addtoTable.IsChecked = false;
                }
            }
            catch (Exception)
            {

            }
        }

        private void Output_Collected_Data()
        {
            string averageSinkVolt = Sink_Voltage.Average().ToString();
                string averageSourceVolt = Source_Voltage.Average().ToString();
                try
                {
                    if (PowerEfficiency_OutputCurrent != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            PowerEfficiency_OutputCurrent.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Sink_Current, Test_Efficiency, Choosen_Color);
                            PowerEfficiency_OutputCurrent.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Power Efficiency vs Output Current Graph", Error_Code);
                }
                try
                {
                    if (PowerEfficiency_OutputPower != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            PowerEfficiency_OutputPower.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Output_Power, Test_Efficiency, Choosen_Color);
                            PowerEfficiency_OutputPower.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Power Efficiency vs Output Power Graph", Error_Code);
                }
                try
                {
                    if (OutputPower_InputPower != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            OutputPower_InputPower.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Input_Power, Output_Power, Choosen_Color);
                            OutputPower_InputPower.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Input Power vs Output Power Graph", Error_Code);
                }
                try
                {
                    if (InputCurrent_InputVoltage != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            InputCurrent_InputVoltage.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Source_Voltage, Source_Current, Choosen_Color);
                            InputCurrent_InputVoltage.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Input Current vs Input Voltage Graph", Error_Code);
                }
                try
                {
                    if (OutputCurrent_OutputVoltage != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            OutputCurrent_OutputVoltage.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Sink_Voltage, Sink_Current, Choosen_Color);
                            OutputCurrent_OutputVoltage.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Output Load Current vs Output Voltage Graph", Error_Code);
                }
                try
                {
                    if (InputVoltage_InputCurrent != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            InputVoltage_InputCurrent.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Source_Current, Source_Voltage, Choosen_Color);
                            InputVoltage_InputCurrent.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Input Voltage vs Input Current Graph", Error_Code);
                }
                try
                {
                    if (OutputVoltage_OutputCurrent != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            OutputVoltage_OutputCurrent.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Sink_Current, Sink_Voltage, Choosen_Color);
                            OutputVoltage_OutputCurrent.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Input Voltage vs Input Current Graph", Error_Code);
                }
                try
                {
                    if (PowerLoss_OutputCurrent != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            PowerLoss_OutputCurrent.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Sink_Current, Test_power_Loss, Choosen_Color);
                            PowerLoss_OutputCurrent.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Power Loss vs Output Load Current Graph", Error_Code);
                }
                try
                {
                    if (PowerLoss_OutputPower != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate
                        {
                            PowerLoss_OutputPower.insertGraph(("Vi: " + averageSourceVolt + "V, Vo: " + averageSinkVolt + "V, " + testName), Output_Power, Test_power_Loss, Choosen_Color);
                            PowerLoss_OutputPower.insert_Log(("Test Name: " + testName + " [Input] Source Voltage: " + averageSourceVolt + "V, [Output] Sink Voltage: " + averageSinkVolt + "V"), Choosen_Color);
                        }));
                    }
                }
                catch (Exception)
                {
                    insert_Log("Could not add test data to Power Loss vs Output Load Power Graph", Error_Code);
                }
        }

        private void Clear_Output_Collected_Data()
        {
            Source_Voltage.Clear();
            Source_Current.Clear();
            Sink_Voltage.Clear();
            Sink_Current.Clear();
            Test_Efficiency.Clear();
            Input_Power.Clear();
            Output_Power.Clear();
            Test_power_Loss.Clear();
            CircuitResistance.Clear();
            SourceResistance.Clear();
            SinkResistance.Clear();
        }

        private void File_Border_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
            {
                
                string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filePath = System.IO.Path.GetFullPath(file[0]);
                string fileExtension = System.IO.Path.GetExtension(file[0]);
                string fileName = System.IO.Path.GetFileName(file[0]);
                if (fileExtension == ".txt")
                {
                    insert_Log("File Path: " + filePath, Message_Code);
                    File_Path.Text = filePath;
                    FilePath = filePath;
                    try
                    {
                        int NameFrom = fileName.IndexOf("");
                        int NameTo = fileName.LastIndexOf("-");
                        string TestName = fileName.Substring(NameFrom, NameTo);
                        Test_Name.Text = TestName.Trim();
                        testName = TestName.Trim();
                    }
                    catch (Exception) 
                    {
                        insert_Log("Cannot read test name. Please enter it manually.", Error_Code);
                        Test_Name.Text = string.Empty;
                    }
                }
                else 
                {
                    insert_Log("Invalid file type. Must be a .txt file.", Error_Code);
                }

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
