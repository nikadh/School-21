#include <curses.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

void init_matrix(int ***matrix, int n, int m);
void input_matrix(int **matrix, int n, int m);
void render(int **matrix, int n, int m);

void next_step(int **previous, int **current, int n, int m);
int is_equal(int **a, int **b, int n, int m);
int is_empty(int **matrix, int n, int m);

void controll(int *is_fine, int *speed, int *is_paused);

int get_at(int **matrix, int y, int x, int n, int m);
int neighbours_count(int **matrix, int y, int x, int n, int m);

int main() {
    int n = 25, m = 80, is_fine = 1, speed = 1000000, is_paused = 0;
    int **current, **previous;

    init_matrix(&current, n, m);
    init_matrix(&previous, n, m);

    input_matrix(current, n, m);

    fclose(stdin);
    is_fine = freopen("/dev/tty", "r", stdin) != NULL;

    initscr();
    noecho();
    curs_set(0);
    timeout(0);

    while (is_fine == 1) {
        controll(&is_fine, &speed, &is_paused);
        if (!is_paused) {
            render(current, n, m);
            int **tmp = previous;
            previous = current;
            current = tmp;

            next_step(previous, current, n, m);
            is_fine = is_fine && !is_equal(previous, current, n, m) && !is_empty(current, n, m);
            usleep(speed);
        }
    }

    endwin();
    free(current);
    free(previous);
    return 0;
}

void init_matrix(int ***matrix, int n, int m) {
    *matrix = malloc(n * sizeof(int *) + n * m * sizeof(int));
    int *ptr = (int *)(*matrix + n);
    for (int i = 0; i < n; i++) {
        (*matrix)[i] = ptr + i * m;
    }
}

void render(int **matrix, int n, int m) {
    clear();
    for (int i = 0; i < m + 2; i++) printw("-");
    for (int i = 0; i < n; i++) {
        printw("\n");
        printw("|");
        for (int j = 0; j < m; j++) {
            char c = matrix[i][j] ? '*' : ' ';
            printw("%c", c);
        }
        printw("|");
    }
    printw("\n");
    for (int i = 0; i < m + 2; i++) printw("-");
}

void input_matrix(int **matrix, int n, int m) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            scanf("%d", &(matrix[i][j]));
        }
    }
}

int get_at(int **matrix, int y, int x, int n, int m) {
    if (y == n)
        y = 0;
    else if (y == -1)
        y = n - 1;

    if (x == m)
        x = 0;
    else if (x < 0)
        x = m - 1;

    return matrix[y][x];
}

int neighbours_count(int **matrix, int y, int x, int n, int m) {
    int neighbours = 0;
    for (int i = y - 1; i <= y + 1; i++) {
        for (int j = x - 1; j <= x + 1; j++) {
            if (i != y || j != x) neighbours += get_at(matrix, i, j, n, m);
        }
    }
    return neighbours;
}

void next_step(int **previous, int **current, int n, int m) {
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < m; j++) {
            int neighbours = neighbours_count(previous, i, j, n, m);
            if (neighbours == 3)
                current[i][j] = 1;
            else if (neighbours == 2 && previous[i][j])
                current[i][j] = 1;
            else
                current[i][j] = 0;
        }
    }
}

int is_equal(int **a, int **b, int n, int m) {
    int is_equal = 1;
    for (int i = 0; i < n && is_equal; i++) {
        for (int j = 0; j < m && is_equal; j++) {
            is_equal = a[i][j] == b[i][j];
        }
    }
    return is_equal;
}
int is_empty(int **matrix, int n, int m) {
    int is_empty = 1;
    for (int i = 0; i < n && is_empty; i++) {
        for (int j = 0; j < m && is_empty; j++) {
            is_empty = matrix[i][j] == 0;
        }
    }

    return is_empty;
}

void controll(int *is_fine, int *speed, int *is_paused) {
    char c = getch();
    if (c == 'q' || c == 'Q') {
        *is_fine = 0;
    } else if (c == 'p' || c == 'P') {
        *is_paused = !*is_paused;
    } else if (c == '+' && *speed - 100000 > 0)
        *speed = *speed - 100000;
    else if (c == '-')
        *speed = *speed + 100000;
}