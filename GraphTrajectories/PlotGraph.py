import numpy as np
import matplotlib.pyplot as plt


def preprocess_information( 
        root_folder:str,
        patient:str, 
        session:str,
        game: str, 
        file_name: str) -> np.array:
    
    """Preprocess the trajectory followed by the user

    This functions preprocess the information from the .csv
    
    Args:
        root_folder : Folder where .csv files are located
        patient   : Name of the patient that we wnat to plot the information.
        session   : Number of session played by "patient".
        game      : Number of game played by "patient" on the "session".
        file_name : Name of the file to from which information is extracted.

    Returns:
        position_plot: Array of the extracted position from the .csv file.

    
    """

    # Path is assumed to be on Windows
    PATH_TO_FILE = f"{root_folder}\\{patient}\\Sesion {session}\\Partida {game}\\{file_name}"
   
    #Open the file
    file_end_effector_position = open(PATH_TO_FILE, 'r')

    #Read first line(header line)
    file_end_effector_position.readline()

    
    deltaT = 1/60
    t = 0
    time_passed = []
    position_end_effector = []

    #Save .csv on a python variables
    for line in file_end_effector_position:
        
        #Append, add position and update time
        time_passed.append(t)
        px, py, pz = line.split(",")
        positions = np.array([np.float64(px), np.float64(py), np.float64(pz)])
        position_end_effector.append(positions)
        t+=deltaT

    #Convert to "np.array" the "list" of elements
    position_plot = np.array(position_end_effector)

    file_end_effector_position.close()

    return position_plot

def plot_information(
        position_plot: np.array, 
        session: str, 
        game: str,
        save_graph = False
        ) -> None:
    
    """Plot the trajectory followed by the user

    This functions saves an image of the trajectory 
    that is stored on a directory as a .csv file.
    
    Args:
        position_plot : Position information(x, y, z) of the user as a np.array
        session : Number of session played by user
        partida: Number of partida played by the user

    Returns:
        Nothing, just saves the image 
        of the file

    """
    #Create the figure to plot the graph
    fig = plt.figure()
    fig.set_size_inches(8,6)
    fig.set_dpi(300)
    axes = fig.add_subplot(projection = '3d')

    #Plot the graph
    x = position_plot[:, 0]
    y = position_plot[:, 1]
    z = position_plot[:, 2]

    #Plot to coincide with the unity coordinate system
    axes.plot(x, z, y)
    axes.set_title(f"End effector position of Session {session} Game {game}")

    #Set the legends to the axis
    axes.set_xlabel("X (mm)")
    axes.set_ylabel("Z (mm)")
    axes.set_zlabel("Y (mm)")
    axes.set_proj_type('persp', focal_length =1)

    #Plot the initial and final point
    axes.plot(position_plot[0,0], position_plot[0,2], position_plot[0,1], 'gx')
    axes.plot(position_plot[-1,0], position_plot[-1,2], position_plot[-1,1], 'rx')
    axes.legend(["Path followed", "Initial position", "Final position"])
   
    #Save the figure of the plot
    figName = f"EndEffectorPosition{session}{game}"
    
    if save_graph:
        plt.savefig(f"{figName}.eps", bbox_inches='tight', pad_inches=0.4)

    plt.show()
   
def plot_user_trajectory(
        root_folder: str,
        patient: str, 
        session: str,
        game: str, 
        file_name: str
        ) -> None:
    
    """Wrapper to plot the trajectory followed by the user

    This functions is a wrapper that preprocess the information from the .csv file
    and saves an image of the trajectory as an .eps file.
    
    Args:
        patient: Name of the patient that we wnat to plot the information.
        session: Number of session played by "patient".
        game: Number of game played by "patient" on the "session".
        file_name : Name of the file to from which information is extracted.

    Returns:
        Nothing, just saves the image of the
        trajectory of the .csv file

    
    """
    
    position_plot = preprocess_information(root_folder, patient, session, game, file_name)
    
    plot_information(position_plot, session, game)