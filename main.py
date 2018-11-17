import matplotlib.pyplot as plt
from model import Model
from phase_trajectories import build_phase_trajectory
from bifurcation_diagram import build_bifurcation_diagram


def task1():
    model = Model(d12=0.002279)
    x1, x2 = build_phase_trajectory(model, 1, 1)
    plt.scatter(x1, x2)
    plt.show()


def task2():
    model = Model()
    d12, x1, x2 = build_bifurcation_diagram(model, 10, 10)
    plt.axis([0.0017, 0.00245, 10, 40])
    plt.scatter(d12, x1, s=5.0)
    plt.show()


if __name__ == '__main__':
    task2()
