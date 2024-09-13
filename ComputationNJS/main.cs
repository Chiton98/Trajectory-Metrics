using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrajectoryMetrics;
using System.IO;

class TestSmoothMetricsWithData
{
    
    static void Main(string[] args)
    {
        //Define the path for search the "data" folder where the .csv files are stored
        string ROOT_FOLDER = @"..\data";

        string patientName = args[0];
        string session = args[1];
        string game = args[2];

        Console.WriteLine("\nComputing the NJS for " + patientName + " of session " + session + " game " + game + "\n");

        Console.WriteLine("T: Total time of the trajectory (seconds) \n" +
                          "S: Arc length of the trajectory (milimeters) \n" +
                          "NJS: Normalized Jerk Smoothness (adimensional) \n")

        //Define the full location of the trajectory file
        string FILE_NAME = @"\EndEffectorPositionDeviceCoordinates_Sesion" + session
                            + "_Partida" + game + ".csv";

        UserTrajectory myUserTrajectory = new UserTrajectory();

        myUserTrajectory.ComputeNJSFromFile(ROOT_FOLDER, patientName, session, game, FILE_NAME);

        myUserTrajectory.ShowInformation();

    }



}

