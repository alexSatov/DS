from typing import Tuple


class State:
    def __init__(self, x1: float, x2: float):
        self.x1 = x1
        self.x2 = x2

    def to_point(self) -> Tuple[float, float]:
        return self.x1, self.x2

    def __str__(self):
        return f'({self.x1}, {self.x2})'


class Model:
    def __init__(self, d12=0, d21=0.0063, a1=0.0002, a2=0.00052, b1=10, b2=20, px=0.25, py=1):
        self.a1: float = a1
        self.a2: float = a2
        self.b1: float = b1
        self.b2: float = b2
        self.px: float = px
        self.py: float = py
        self.d12: float = d12
        self.d21: float = d21

    def f(self, x1, x2) -> float:
        return (self.b1 / (self.px * self.py)) * \
               (self.a1 * x1 * (self.b1 - self.px * x1) + self.d12 * x2 * (self.b2 - self.px * x2))

    def g(self, x1, x2) -> float:
        return (self.b2 / (self.px * self.py)) * \
               (self.a2 * x2 * (self.b2 - self.px * x2) + self.d21 * x1 * (self.b1 - self.px * x1))

    def next_state(self, state: State):
        return State(self.f(state.x1, state.x2), self.g(state.x1, state.x2))

    def __str__(self):
        return f'Model: a1 = {self.a1}, a2 = {self.a2}, b1 = {self.b1}, b2 = {self.b2}, px = {self.px}, ' \
               f'py = {self.py}, d12 = {self.d12}, d21 = {self.d21}'
