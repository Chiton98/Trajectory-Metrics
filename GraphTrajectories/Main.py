import numpy as np
import matplotlib.pyplot as plt
from PlotGraph import *
import sys

# User information is get from command line
patient_name = sys.argv[1]
session = int(sys.argv[2])
game = int(sys.argv[3])

print("Loading data")
ROOT_FOLDER = "..\\data"
FILE_NAME = f"EndEffectorPositionDeviceCoordinates_Sesion{session}_Partida{game}.csv"

print("Graphing plot")
plot_user_trajectory(ROOT_FOLDER, patient_name, session, game, FILE_NAME)

print("Closing program")