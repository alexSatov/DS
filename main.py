import matplotlib.pyplot as plt
from time import time
from model import Model
from phase_trajectory import build_phase_trajectory
from bifurcation_diagram import build_bifurcation_diagram_parallel


def task1():
    model = Model(d12=0.002)
    print(model)
    x1, x2 = build_phase_trajectory(model, 10, 10)
    plt.scatter(x1, x2, s=3)
    plt.show()


def task2():
    model = Model()
    print(model)
    d12, x1, x2 = build_bifurcation_diagram_parallel(model, 10, 10)
    plt.axis([0.0017, 0.00245, 10, 40])
    plt.scatter(d12, x1, s=3)
    print(time() - start_time)
    plt.show()


if __name__ == '__main__':
    start_time = time()
    task2()
