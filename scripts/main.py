import Train
from ImageVisualization import ImageVisualization

image_visualization = ImageVisualization()

if __name__ == '__main__':
    Train.custom_side_channels.append(image_visualization.channel)
    Train.main()
