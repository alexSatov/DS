function phase_trajectory_3d(path)
    data = load(path);
    plot3(data(:, 1), data(:, 2), data(:, 3), '.');
    xlabel('Wx');
    ylabel('Wy');
    zlabel('Wz');
    grid on;
end