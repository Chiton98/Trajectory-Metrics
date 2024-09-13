import numpy as np
import matplotlib.pyplot as plt
from PlotGraph import *

### TO DO: MODIFY THE FILE TO JUST ACCEPT ONE 

#Define sessions to analize
print("Cargando los datos")
user_data = {1: [1,2,3,4,5,6,7],
             2: [1,2,3,4,5,6]}

#Define root folder to search for .csv files
ROOT_FOLDER = r"C:\Users\octus\Desktop\Tesis-Unity\PracticasPhantom\SeriousGame\PatientExpedients"

print("Generando las trayectorias")
#Specify the patient
name = r'\Octavio'
patient = name
for session in user_data:
    for partida in user_data[session]:
        
        file_name = r"\EndEffectorPositionDeviceCoordinates_Sesion" + str(session) + "_Partida" + str(partida) + ".csv"
        plot_user_trajectory(ROOT_FOLDER, patient, session, partida, file_name)

print("Trayectorias generadas")