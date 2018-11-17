import matplotlib.pyplot as plt
from model import Model
from phase_trajectories import build_pt

if __name__ == '__main__':
    fig = plt.figure()
    model = Model(0.002279, 0.0063)
    x, y = build_pt(model, 1, 1)
    plt.scatter(x, y)
    plt.show()
