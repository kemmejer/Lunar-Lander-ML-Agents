import uuid
from enum import IntEnum
from pyclbr import Function

from mlagents_envs.side_channel.side_channel import (IncomingMessage,
                                                     OutgoingMessage,
                                                     SideChannel)


class ImageGraphName(IntEnum):
    WorldBounds = 0  # float minX, float minY, float maxX, float maxY
    Position = 1     # float x, float y
    Rotation = 2     # float rotation (euler)
    Velocity = 3     # float velocityX, float velocity
    Reward = 4       # float reward
    Thrust = 5       # float thrust (0 = false, 1 = true)


class ImageVisualizationChannel(SideChannel):

    def __init__(self, add_data_callback: Function) -> None:
        super().__init__(uuid.UUID("E6FC6161-7938-4C11-825B-56870D25E80F"))
        self.add_data_callback: Function = add_data_callback

    def on_message_received(self, msg: IncomingMessage) -> None:
        graph_name: ImageGraphName = msg.read_int32()
        data: list[float] = msg.read_float32_list()
        self.add_data_callback(graph_name, data)

    def send_string(self, data: str) -> None:
        msg = OutgoingMessage()
        msg.write_string(data)
        super().queue_message_to_send(msg)
