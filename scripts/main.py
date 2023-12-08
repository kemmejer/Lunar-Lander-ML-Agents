from ImageVisualization import ImageVisualization
import Train

imageVisualization = ImageVisualization()

if __name__ == '__main__':
    Train.custom_side_channels.append(imageVisualization.channel)
    Train.main()
