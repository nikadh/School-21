#include "io.h"

void input(char *str) {
    char c = '\0';
    while (c != '\n') {
        scanf("%c", &c);
        *str = c == '\n' ? '\0' : c;
        str++;
    }
}

void init_matrix(int matrix[25][80]) {
    for (int y = 0; y < 25; y++) {
        for (int x = 0; x < 80; x++) {
            matrix[y][x] = 0;
        }
    }
}

void fill_matrix(struct polish *polish, int matrix[25][80]) {
    double scale_x = 80.0 / (4.0 * M_PI);
    // double scale_y = 25 / 2;

    for (int screen_x = 0; screen_x < 80; screen_x++) {
        double coord_x = screen_x / scale_x;
        double coord_y = calc(polish, coord_x);

        int screen_y = (int)round(coord_y * 12 + 12);

        if (screen_y >= 0 && screen_y < 25) {
            matrix[screen_y][screen_x] = 1;
        }
    }
}

void draw_matrix(int matrix[25][80]) {
    for (int y = 0; y < 25; y++) {
        for (int x = 0; x < 80; x++) {
            if (matrix[y][x]) {
                printf("*");
            } else {
                printf(".");
            }
        }
        printf("\n");
    }
}
