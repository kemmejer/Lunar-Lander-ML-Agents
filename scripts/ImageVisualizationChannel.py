from pyclbr import Function
from enum import IntEnum
from mlagents_envs.environment import UnityEnvironment
from mlagents_envs.side_channel.side_channel import (
    SideChannel,
    IncomingMessage,
    OutgoingMessage,
)

import numpy as np
import uuid


class ImageGraphName(IntEnum):
    PositionImage = 0  # float x, float y
    RotationImage = 1  # float rotation (euler)
    VelocityImage = 2  # float velocityX, float velocity
    RewardImage = 3    # float reward
    ThrustImage = 4    # float thrust (0 = false, 1 = true)

class ImageVisualizationChannel(SideChannel):

    def __init__(self, add_data_callback: Function) -> None:
        super().__init__(uuid.UUID("E6FC6161-7938-4C11-825B-56870D25E80F"))
        self.add_data_callback: Function = add_data_callback


    def on_message_received(self, msg: IncomingMessage) -> None:
        graph_name: ImageGraphName = msg.read_int32()
        data: list[float] = msg.read_float32_list()
        self.add_data_callback(graph_name, data)


    def send_string(self, data: str) -> None:
        # Add the string to an OutgoingMessage
        msg = OutgoingMessage()
        msg.write_string(data)
        # We call this method to queue the data we want to send
        super().queue_message_to_send(msg)
