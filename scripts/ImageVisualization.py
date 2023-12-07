from mlagents_envs.environment import UnityEnvironment
import numpy as np
from ImageVisualizationChannel import ImageVisualizationChannel


def start():
    # Create the channel
    channel = ImageVisualizationChannel()

    print("Waiting for Unity to connect...")

    # We start the communication with the Unity Editor and pass the channel side channel as input
    env = UnityEnvironment(side_channels=[channel])
    env._get_communicator
    print("Connected to Unity")
