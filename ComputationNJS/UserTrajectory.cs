using System;
using System.IO;
using System.Collections.Generic;



namespace TrajectoryMetrics
{
	//Class of vector3 for not depend on System.Numerics of C#
	public class Vector3
	{
		//methods
		public double m_x;
		public double m_y;
		public double m_z;
		public Vector3(double x, double y, double z)
		{
			m_x = x;
			m_y = y;
			m_z = z;
		}

	}

	public class UserTrajectory
	{

		private double m_tStart;
		private double m_tEnd;
		private double m_deltaT;

		//Parameters of the trajectory
		public double m_arcLength;
		public double m_NJS;
		public double m_totalTime;

		private double[] m_t;
		private Vector3[] m_position;
		private Vector3[] m_velocity;
		private Vector3[] m_acceleration;
		private Vector3[] m_jerk;


		//Constructors
		public UserTrajectory()
		{

		}

		public UserTrajectory(double tStart, double tEnd, double deltaT)
		{
			m_tStart = tStart;
			m_tEnd = tEnd;
			m_deltaT = deltaT;
		}

		public void ShowInformation()
		{
			 Console.WriteLine("\t T: " + (m_totalTime).ToString("F4") +
                          "\t S: " + (m_arcLength).ToString("F4") +
                          "\t NJS: " + ((m_NJS) / 100000).ToString("F4") + "\n"); //Scale by 100000 to see an easier comparison
		}

		#region SMOOTH BASED METRICS

		public void ComputeNJSFromFile(string rootFolder, string patient, string session, string partida, string fileName)
		{

			// Define where the program will be searching for the .csv files of the trajectory
			string PATH_TO_FILE = rootFolder + "\\" + patient + "\\" + 
				   "Sesion " + session + "\\" + "Partida " +  partida + fileName;

			// Open file trajectory as lecture mode
			StreamReader endEffectorPosition = new StreamReader(PATH_TO_FILE);

			// Always read first line to omit header
			endEffectorPosition.ReadLine();

			// Create array that will hold the instants of time
			// where the function will be evaluated
			List<double> tElements = new List<double>();

			double t = 0.0;
			double tStart = 0.0;
			double tEnd = 0.0;
			double deltaTime = 1.0 / 60.0;

			string positionLine;
			string[] positions = { "0", "0", "0" };
			Vector3 positionVector;

			// Create the list of elements to get the vectors
			List<Vector3> positionElements = new List<Vector3>();

			// Iterate each row on the file
			while ((positionLine = endEffectorPosition.ReadLine()) != null)
			{

				//Split the line for get the position values
				positions = positionLine.Split(',');

				//Information is given in mm
				double px = Double.Parse(positions[0]);
				double py = Double.Parse(positions[1]);
				double pz = Double.Parse(positions[2]);

				positionVector = new Vector3(px, py, pz);

				positionElements.Add(positionVector);

				//Add the instant of time
				tElements.Add(t);

				t += deltaTime;

			}

			tEnd = t;

			//5.- Set the time from reading the information of the file
			m_tStart = tStart;
			m_tEnd = tEnd;
			m_deltaT = deltaTime;

			m_position = positionElements.ToArray();
			m_t = tElements.ToArray();

			ComputeNumericalVelocity();
			ComputeArcLength();
			ComputeNJS();

			//Always close the file
			endEffectorPosition.Close();
		}


		//Methods for generating the instants of time 
		// to evaluate the kinematics information		
		private void ComputeNumericalVelocity()
		{
			int n = m_position.Length;

			m_velocity = new Vector3[n];

			double vx;
			double vy;
			double vz;


			for (int k = 1; k < n - 1; k++)
			{
				vx = (m_position[k + 1].m_x - m_position[k - 1].m_x) / (m_t[k + 1] - m_t[k - 1]);
				vy = (m_position[k + 1].m_y - m_position[k - 1].m_y) / (m_t[k + 1] - m_t[k - 1]);
				vz = (m_position[k + 1].m_z - m_position[k - 1].m_z) / (m_t[k + 1] - m_t[k - 1]);


				m_velocity[k] = new Vector3(vx, vy, vz);
			}
			//Asumir que la velocidad v[0] es igual
			m_velocity[0] = m_velocity[1];

			//Y que la velocidad n-1(el último arreglo de la velocidad, es igual a la que tenía anteriormente)
			m_velocity[n - 1] = m_velocity[n - 2];

		}

		//Compute jerk as the second derivative of the velocity
		private void ComputeNumericalJerk()
		{

			int n = m_velocity.Length;

			double wx;
			double wy;
			double wz;

			m_jerk = new Vector3[n];

			for (int i = 2; i <= n - 2; i++)
			{
				wx = (m_velocity[i + 1].m_x - 2 * m_velocity[i].m_x + m_velocity[i - 1].m_x) / (Math.Pow(m_t[i + 1] - m_t[i], 2));
				wy = (m_velocity[i + 1].m_y - 2 * m_velocity[i].m_y + m_velocity[i - 1].m_y) / (Math.Pow(m_t[i + 1] - m_t[i], 2));
				wz = (m_velocity[i + 1].m_z - 2 * m_velocity[i].m_z + m_velocity[i - 1].m_z) / (Math.Pow(m_t[i + 1] - m_t[i], 2));


				m_jerk[i] = new Vector3(wx, wy, wz);

			}
		}

		//Methods for computing the information assumming only position is available
		private void ComputeArcLength()
		{
			double s = 0;

			double squareTerm;

			for (int k = 1; k < m_t.Length - 1; k++)
			{

				//Take the numerical derivative of the position(because I am assumming i do not know it)
				squareTerm = Math.Pow(m_velocity[k].m_x, 2) +
							 Math.Pow(m_velocity[k].m_y, 2) +
							 Math.Pow(m_velocity[k].m_z, 2);


				s += ((Math.Sqrt(squareTerm)) * m_deltaT);
			}

			m_arcLength = s;
		}

		private void ComputeNJS()
		{

			double sumaNJS = 0;

			double NJS;

			double squareJerk;
			double squareJerk2;


			//Compute the numerical jerk
			ComputeNumericalJerk();

			int n = m_velocity.Length;

			//Begin at 2 beacuase the last 2 jerks are not going to be on the
			//array and if we considered it, there is going to be an error
			//Because the big values of the velocities on the positions for going to one
			//to another, so thas why we cut off the last ones
			for (int k = 2; k < n - 3; k++)
			{

				//Take the numerical derivative of the position(because I am assumming i do not know it)
				squareJerk = Math.Pow(m_jerk[k].m_x, 2) +
							 Math.Pow(m_jerk[k].m_y, 2) +
							 Math.Pow(m_jerk[k].m_z, 2);

				squareJerk2 = Math.Pow(m_jerk[k + 1].m_x, 2) +
							 Math.Pow(m_jerk[k + 1].m_y, 2) +
							 Math.Pow(m_jerk[k + 1].m_z, 2);


				sumaNJS += (squareJerk + squareJerk2) * 0.5 * m_deltaT;
			}


			double T = m_tEnd - m_tStart;

			ComputeArcLength();

			NJS = Math.Sqrt((0.5 * Math.Pow(T, 5) / Math.Pow(m_arcLength, 2)) * sumaNJS);

			//Set private members
			m_NJS = NJS;
			m_totalTime = T;
		}

		#endregion


		// TO DO: IMPLEMENT PATH BASED METRICS ON .CSV FILEd
		#region PATH BASED METRICS 
		/// <summary>
		/// THOSE ARE THE ONES THAT LOG DISCRETE EVENTS
		///		TRE -> Target Re-entry
		///		TAC -> Task axis crossing
		///		MDC -> Movemente Direction Change
		///		ODC -> Orthogonal Direction Change
		/// </summary>



		/// <summary>
		/// THOSE ARE THE ONES THAT RECORD CONTINUOUS MEASURES
		///		MV -> Movement variability
		///		ME -> Movement error
		///		MO -> Movement offset
		/// </summary>

		public void ComputeTRE()
		{

		}

		public void ComputeTAC()
		{

		}


		#endregion



	}
}


