import matplotlib.pyplot as plt
from time import time
from typing import Callable, List

from model import State, Model
from phase_trajectory import get_phase_trajectory
from bifurcation_diagram import get_bifurcation_diagram_xd_parallel, get_bifurcation_diagram_dd_parallel


def draw_plot(model: Model, axis: List[float], size: int, func: Callable, *args):
    print(model)

    points = set(func(model, *args))
    components = list(zip(*points))

    print(f'points count: {len(points)}')
    print(f'before plot: {time() - start_time}')

    plt.axis(axis)
    plt.scatter(components[0], components[1], s=size)

    print(f'after plot: {time() - start_time}')

    plt.show()


def task1():
    draw_plot(Model(d12=0.002), [0, 40, 0, 80], 4,
              get_phase_trajectory, State(10, 10), 2000, 1000)


def task2():
    draw_plot(Model(), [0.0017, 0.0025, 10, 45], 3,
              get_bifurcation_diagram_xd_parallel, State(10, 10), 0.0025, 0.000005)


def draw_cycle():
    model = Model(d12=0.0017, d21=0.005)
    print(model)
    equilibrium, infs, cycles = get_bifurcation_diagram_dd_parallel(model, State(10, 10), 0.00245, 0.00792,
                                                                    0.000005, 0.00002)
    print(f'equilibrium points count: {len(equilibrium)}')
    print(f'infinity points count: {len(infs)}')

    for i in range(15):
        print(f'cycle period {i + 2} points count: {len(cycles[i + 2])}')

    print(f'before plot: {time() - start_time}')

    plt.axis([0.0017, 0.00245, 0.005, 0.00792])
    plt.scatter(*list(zip(*cycles[2])), s=3)

    print(f'after plot: {time() - start_time}')

    plt.show()


if __name__ == '__main__':
    start_time = time()
    draw_cycle()
