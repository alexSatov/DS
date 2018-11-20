import matplotlib.pyplot as plt
from time import time

from model import State, Model
from phase_trajectory import get_phase_trajectory
from bifurcation_diagram import get_bifurcation_diagram_parallel


def task1():
    model = Model(d12=0.002)
    print(model)
    points = get_phase_trajectory(model, State(10, 10), 2000, 1000, 1000)
    plt.axis([0, 40, 0, 80])
    plt.scatter(*list(zip(*points)), s=4)
    plt.show()


def task2():
    model = Model()
    print(model)
    points = get_bifurcation_diagram_parallel(model, State(10, 10))
    d12, x1, x2 = list(zip(*points))
    print(time() - start_time)
    plt.axis([0.0017, 0.00245, 10, 40])
    plt.scatter(d12, x1, s=3)
    print(time() - start_time)
    plt.show()


if __name__ == '__main__':
    start_time = time()
    task2()
