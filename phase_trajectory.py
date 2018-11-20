from math import fabs
from model import Model
from typing import Set, Tuple


from model import State


def get_phase_trajectory(model: Model, state_0: State, t1: int, t2: int, x_count: int, x_max=100) -> \
        Set[Tuple[float, float]]:
    state = state_0
    points = set()

    for t in range(t1 + t2):
        next_state = model.next_state(state)

        if fabs(next_state.x1) > x_max or fabs(next_state.x2) > x_max:
            print(f'{state_0} -> inf')
            return set()

        if t >= t1 and (t - t1) < x_count:
            points.add(next_state.to_point())

        state = next_state

    return points
