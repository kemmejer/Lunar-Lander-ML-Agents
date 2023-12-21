from enum import IntEnum
from typing import List

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import seaborn as sns

POSITION_STEP_SIZE = 0.5


def round_array(array: np.ndarray[float]) -> np.ndarray[float]:
    return np.round(array / POSITION_STEP_SIZE) * POSITION_STEP_SIZE


class ImageGraphName(IntEnum):
    WorldBounds = 0  # float minX, float minY, float maxX, float maxY
    Position = 1     # float x, float y
    Rotation = 2     # float rotation (euler)
    Velocity = 3     # float velocityX, float velocity
    Reward = 4       # float reward
    Thrust = 5       # float thrust (0 = false, 1 = true)


class WorldBounds:
    def __init__(self, world_bounds: List[float]) -> None:
        world_bounds = round_array(world_bounds)
        self.min_x: float = world_bounds[0]
        self.min_y: float = world_bounds[1]
        self.max_x: float = world_bounds[2]
        self.max_y: float = world_bounds[3]
        self.width: float = self.max_x - self.min_x
        self.height: float = self.max_y - self.min_y
        self.width_count: int = int(self.width / POSITION_STEP_SIZE) + 1
        self.height_count: int = int(self.height / POSITION_STEP_SIZE) + 1


class ImageVisualization:

    def __init__(self) -> None:
        self.data = [np.array(np.float) for _ in range(len(ImageGraphName))]

    def set_image_data(self, graph_name: ImageGraphName, data: List[float]) -> None:
        self.data[int(graph_name)] = np.asarray(data)
        if graph_name == ImageGraphName.WorldBounds:
            self.bounds = WorldBounds(self.data[ImageGraphName.WorldBounds])

    def generate_image(self, graph_name: ImageGraphName) -> plt.Figure | None:
        plt.figure(graph_name.value)
        figure: plt.Figure = None
        match graph_name:
            case ImageGraphName.Position:
                figure = self.generate_position_image()
            case ImageGraphName.Rotation:
                figure = self.generate_rotation_image()
            case ImageGraphName.Velocity:
                figure = self.generate_velocity_image()
            case ImageGraphName.Reward:
                figure = self.generate_reward_image()
            case ImageGraphName.Thrust:
                figure = self.generate_thrust_image()

        return figure

    def get_grid_positions(self) -> tuple[np.ndarray[float], np.ndarray[float]]:
        positions = self.data[int(ImageGraphName.Position)]
        positions = round_array(positions)

        # Fill the value and coordinate arrays with all possible positions. This ensures that there are no gaps / jumps in the dataframe
        x_fill = np.arange(self.bounds.min_x, self.bounds.max_x + POSITION_STEP_SIZE, POSITION_STEP_SIZE)
        y_fill = np.arange(self.bounds.min_y, self.bounds.max_y + POSITION_STEP_SIZE, POSITION_STEP_SIZE)

        # Pad the coordinate arrays to the same length
        if self.bounds.width > self.bounds.height:
            y_fill = np.pad(y_fill, (0, self.bounds.width_count - self.bounds.height_count), 'constant')
        else:
            x_fill = np.pad(x_fill, (0, self.bounds.height_count - self.bounds.width_count), 'constant')

        # Retrieve the coordinate arrays
        x_coords = np.append(positions[0::2], x_fill)
        y_coords = np.append(positions[1::2], y_fill)

        return x_coords, y_coords

    def generate_dataframe(self, values: np.ndarray[float]) -> pd.DataFrame:
        x_coords, y_coords = self.get_grid_positions()
        values = np.append(values, np.full(x_coords.size - values.size, np.nan))  # Pad the value array to the same length as the coordinate arrays
        df = pd.DataFrame(data={"x": x_coords, "y": y_coords, "z": values})

        return df

    def generate_dataframe_2d(self, values: np.ndarray[float]) -> pd.DataFrame:
        x_coords, y_coords = self.get_grid_positions()
        values = np.append(values, np.full(int(2 * (x_coords.size - values.size / 2)), np.nan)
                           )  # Pad the value array to the same length as the coordinate arrays
        u_values = values[0::2]
        v_values = values[1::2]
        df = pd.DataFrame(data={"x": x_coords, "y": y_coords, "u": u_values, "v": v_values})

        return df

    def generate_position_image(self) -> plt.Figure:
        positions = self.data[int(ImageGraphName.Position)]
        values = np.ones(int(positions.size / 2))

        df: pd.DataFrame = self.generate_dataframe(values)
        df = df.groupby(["x", "y"], as_index=False).sum(min_count=1)
        df = df.pivot(index="y", columns="x", values="z")

        axes: plt.Axes = sns.heatmap(df, cmap="viridis")
        axes.set_title("Position")
        axes.invert_yaxis()
        axes.set_aspect('equal', adjustable='box')

        return axes.get_figure()

    def generate_rotation_image(self) -> plt.Figure:
        rotations = self.data[int(ImageGraphName.Rotation)]
        df: pd.DataFrame = self.generate_dataframe(rotations)
        df = df.groupby(["x", "y"], as_index=False).mean()

        radians = np.radians(df["z"] + 90.0)
        u_values = np.cos(radians)
        v_values = np.sin(radians)

        quiver: plt.Quiver = plt.quiver(df["x"], df["y"], u_values, v_values, df["z"], pivot="mid", cmap="viridis")
        plt.title("Rotation")
        quiver.axes.set_aspect('equal', adjustable='box')

        colorbar: plt.Colorbar = plt.colorbar(quiver)
        colorbar.set_label("Euler Angle")

        return quiver.axes.get_figure()

    def generate_velocity_image(self) -> plt.Figure:
        velocity = self.data[int(ImageGraphName.Velocity)]
        df: pd.DataFrame = self.generate_dataframe_2d(velocity)
        df = df.groupby(["x", "y"], as_index=False).mean()

        magnitudes = np.sqrt(df["u"] ** 2 + df["v"] ** 2)
        quiver: plt.Quiver = plt.quiver(df["x"], df["y"], df["u"], df["v"], magnitudes, cmap="viridis")
        plt.title("Velocity")
        quiver.axes.set_aspect('equal', adjustable='box')

        colorbar: plt.Colorbar = plt.colorbar(quiver)
        colorbar.set_label('Magnitude of Velocity')

        return quiver.axes.get_figure()

    def generate_reward_image(self) -> plt.Figure:
        rewards = self.data[ImageGraphName.Reward]

        df: pd.DataFrame = self.generate_dataframe(rewards)
        df = df.groupby(["x", "y"], as_index=False).mean()
        df = df.pivot(index="y", columns="x", values="z")

        axes: plt.Axes = sns.heatmap(df, cmap="viridis", center=0.0)
        axes.set_title("Reward")
        axes.invert_yaxis()
        axes.set_aspect('equal', adjustable='box')

        return axes.get_figure()

    def generate_thrust_image(self) -> plt.Figure:
        thrust = self.data[ImageGraphName.Thrust]

        df: pd.DataFrame = self.generate_dataframe(thrust)
        df = df.groupby(["x", "y"], as_index=False).sum(min_count=1)
        df = df.pivot(index="y", columns="x", values="z")

        axes: plt.Axes = sns.heatmap(df, cmap="viridis")
        axes.set_title("Thrust")
        axes.invert_yaxis()
        axes.set_aspect('equal', adjustable='box')

        return axes.get_figure()
